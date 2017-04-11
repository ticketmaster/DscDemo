// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;
    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.Extensions;
    using Ticketmaster.Dsc.ReportingEndpoint.RequestModels;

    /// <summary>
    /// The configuration report controller.
    /// </summary>
    [RoutePrefix("configurationReports")]
    public class ConfigurationReportController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReportController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ConfigurationReportController(ReportingEndpointContext context, IReportingEndpointLogging logging)
        {
            this.Context = context;
            this.ConfigurationReportRepository = context.Set<ConfigurationReport>();
            this.Logging = logging;
        }

        /// <summary>
        /// Gets or sets the configuration report repository.
        /// </summary>
        protected DbSet<ConfigurationReport> ConfigurationReportRepository { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected ReportingEndpointContext Context { get; set; }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        protected IReportingEndpointLogging Logging { get; set; }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{target}")]
        public IHttpActionResult Get(string target)
        {
            var results =
                this.ConfigurationReportRepository.Where(c => c.Target == target).ToConfigurationViews().ToArray();
            if (!results.Any())
            {
                return this.NotFound();
            }

            return this.Ok(results);
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
        public IHttpActionResult GetAll(ODataQueryOptions<ConfigurationReport> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var results =
                ((IQueryable<ConfigurationReport>)options.ApplyTo(this.ConfigurationReportRepository, settings))
                    .ToConfigurationViews();
            return this.Ok(new PagedResult<ConfigurationReportView>(results, options, resultSize));
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("id/{id}", Name = "ConfigurationReportGetById")]
        public IHttpActionResult GetById(string id)
        {
            Guid gid;
            if (!Guid.TryParse(id, out gid))
            {
                return this.BadRequest("The specified ID is not a Guid.");
            }

            var results =
                this.ConfigurationReportRepository.Include(e => e.Resources)
                    .Where(c => c.RunId == gid)
                    .ToConfigurationDetailView()
                    .ToArray();
            if (!results.Any())
            {
                return this.NotFound();
            }

            return this.Ok(results);
        }

        /// <summary>
        /// The get record by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("record/{id:int}", Name = "ConfigurationReportRecordGetById")]
        public async Task<IHttpActionResult> GetRecordById(int id)
        {
            var results =
                await this.ConfigurationReportRepository.Include(e => e.Resources).FirstOrDefaultAsync(r => r.Id == id);
            if (results == null)
            {
                return this.NotFound();
            }

            return this.Ok(results.Map<ConfigurationReportRecordDetailView>());
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
        [AllowAnonymous]
        [Route]
        public async Task<IHttpActionResult> Post(
            [EnumerableParameter] IEnumerable<ConfigurationReportRecordRequest> requests)
        {
            if (!this.ModelState.IsValid || requests == null)
            {
                return this.BadRequest(this.ModelState);
            }

            foreach (var request in requests)
            {
                this.Logging.ConfigurationReportPosted(request.Target, request);
                var report = request.Map<ConfigurationReport>();
                this.ConfigurationReportRepository.Add(report);
            }

            await this.Context.SaveChangesAsync();
            return this.Ok();
        }
    }
}