// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportingEndpointOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    /// <summary>
    /// The ReportingEndpointOptions interface.
    /// </summary>
    public interface IReportingEndpointOptions
    {
        /// <summary>
        /// Gets or sets the days to keep configuration reports.
        /// </summary>
        int DaysToKeepConfigurationReports { get; set; }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        IReportingEndpointLogging Logging { get; set; }

        /// <summary>
        /// Gets or sets the name or connection string.
        /// </summary>
        string NameOrConnectionString { get; set; }
    }
}