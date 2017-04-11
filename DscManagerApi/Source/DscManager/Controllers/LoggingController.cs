// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Controllers
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Ticketmaster.Dsc.DscManager.DataAccess;
    using Ticketmaster.Dsc.DscManager.DataModels;
    using Ticketmaster.Dsc.DscManager.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;

    /// <summary>
    /// The logging controller.
    /// </summary>
    [RoutePrefix("logging")]
    public class LoggingController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public LoggingController(DscManagerContext context)
        {
            this.Context = context;
            this.LoggingRepository = context.Set<LoggingEntity>();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected DscManagerContext Context { get; set; }

        /// <summary>
        /// Gets or sets the logging repository.
        /// </summary>
        protected DbSet<LoggingEntity> LoggingRepository { get; set; }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{id:int}", Name = "LoggingViewDetail")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var log = await this.LoggingRepository.FindAsync(id);
            if (log == null)
            {
                return this.NotFound();
            }

            return this.Ok(log.ToViewModel());
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
        [Route]
        public IHttpActionResult GetAll(ODataQueryOptions<LoggingEntity> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var logRecords = options.ApplyTo(this.LoggingRepository, settings).OfType<LoggingEntity>();
            if (options.OrderBy == null)
            {
                logRecords = logRecords.OrderByDescending(l => l.Timestamp);
            }

            var logs = new List<LoggingView>();
            foreach (var record in logRecords)
            {
                logs.Add(record.ToViewModel());
            }

            return this.Ok(new PagedResult<LoggingView>(logs, options, resultSize));
        }
    }
}