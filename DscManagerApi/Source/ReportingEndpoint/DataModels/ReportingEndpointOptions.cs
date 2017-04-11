// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    /// <summary>
    /// The reporting endpoint options.
    /// </summary>
    public class ReportingEndpointOptions : IReportingEndpointOptions
    {
        /// <summary>
        /// Gets or sets the days to keep configuration reports.
        /// </summary>
        public int DaysToKeepConfigurationReports { get; set; } = 30;

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        public IReportingEndpointLogging Logging { get; set; }

        /// <summary>
        /// Gets or sets the name or connection string.
        /// </summary>
        public string NameOrConnectionString { get; set; } = "ReportingEndpointContext";
    }
}