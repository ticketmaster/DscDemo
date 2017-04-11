// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationsController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.DeploymentServer.RequestModels;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.Views;

    /// <summary>
    ///     The deployment server controller.
    /// </summary>
    [RoutePrefix("configurations")]
    public class ConfigurationsController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationsController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="configurationService">
        /// The configuration Service.
        /// </param>
        /// <param name="deploymentServerLogging">
        /// The deployment Server Logging.
        /// </param>
        public ConfigurationsController(
            DeploymentServerContext context, 
            IConfigurationService configurationService, 
            IDeploymentServerLogging deploymentServerLogging)
        {
            this.Context = context;
            this.Repository = context.Set<Configuration>();
            this.DocumentRepository = context.Set<ConfigurationDocument>();
            this.ConfigurationService = configurationService;
            this.DeploymentServerLogging = deploymentServerLogging;
        }

        /// <summary>
        ///     Gets or sets the configuration service.
        /// </summary>
        public IConfigurationService ConfigurationService { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the deployment server logging.
        /// </summary>
        protected IDeploymentServerLogging DeploymentServerLogging { get; set; }

        /// <summary>
        ///     Gets or sets the document repository.
        /// </summary>
        protected DbSet<ConfigurationDocument> DocumentRepository { get; set; }

        /// <summary>
        ///     Gets or sets the repository.
        /// </summary>
        protected DbSet<Configuration> Repository { get; set; }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{id:int}", Name = "ConfigurationDelete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var entity = await this.Repository.FindAsync(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            if (entity.ArchiveTimestamp == null)
            {
                var newestArchive =
                    this.Repository.Where(e => e.Target == entity.Target && e.ArchiveTimestamp != null)
                        .OrderByDescending(o => o.ArchiveTimestamp)
                        .FirstOrDefault();
                if (newestArchive != null)
                {
                    newestArchive.ArchiveTimestamp = null;
                    this.Context.Entry(newestArchive).State = EntityState.Modified;
                }
            }

            this.Repository.Remove(entity);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.LogException(e);
                return this.InternalServerError(e);
            }

            return this.Ok();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{target}", Name = "ConfigurationGet")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult Get(string target)
        {
            var group = this.Repository.Where(e => e.Target == target);

            var activeConfig = group.FirstOrDefault(e => e.ArchiveTimestamp == null);
            if (activeConfig == null)
            {
                return this.NotFound();
            }

            var mainView = activeConfig.ToViewModel();

            var archiveConfigs = group.Where(e => e.ArchiveTimestamp != null);
            var archives = new List<ArchiveConfigurationView>();
            foreach (var archive in archiveConfigs)
            {
                archives.Add(archive.Map<ArchiveConfigurationView>());
            }

            mainView.ArchiveConfigurations = archives;

            return this.Ok(mainView);
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
        [Route]
        public IHttpActionResult GetAll(
            ODataQueryOptions<IGrouping<string, Configuration>> options, 
            int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var groups =
                (IQueryable<IGrouping<string, Configuration>>)
                options.ApplyTo(this.Repository.GroupBy(e => e.Target), settings);
            var configs = new List<ConfigurationView>();
            foreach (var group in groups)
            {
                var activeConfig = group.FirstOrDefault(e => e.ArchiveTimestamp == null);
                if (activeConfig == null)
                {
                    continue;
                }

                var mainView = activeConfig.ToViewModel();

                var archiveConfigs = group.Where(e => e.ArchiveTimestamp != null);
                var archives = archiveConfigs.Select(archive => archive.Map<ArchiveConfigurationView>()).ToList();
                mainView.ArchiveConfigurations = archives;

                configs.Add(mainView);
            }

            return this.Ok(new PagedResult<ConfigurationView>(configs, options, resultSize));
        }

        /// <summary>
        /// The get document.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("document/{id:int}", Name = "ConfigurationGetDocument")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetDocument(int id)
        {
            var entity = await this.Repository.FindAsync(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            return this.Ok(entity.Map<ConfigurationDocumentView>());
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="configurations">
        /// The configurations.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route]
        public async Task<IHttpActionResult> Post([EnumerableParameter]IEnumerable<ConfigurationRequest> configurations)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var enumerable = configurations as ConfigurationRequest[] ?? configurations.ToArray();
            foreach (var configuration in enumerable)
            {
                this.ConfigurationService.Publish(configuration.Document, configuration.Target, configuration.CertificateThumbprint);
            }

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.LogException(e);
                return this.InternalServerError(e);
            }

            return this.Created(this.Request.RequestUri, enumerable);
        }

        /// <summary>
        /// The restore.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("restore/{id:int}", Name = "ConfigurationRestore")]
        [HttpPut]
        public async Task<IHttpActionResult> Restore(int id)
        {
            var entityToRestore = await this.Repository.FindAsync(id);

            if (entityToRestore == null)
            {
                return this.NotFound();
            }

            if (entityToRestore.ArchiveTimestamp == null)
            {
                return this.BadRequest("The specified configuration is not an archived configuration.");
            }

            var activeEntity =
                await
                this.Repository.FirstOrDefaultAsync(
                    e => e.Target == entityToRestore.Target && e.ArchiveTimestamp == null);

            if (activeEntity != null)
            {
                activeEntity.ArchiveTimestamp = DateTime.UtcNow;
            }

            entityToRestore.ArchiveTimestamp = null;

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this.LogException(e);
                return this.InternalServerError(e);
            }

            return this.Get(entityToRestore.Target);
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void LogException(Exception e)
        {
            var err = e;
            do
            {
                this.DeploymentServerLogging?.RequestException(
                    this.Url.Request.RequestUri.ToString(), 
                    err.Message, 
                    err.Source, 
                    err.StackTrace);
                err = err.InnerException;
            }
            while (err != null);
        }
    }
}