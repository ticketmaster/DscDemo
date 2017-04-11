// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    using Ticketmaster.Dsc.ReportingEndpoint.RequestModels;

    /// <summary>
    /// The reporting endpoint logging.
    /// </summary>
    public class ReportingEndpointLogging : IReportingEndpointLogging
    {
        /// <summary>
        /// The cleanup expired reports.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="daysToKeepReports">
        /// The days to keep reports.
        /// </param>
        public void CleanupExpiredReports(int count, int daysToKeepReports)
        {
        }

        /// <summary>
        /// The configuration report posted.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public void ConfigurationReportPosted(string target, ConfigurationReportRecordRequest request)
        {
        }

        /// <summary>
        /// The delete configuration reports node removed.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void DeleteConfigurationReportsNodeRemoved(int count, string nodeName)
        {
        }

        /// <summary>
        /// The upgrade reporting endpoint database schema.
        /// </summary>
        public void UpgradeReportingEndpointDatabaseSchema()
        {
        }
    }
}