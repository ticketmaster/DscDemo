// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointCleanup.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Services
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The reporting endpoint cleanup.
    /// </summary>
    public class ReportingEndpointCleanup : IReportingEndpointCleanup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingEndpointCleanup"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ReportingEndpointCleanup(
            ReportingEndpointContext context, 
            IReportingEndpointOptions options, 
            IReportingEndpointLogging logging)
        {
            this.Context = context;
            this.ConfigurationReportRepository = context.Set<ConfigurationReport>();
            this.Options = options;
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
        /// Gets or sets the options.
        /// </summary>
        protected IReportingEndpointOptions Options { get; set; }

        /// <summary>
        /// The cleanup old reports.
        /// </summary>
        public void CleanupOldReports()
        {
            var timeToRemove = DateTime.UtcNow.AddDays(this.Options.DaysToKeepConfigurationReports * -1);
            var toRemove = this.ConfigurationReportRepository.Where(r => r.EndDate < timeToRemove);
            this.ConfigurationReportRepository.RemoveRange(toRemove);
            this.Logging.CleanupExpiredReports(toRemove.Count(), this.Options.DaysToKeepConfigurationReports);
            this.Context.SaveChanges();
        }
    }
}