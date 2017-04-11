// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeploymentServerOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using Ticketmaster.Dsc.DeploymentServer.Logging;

    /// <summary>
    ///     The DeploymentServerOptions interface.
    /// </summary>
    public interface IDeploymentServerOptions
    {
        /// <summary>
        ///     Gets or sets the configuration working path.
        /// </summary>
        string ConfigurationWorkingPath { get; set; }

        /// <summary>
        ///     Gets or sets the days to keep build history.
        /// </summary>
        int DaysToKeepBuildHistory { get; set; }

        /// <summary>
        ///     Gets or sets the default encryption certificate thumbprint.
        /// </summary>
        string DefaultEncryptionCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the dsc manager endpoint.
        /// </summary>
        string DscManagerEndpoint { get; set; }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        IDeploymentServerLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the previous configurations stored.
        /// </summary>
        int PreviousConfigurationsStored { get; set; }

        /// <summary>
        ///     Gets or sets the root configuration path.
        /// </summary>
        string RootConfigurationPath { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use job dashbaord.
        /// </summary>
        bool UseJobDashbaord { get; set; }

        /// <summary>
        ///     Gets or sets the worker count.
        /// </summary>
        int WorkerCount { get; set; }
    }
}