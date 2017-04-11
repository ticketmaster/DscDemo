// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildsController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Hangfire;
    using Hangfire.Storage;

    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.RequestModels;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;

    /// <summary>
    ///     The builds controller.
    /// </summary>
    [RoutePrefix("builds")]
    public class BuildsController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildsController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="monitoringApi">
        /// The monitoring api.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        public BuildsController(
            DeploymentServerContext context, 
            IMonitoringApi monitoringApi, 
            IMofBuilderService service)
        {
            this.Context = context;
            this.MonitoringApi = monitoringApi;
            this.BuildRepository = context.Set<Build>();
            this.BuildTargetsRepository = context.Set<BuildTarget>();
            this.MofBuilderService = service;
        }

        /// <summary>
        ///     Gets or sets the build repository.
        /// </summary>
        protected DbSet<Build> BuildRepository { get; set; }

        /// <summary>
        ///     Gets or sets the build targets repository.
        /// </summary>
        protected DbSet<BuildTarget> BuildTargetsRepository { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the mof builder service.
        /// </summary>
        protected IMofBuilderService MofBuilderService { get; set; }

        /// <summary>
        ///     Gets or sets the monitoring api.
        /// </summary>
        protected IMonitoringApi MonitoringApi { get; set; }

        /// <summary>
        /// The cancel.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{id:int}/cancel", Name = "BuildsCancel")]
        [HttpGet]
        public async Task<IHttpActionResult> Cancel(int id)
        {
            var build = await this.BuildRepository.Include(e => e.Targets).FirstOrDefaultAsync(e => e.Id == id);

            if (build == null)
            {
                return this.NotFound();
            }

            var jobIds = build.Targets.Select(t => t.JobId).Distinct();
            foreach (var job in jobIds)
            {
                BackgroundJob.Delete(job.ToString());
            }

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
        [Route("{id:int}", Name = "BuildsDetail")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Get(int id)
        {
            var build = await this.BuildRepository.Include(e => e.Targets).FirstOrDefaultAsync(e => e.Id == id);

            if (build == null)
            {
                return this.NotFound();
            }

            var view = build.Map<BuildDetailView>();

            var jobs = build.Targets.Select(target => target.JobId).Distinct().ToList();

            view.Jobs = this.MofBuilderService.GetJobViews(jobs);

            view.SubmissionJob = this.MofBuilderService.GetJobView(build.SubmissionJobId);

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
        [Route(Name = "BuildsGetAll")]
        public IHttpActionResult GetAll(ODataQueryOptions<Build> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var list = new List<BuildView>();
            var items = (IQueryable<Build>)options.ApplyTo(this.BuildRepository, settings);
            foreach (var item in items)
            {
                list.Add(item.ToViewModel());
            }

            return this.Ok(new PagedResult<BuildView>(list, options, resultSize));
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
        [Route]
        public async Task<IHttpActionResult> Post(BuildRequest request)
        {
            // this.Validate(request);
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Model is not valid.");
            }

            var build =
                await
                this.MofBuilderService.Build(
                    request.ConfigurationData, 
                    request.ConfigurationPackageName,
                    request.ConfigurationPackageVersion,
                    request.CertificateThumbprint);
            return this.Ok(build);
        }
    }
}