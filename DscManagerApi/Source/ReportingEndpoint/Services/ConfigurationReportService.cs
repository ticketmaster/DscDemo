// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Services
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.DscManager.Services;
    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The configuration report service.
    /// </summary>
    public class ConfigurationReportService : DscEventHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReportService"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ConfigurationReportService(
            IDscEventManager eventManager, 
            ReportingEndpointContext context, 
            IReportingEndpointLogging logging)
            : base(eventManager)
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
        /// The handle event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task HandleEvent(DscEventArgs eventArgs)
        {
            if (eventArgs.Name == "DeleteNode")
            {
                var name = eventArgs.GetMember<string>("NodeName");
                var reports = this.ConfigurationReportRepository.Where(r => r.Target == name);
                if (reports.Any())
                {
                    this.ConfigurationReportRepository.RemoveRange(reports);
                    this.Logging.DeleteConfigurationReportsNodeRemoved(reports.Count(), name);
                    await this.Context.SaveChangesAsync();
                }
            }
        }
    }
}