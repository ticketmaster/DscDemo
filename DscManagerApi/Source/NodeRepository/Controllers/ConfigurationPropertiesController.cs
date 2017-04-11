// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPropertiesController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Hangfire;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.NodeRepository;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Extensions;
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The configuration properties controller.
    /// </summary>
    [RoutePrefix("configurationProperties")]
    public class ConfigurationPropertiesController : ApiController
    {

        public ConfigurationPropertiesController(NodeRepositoryContext context, INodeConfigurationService nodeService, INodeRepositoryLogging logging)
        {
            this.Context = context;
            this.NodeRepository = context.Set<Node>();
            this.ConfigurationPropertiesRepository = context.Set<ConfigurationProperty>();
            this.NodeService = nodeService;
            this.Logging = logging;
        }

        internal IMofBuilderService BuildService { get; set; }
        protected INodeConfigurationService NodeService { get; set; }

        /// <summary>
        ///     Gets or sets the configuration properties repository.
        /// </summary>
        protected DbSet<ConfigurationProperty> ConfigurationPropertiesRepository { get; set; }

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

        [HttpGet]
        [Route("build")]
        public async Task<IHttpActionResult> Build(PropertyScope scope, string target)
        {
            var nodesToBuild = new Dictionary<PropertyScope, ICollection<string>>();
            this.GetTargetsFromScope(
                nodesToBuild,
                scope,
                target);
            if (nodesToBuild.Count < 1)
            {
                return this.NotFound();
            }

            var build = await this.BuildService.CreateBuild();
            BackgroundJob.Enqueue(() => this.NodeService.BuildNode(build.Id, nodesToBuild));
            return this.Ok(build);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{id:int}", Name = "ConfigurationPropertiesDelete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var property = await this.ConfigurationPropertiesRepository.FindAsync(id);
            if (property == null)
            {
                return this.NotFound();
            }

            this.ConfigurationPropertiesRepository.Remove(property);
            await this.Context.SaveChangesAsync();

            return this.Ok();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{id:int}", Name = "ConfigurationPropertiesDetail")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var property = await this.ConfigurationPropertiesRepository.FindAsync(id);
            if (property == null)
            {
                return this.NotFound();
            }

            return this.Ok(property.ToViewModel());
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
        /// The <see cref="Task"/>.
        /// </returns>
        [Route(Name = "ConfigurationPropertiesGetAll")]
        public IHttpActionResult GetAll(
            ODataQueryOptions<ConfigurationProperty> options, 
            int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var properties = new List<ConfigurationPropertyView>();
            var odataProps = options.ApplyTo(this.ConfigurationPropertiesRepository.AsQueryable(), settings).OfType<ConfigurationProperty>();
            foreach (var prop in odataProps)
            {
                properties.Add(prop.ToViewModel());
            }

            return this.Ok(new PagedResult<ConfigurationPropertyView>(properties, options, resultSize));
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route(Name = "ConfigurationPropertiesCreate")]
        public async Task<IHttpActionResult> Post(
            [EnumerableParameter] IEnumerable<ConfigurationPropertyRequest> request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Model is not valid.");
            }

            var models = new List<ConfigurationProperty>();
            var nodesToBuild = new Dictionary<PropertyScope, ICollection<string>>();
            var mapper = new TypeMapping<ConfigurationPropertyRequest, ConfigurationProperty>();
            foreach (var req in request)
            {
                var model = mapper.Map(req) as ConfigurationProperty;
                if (model == null)
                {
                    continue;
                }

                models.Add(model);
                this.ConfigurationPropertiesRepository.Add(model);

                if (req.BuildMof && (model.Type == PropertyType.Node || model.Type == PropertyType.ResourceVersion))
                {
                    this.GetTargetsFromProperty(nodesToBuild, model);
                }
            }

            await this.Context.SaveChangesAsync();
            var build = await this.BuildService.CreateBuild();
            BackgroundJob.Enqueue(() => this.NodeService.BuildNode(build.Id, nodesToBuild));

            var views = new List<ConfigurationPropertyResult>();
            models.ForEach(m => views.Add(new ConfigurationPropertyResult { Build = build, ConfigurationProperty = m.ToViewModel() }));
            return this.CreatedAtRoute("ConfigurationPropertiesGetAll", new Dictionary<string, object>(), views);
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
        [Route(Name = "ConfigurationPropertiesUpdate")]
        public async Task<IHttpActionResult> Put(
            [EnumerableParameter] IEnumerable<ConfigurationPropertyUpdateRequest> requests)
        {           
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Model is not valid.");
            }

            var models = new List<ConfigurationProperty>();
            var nodesToBuild = new Dictionary<PropertyScope, ICollection<string>>();
            var configurationPropertyUpdateRequests = requests as ConfigurationPropertyUpdateRequest[]
                                                      ?? requests.ToArray();
            var ids = configurationPropertyUpdateRequests.Select(r => r.Id);
            var properties = this.ConfigurationPropertiesRepository.Where(p => ids.Contains(p.Id));

            foreach (var req in configurationPropertyUpdateRequests)
            {
                var entity = await properties.FirstOrDefaultAsync(p => p.Id == req.Id);
                if (entity == null)
                {
                    continue;
                }

                var model = req.MergeCommonProperties(entity);
                models.Add(model);
                if (req.BuildMof && (model.Type == PropertyType.Node || model.Type == PropertyType.ResourceVersion))
                {
                    this.GetTargetsFromProperty(nodesToBuild, model);
                }
            }

            await this.Context.SaveChangesAsync();

            var build = await this.BuildService.CreateBuild();
            BackgroundJob.Enqueue(() => this.NodeService.BuildNode(build.Id, nodesToBuild));

            var views = new List<ConfigurationPropertyResult>();
            models.ForEach(m => views.Add(new ConfigurationPropertyResult {Build = build, ConfigurationProperty = m.ToViewModel() }));
            return this.CreatedAtRoute("ConfigurationPropertiesGetAll", new Dictionary<string, object>(), views);
        }

        private Expression<Func<Node, bool>> GetExpressionValue(Expression<Func<Node, bool>> expression)
        {
            return expression;
        }

        protected void GetTargetsFromProperty(Dictionary<PropertyScope, ICollection<string>> collection, ConfigurationProperty property)
        {
            this.GetTargetsFromScope(collection, property.Scope, property.Target);
        }

        protected void GetTargetsFromScope(
            Dictionary<PropertyScope, ICollection<string>> collection,
            PropertyScope scope,
            string target)
        {
            switch (scope)
            {
                case PropertyScope.Global:
                    if (!collection.ContainsKey(PropertyScope.Global))
                    {
                        collection.Add(PropertyScope.Global, new List<string> { "Global" });
                    }
                    return;
                case PropertyScope.Site:
                    if (!collection.ContainsKey(PropertyScope.Site))
                    {
                        collection.Add(PropertyScope.Site, new List<string> { target });
                    }
                    else
                    {
                        collection[PropertyScope.Site].Add(target);
                    }
                    return;
                case PropertyScope.ConfigurationEnvironment:
                    if (!collection.ContainsKey(PropertyScope.ConfigurationEnvironment))
                    {
                        collection.Add(PropertyScope.ConfigurationEnvironment, new List<string> { target });
                    }
                    else
                    {
                        collection[PropertyScope.ConfigurationEnvironment].Add(target);
                    }
                    return;
                case PropertyScope.Role:
                    if (!collection.ContainsKey(PropertyScope.Role))
                    {
                        collection.Add(PropertyScope.Role, new List<string> { target });
                    }
                    else
                    {
                        collection[PropertyScope.Role].Add(target);
                    }
                    return;
                case PropertyScope.Node:
                    if (!collection.ContainsKey(PropertyScope.Node))
                    {
                        collection.Add(PropertyScope.Node, new List<string> { target });
                    }
                    else
                    {
                        collection[PropertyScope.Node].Add(target);
                    }
                    return;
            }
        }
    }
}