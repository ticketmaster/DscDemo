// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscManagerOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataModels
{
    using System.Collections.Generic;
    using System.Web.Http;

    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    ///     The DscManagerOptions interface.
    /// </summary>
    public interface IDscManagerOptions
    {
        /// <summary>
        ///     Gets or sets the connection strings.
        /// </summary>
        ICollection<ConnectionStringEntry> ConnectionStrings { get; set; }

        /// <summary>
        ///     Gets or sets the credential repository options.
        /// </summary>
        ICredentialRepositoryOptions CredentialRepositoryOptions { get; set; }

        /// <summary>
        ///     Gets or sets the days to keep logs.
        /// </summary>
        int DaysToKeepLogs { get; set; }

        /// <summary>
        ///     Gets or sets the deployment server options.
        /// </summary>
        IDeploymentServerOptions DeploymentServerOptions { get; set; }

        /// <summary>
        ///     Gets or sets the error detail policy.
        /// </summary>
        IncludeErrorDetailPolicy ErrorDetailPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the node repository options.
        /// </summary>
        INodeRepositoryOptions NodeRepositoryOptions { get; set; }

        /// <summary>
        ///     Gets or sets the reporting endpoint options.
        /// </summary>
        IReportingEndpointOptions ReportingEndpointOptions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use api.
        /// </summary>
        bool UseApi { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use hangfire job server.
        /// </summary>
        bool UseHangfireJobServer { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use job dashboard.
        /// </summary>
        bool UseJobDashboard { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use slack logging.
        /// </summary>
        bool UseSlackLogging { get; set; }

        /// <summary>
        ///     Gets or sets the worker count.
        /// </summary>
        int WorkerCount { get; set; }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        void Save(string filename);
    }
}