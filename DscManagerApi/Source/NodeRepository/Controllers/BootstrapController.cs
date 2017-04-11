// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.NodeRepository;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Extensions;
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The bootstrap controller.
    /// </summary>
    [RoutePrefix("bootstrap")]
    public class BootstrapController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="nodeService">
        /// The node service.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public BootstrapController(
            NodeRepositoryContext context, 
            INodeConfigurationService nodeService, 
            INodeRepositoryLogging logging)
        {
            this.Context = context;
            this.NodeRepository = context.Set<Node>();
            this.ConfigurationPropertyRepository = context.Set<ConfigurationProperty>();
            this.NodeService = nodeService;
            this.Logging = logging;
        }

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

        /// <summary>
        ///     Gets or sets the node service.
        /// </summary>
        protected INodeConfigurationService NodeService { get; set; }

        [AllowAnonymous]
        [Route("localAgentProperties/{nodeName}", Name = "GetLocalAgentProperties")]
        public virtual async Task<IHttpActionResult> GetLocalAgentProperties(string nodeName)
        {
            return this.Ok(await this.GetLocalAgentPropertiesView(nodeName));
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        [AllowAnonymous]
        [Route("{nodeName}", Name = "GetBootstrap")]
        public virtual async Task<IHttpActionResult> Get(string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                var defView = this.GetDefaultBootstrap(nodeName);
                if (defView == null)
                {
                    this.Logging.BootstrapOptionsNotPresent();
                    throw new Exception("The required bootstrap options are not present in the database.");
                }

                return this.Ok(defView);
            }

            var node = await this.NodeRepository.FindNodeByName(nodeName);
            if (node == null)
            {
                var defView = this.GetDefaultBootstrap(nodeName);
                if (defView == null)
                {
                    this.Logging.BootstrapOptionsNotPresent();
                    throw new Exception("The required bootstrap options are not present in the database.");
                }

                return this.Ok(defView);
            }

            node.IncludeConfigurationProperties(this.Context);
            var view = this.GetBootstrapOptions(node);

            if (view == null)
            {
                this.Logging.BootstrapOptionsNotPresent();
                throw new Exception("The required bootstrap options are not present in the database.");
            }

            view.NodeData = node.Map<NodeDetailView>().ToNodeData();
            return this.Ok(view);
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <param name="nodeData">
        /// The node data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [AllowAnonymous]
        [Route("{nodeName}", Name = "BootstrapData")]
        public async Task<IHttpActionResult> Post(string nodeName, Hashtable nodeData)
        {
            var node = await this.NodeRepository.Include(n => n.Roles).FindNodeByName(nodeName);
            var request = new NodeRequest { NodeName = nodeName, BuildMof = true, NodeData = nodeData };
            this.Logging.BootstrapUpdateReceived(nodeName, nodeData);

            NodeDetailResult newNode;
            if (node == null)
            {
                newNode = await this.NodeService.CreateNode(request);
            }
            else
            {
                node.IncludeConfigurationProperties(this.Context);
                newNode = await this.NodeService.UpdateNode(request);
            }

            var map = new TypeMapping(typeof(NodeDetailResult), typeof(BootstrapResultView));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<BootstrapResultView>(
                    m => m.NodeData, 
                    () => newNode.NodeView.ToNodeData()));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<BootstrapResultView>(m => m.NodeName, () => nodeName));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<BootstrapResultView>(m => m.Build, () => newNode.Build));
            map.PropertyResolvers.Add(new DestinationMemberPropertyResolver<BootstrapResultView>(d => d.LocalAgentProperties, () => newNode.NodeView.LocalAgentProperties));
            var view = map.Map(newNode.NodeView.BootstrapProperties) as BootstrapResultView;

            return this.Ok(view);
        }

        /// <summary>
        /// The get default bootstrap.
        /// </summary>
        /// <param name="nodeName">
        /// The node Name.
        /// </param>
        /// <returns>
        /// The <see cref="BootstrapResultView"/>.
        /// </returns>
        protected BootstrapOptionsView GetDefaultBootstrap(string nodeName)
        {
            var bootstrapOptions =
                this.ConfigurationPropertyRepository.Where(
                    c => c.Type == PropertyType.Bootstrap && c.Scope == PropertyScope.Global);
            var map = new TypeMapping(typeof(IEnumerable<ConfigurationProperty>), typeof(BootstrapOptionsView));
            map.PropertyResolvers.Add(
                new CollectionToViewPropertyResolver<ConfigurationProperty>(b => b.Name, b => b.Value));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<BootstrapOptionsView>(m => m.NodeName, () => nodeName));
            var view = map.Map(bootstrapOptions) as BootstrapOptionsView;
            return view;
        }

        protected BootstrapOptionsView GetBootstrapOptions(Node node)
        {
            var map = new TypeMapping(typeof(IEnumerable<ConfigurationProperty>), typeof(BootstrapOptionsView));
            map.PropertyResolvers.Add(
                new CollectionToViewPropertyResolver<ConfigurationProperty>(b => b.Name, b => b.Value));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<BootstrapOptionsView>(m => m.NodeName, () => node.NodeName));
            var view = map.Map(node.BootstrapProperties) as BootstrapOptionsView;
            return view;
        }

        protected async Task<LocalAgentPropertiesView> GetLocalAgentPropertiesView(string name)
        {
            var node = await this.NodeRepository.FindNodeByName(name);
            IEnumerable<ConfigurationProperty> options;
            if (node == null)
            {
                options =
                    this.ConfigurationPropertyRepository.Where(
                        c => c.Type == PropertyType.LocalAgent && c.Scope == PropertyScope.Global);
            }
            else
            {
                node.IncludeConfigurationProperties(this.Context);
                options = node.LocalAgentProperties;
            }


            var map = new TypeMapping(typeof(IEnumerable<ConfigurationProperty>), typeof(LocalAgentPropertiesView));
            map.PropertyResolvers.Add(
                new CollectionToViewPropertyResolver<ConfigurationProperty>(b => b.Name, b => b.Value));
            map.PropertyResolvers.Add(
                new DestinationMemberPropertyResolver<LocalAgentPropertiesView>(m => m.NodeName, () => name));
            var view = map.Map(options) as LocalAgentPropertiesView;

            if (view == null)
            {
                this.Logging.BootstrapOptionsNotPresent();
                throw new Exception("The required local agent properties are not present in the database.");
            }

            return view;
        }
    }
}