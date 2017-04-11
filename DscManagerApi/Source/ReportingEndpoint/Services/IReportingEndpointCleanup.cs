// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportingEndpointCleanup.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Services
{
    /// <summary>
    /// The ReportingEndpointCleanup interface.
    /// </summary>
    public interface IReportingEndpointCleanup
    {
        /// <summary>
        /// The cleanup old reports.
        /// </summary>
        void CleanupOldReports();
    }
}