// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodesController.cs" company="Ticketmaster">
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
    using System.Web.Http.OData.Query;

    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.NodeRepository;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Extensions;
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The nodes controller.
    /// </summary>
    [RoutePrefix("nodes")]
    public class NodesController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodesController"/> class.
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
        public NodesController(
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
        /// Gets or sets the logging.
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

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        [Route("{nodeName}/build", Name = "NodesBuild")]
        [HttpGet]
        public async Task<IHttpActionResult> Build(string nodeName)
        {
            var node = await this.NodeRepository.Include(n => n.Roles).FirstOrDefaultAsync(n => n.Name == nodeName);
            if (node == null)
            {
                return this.NotFound();
            }

            node.IncludeConfigurationProperties(this.Context);

            var view = node.Map<NodeDetailView>();
            var build = await this.NodeService.BuildNode(view);
            if (build == null)
            {
                throw new Exception("An error occurred when attempting to build this node.");
            }

            return this.Ok(build);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{nodeName}", Name = "NodesDelete")]
        public async Task<IHttpActionResult> Delete(string nodeName)
        {
            var node = await this.NodeRepository.Include(n => n.Roles).FirstOrDefaultAsync(n => n.NodeName == nodeName);
            if (node == null)
            {
                return this.NotFound();
            }

            var props = this.ConfigurationPropertyRepository.Where(p => p.Target == node.NodeName);

            this.ConfigurationPropertyRepository.RemoveRange(props);
            this.NodeRepository.Remove(node);
            await this.Context.SaveChangesAsync();
            return this.Ok();
        }

        /// <summary>
        /// The enter maintenance.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{nodeName}/enterMaintenance", Name = "NodesEnterMaintenance")]
        [HttpGet]
        public IHttpActionResult EnterMaintenance(string nodeName)
        {
            var result = this.NodeService.SetMaintenanceForNode(nodeName, true);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        /// <summary>
        /// The exit maintenance.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{nodeName}/exitMaintenance", Name = "NodesExitMaintenance")]
        [HttpGet]
        public IHttpActionResult ExitMaintenance(string nodeName)
        {
            var result = this.NodeService.SetMaintenanceForNode(nodeName, false);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
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
        [Route("{nodeName}", Name = "NodesGet")]
        public async Task<IHttpActionResult> Get(string nodeName)
        {
            var node = await this.NodeRepository.Include(n => n.Roles).FirstOrDefaultAsync(n => n.Name == nodeName);
            if (node == null)
            {
                return this.NotFound();
            }

            node.IncludeConfigurationProperties(this.Context);
            var view = node.Map<NodeDetailView>();
            return this.Ok(view);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="resultSize">
        /// The result size.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route(Name = "NodesGetAll")]
        public IHttpActionResult GetAll(ODataQueryOptions<Node> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }
            
            var nodes = (IQueryable<Node>)options.ApplyTo(this.NodeRepository.Include(e => e.Roles), settings);
            var list = new List<NodeView>();
            foreach (var node in nodes)
            {
                list.Add(node.ToViewModel());
            }
            
            return this.Ok(new PagedResult<NodeView>(list, options, resultSize));
        }

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("configurationData", Name = "NodesAllConfigurationData")]
        public IHttpActionResult GetAllConfigurationData(ODataQueryOptions<Node> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var nodes = (IQueryable<Node>)options.ApplyTo(this.NodeRepository.Include(e => e.Roles), settings);
            var list = new List<Hashtable>();
            foreach (var node in nodes)
            {
                node.IncludeConfigurationProperties(this.Context);
                list.Add(node.Map<NodeDetailView>().ToNodeData());
            }

            return this.Ok(new PagedResult<Hashtable>(list, options, resultSize));
        }

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{nodeName}/configurationData", Name = "NodesConfigurationData")]
        public async Task<IHttpActionResult> GetConfigurationData(string nodeName)
        {
            var node = await this.NodeRepository.Include(n => n.Roles).FirstOrDefaultAsync(n => n.Name == nodeName);
            node.IncludeConfigurationProperties(this.Context);

            if (node == null)
            {
                return this.NotFound();
            }

            var view = node.Map<NodeDetailView>();
            return this.Ok(view.ToNodeData());
        }

        /// <summary>
        /// The node agent error.
        /// </summary>
        /// <param name="requests">
        /// The requests.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("NodeAgentError")]
        [HttpPost]
        public IHttpActionResult NodeAgentError([EnumerableParameter] IEnumerable<NodeAgentErrorRequest> requests)
        {
            foreach (var request in requests)
            {
                this.Logging.NodeAgentError(request.Name, request.ErrorMessage);
            }

            return this.Ok();
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="requests">
        /// The requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route(Name = "NodesCreate")]
        public async Task<IHttpActionResult> Post([EnumerableParameter] IEnumerable<NodeRequest> requests)
        {
            if (!this.ModelState.IsValid || requests == null)
            {
                return this.BadRequest("Model is not valid.");
            }

            var detail = await this.NodeService.CreateNode(requests);
            return this.CreatedAtRoute("NodesGetAll", new Dictionary<string, object>(), detail);
        }

        /// <summary>
        /// The put.
        /// </summary>
        /// <param name="requests">
        /// The requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route(Name = "NodesUpdate")]
        public async Task<IHttpActionResult> Put([EnumerableParameter] IEnumerable<NodeRequest> requests)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Model is not valid.");
            }

            var results = await this.NodeService.UpdateNode(requests);
            var nodeDetailResults = results as NodeDetailResult[] ?? results.ToArray();

            if (!nodeDetailResults.Any())
            {
                return this.NotFound();
            }

            return this.Ok(nodeDetailResults);
        }
    }
}