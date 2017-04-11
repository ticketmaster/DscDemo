// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeConfigurationService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Services
{
    using Hangfire;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Extensions;
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The node configuration service.
    /// </summary>
    public class NodeConfigurationService : INodeConfigurationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeConfigurationService"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public NodeConfigurationService(NodeRepositoryContext context, INodeRepositoryLogging logging)
        {
            this.Context = context;
            this.ConfigurationPropertyRepository = context.Set<ConfigurationProperty>();
            this.NodeRepository = context.Set<Node>();
            this.Logging = logging;
        }

        /// <summary>
        ///     Gets or sets the build service.
        /// </summary>
        public IMofBuilderService BuildService { get; set; }

        /// <summary>
        ///     Gets or sets the configuration property repository.
        /// </summary>
        protected DbSet<ConfigurationProperty> ConfigurationPropertyRepository { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected NodeRepositoryContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected INodeRepositoryLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the node repository.
        /// </summary>
        protected DbSet<Node> NodeRepository { get; set; }

        public async Task<BuildDetailView> CreateBuildJob(string certificateThumbprint)
        {
            if (this.BuildService == null)
            {
                this.Logging.BuildRequestedNoBuildService(new NodeRequest());
                return null;
            }

            return await this.BuildService.CreateBuild();
        }

        /// <summary>
        /// The build node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<BuildDetailView> BuildNode(NodeDetailView node)
        {
            return await this.BuildNode(new List<NodeDetailView> { node });
        }

        [Queue("priority")]
        public void BuildNode(int buildId, Dictionary<PropertyScope, ICollection<string>> collection)
        {
            if (this.BuildService == null)
            {
                this.Logging.BuildRequestedNoBuildService(new NodeRequest());
                return;
            }


            IEnumerable<Node> nodes;
            if (collection.ContainsKey(PropertyScope.Global))
            {
                nodes = this.NodeRepository.Include(r => r.Roles);
            }
            else
            {
                var nodeList = new List<Node>();
                foreach (var scope in collection)
                {
                    var targetList = scope.Value.Distinct().ToArray();
                    switch (scope.Key)
                    {
                        case PropertyScope.Site:
                            nodeList.AddRange(this.NodeRepository.Include(r => r.Roles).Where(n => targetList.Contains(n.Site)));
                            break;
                        case PropertyScope.ConfigurationEnvironment:
                            nodeList.AddRange(this.NodeRepository.Include(r => r.Roles).Where(n => targetList.Contains(n.ConfigurationEnvironment)));
                            break;
                        case PropertyScope.Role:
                            foreach (var target in targetList)
                            {
                                nodeList.AddRange(
                                    this.NodeRepository.Include(r => r.Roles)
                                        .Where(n => n.Roles.Select(r => r.Name).Contains(target)));
                            }
                            break;
                        case PropertyScope.Node:
                            nodeList.AddRange(this.NodeRepository.Include(r => r.Roles).Where(n => targetList.Contains(n.NodeName) || targetList.Contains(n.Name) || targetList.Contains(n.QualifiedName)));
                            break;
                    }
                }

                nodes = nodeList;
            }

            nodes.IncludeConfigurationProperties(this.Context);

            var nodesArray = nodes.ToArray();
            if (nodesArray.Length == 0)
            {
                var task = this.BuildService.SetBuildStatus(buildId, BuildStatus.Succeeded);
                task.Wait();
                return;
            }

            var views = new List<NodeDetailView>();
            foreach (var n in nodesArray.Distinct())
            {
                views.Add(n.Map<NodeDetailView>());
            }

            var task2 = this.BuildService.SubmitBuild(buildId, this.GetConfigurationData(views), string.Empty, string.Empty, string.Empty);
            task2.Wait();
        }

        /// <summary>
        /// The build node.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<BuildDetailView> BuildNode(IEnumerable<NodeDetailView> nodes)
        {
            var nodeDetailViews = nodes as NodeDetailView[] ?? nodes.ToArray();
            if (this.BuildService == null)
            {
                this.Logging.BuildRequestedNoBuildService(nodeDetailViews);
                return null;
            }

            return await this.BuildService.Build(this.GetConfigurationData(nodeDetailViews));
        }

        /// <summary>
        /// The create node.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<NodeDetailResult> CreateNode(NodeRequest nodeRequest)
        {
            return (await this.CreateNode(new List<NodeRequest> { nodeRequest })).FirstOrDefault();
        }

        /// <summary>
        /// The create node.
        /// </summary>
        /// <param name="nodeRequests">
        /// The node requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<NodeDetailResult>> CreateNode(IEnumerable<NodeRequest> nodeRequests)
        {
            var details = new List<NodeDetailResult>();
            foreach (var nodeRequest in nodeRequests)
            {
                var detail = new Node();
                detail.Merge(nodeRequest.NodeData, this.Context);
                this.NodeRepository.Add(detail);
                detail.IncludeConfigurationProperties(this.Context);
                var view = detail.Map<NodeDetailView>();
                BuildView build = null;
                if (nodeRequest.BuildMof)
                {
                    if (this.BuildService == null)
                    {
                        this.Logging.BuildRequestedNoBuildService(nodeRequest);
                    }
                    else
                    {
                        build = await this.BuildService.Build(this.GetConfigurationData(view));
                    }
                }

                var result = new NodeDetailResult { Build = build, NodeView = view };
                details.Add(result);
                this.Logging.NodeCreated(detail.Name);
                if (detail.IsInitialDeployment)
                {
                    this.Logging.NodeChangedInitialDeployment(detail.Name, detail.IsInitialDeployment);
                }
            }

            await this.Context.SaveChangesAsync();
            return details;
        }

        /// <summary>
        /// The delete node.
        /// </summary>
        /// <param name="nodeName">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> DeleteNode(string nodeName)
        {
            var node = await this.NodeRepository.FindNodeByName(nodeName);
            if (node == null)
            {
                return false;
            }

            this.NodeRepository.Remove(node);
            this.ConfigurationPropertyRepository.RemoveRange(
                this.ConfigurationPropertyRepository.Where(p => p.Scope == PropertyScope.Node && p.Target == node.Name));
            await this.Context.SaveChangesAsync();
            this.Logging.DeleteNode(node.Name);
            return true;
        }

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        public Hashtable GetConfigurationData(IEnumerable<NodeDetailView> nodes)
        {
            var data = new Hashtable();
            var nodeData = nodes.Select(n => n.ToNodeData());
            data.Add("AllNodes", nodeData);
            return data;
        }

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        public Hashtable GetConfigurationData(IEnumerable<NodeRequest> nodes)
        {
            var data = new Hashtable();
            var nodeData = nodes.Select(n => n.NodeData);
            data.Add("AllNodes", nodeData);
            return data;
        }

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        public Hashtable GetConfigurationData(NodeDetailView node)
        {
            return this.GetConfigurationData(new List<NodeDetailView> { node });
        }

        /// <summary>
        /// The set maintenance for node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<NodeView> SetMaintenanceForNode(string nodeName, bool isInMaintenance)
        {
            return (await this.SetMaintenanceForNode(new[] { nodeName }, isInMaintenance)).FirstOrDefault();
        }

        /// <summary>
        /// The set maintenance for node.
        /// </summary>
        /// <param name="nodeNames">
        /// The node names.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<NodeView>> SetMaintenanceForNode(
            IEnumerable<string> nodeNames, 
            bool isInMaintenance)
        {
            var names = nodeNames.ToArray();
            var nodes =
                this.NodeRepository.Where(
                    n => names.Contains(n.Name) || names.Contains(n.QualifiedName) || names.Contains(n.NodeName));
            var results = new List<NodeView>();
            foreach (var node in nodes)
            {
                node.IsInMaintenance = isInMaintenance;
                results.Add(node.ToViewModel());
                this.Logging.NodeChangedIsInMaintenance(node.Name, isInMaintenance);
            }

            await this.Context.SaveChangesAsync();

            return results.Any() ? results : null;
        }

        /// <summary>
        /// The update node.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<NodeDetailResult> UpdateNode(NodeRequest nodeRequest)
        {
            return (await this.UpdateNode(new List<NodeRequest> { nodeRequest })).FirstOrDefault();
        }

        /// <summary>
        /// The update node.
        /// </summary>
        /// <param name="nodeRequests">
        /// The node requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<NodeDetailResult>> UpdateNode(IEnumerable<NodeRequest> nodeRequests)
        {
            var details = new List<NodeDetailResult>();
            foreach (var entry in nodeRequests)
            {
                var nodeRequest = entry;
                var detail = await this.NodeRepository.Include(n => n.Roles).FindNodeByName(entry.NodeName);
                detail.IncludeConfigurationProperties(this.Context);
                if (detail == null)
                {
                    continue;
                }

                var dirty = detail.Merge(nodeRequest.NodeData, this.Context);
                detail.IncludeConfigurationProperties(this.Context, true);
                var view = detail.Map<NodeDetailView>();
                BuildView build = null;
                if (nodeRequest.BuildMof && dirty)
                {
                    if (this.BuildService == null)
                    {
                        this.Logging.BuildRequestedNoBuildService(nodeRequest);
                    }
                    else
                    {
                        build = await this.BuildService.Build(this.GetConfigurationData(view));
                    }
                }

                var result = new NodeDetailResult { Build = build, NodeView = view };
                details.Add(result);
                this.Logging.NodeUpdated(detail.Name);
                if (entry.NodeData.ContainsKey("IsInitialDeployment")
                    && (bool)entry.NodeData["IsInitialDeployment"] != result.NodeView.IsInitialDeployment)
                {
                    this.Logging.NodeChangedInitialDeployment(result.NodeView.Name, result.NodeView.IsInitialDeployment);
                }
            }

            await this.Context.SaveChangesAsync();
            return details;
        }
    }
}