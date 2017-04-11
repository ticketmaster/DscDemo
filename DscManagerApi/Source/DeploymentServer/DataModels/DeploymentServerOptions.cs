// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeploymentServerOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System;

    using Autofac;

    using Ticketmaster.Dsc.DeploymentServer.Logging;

    /// <summary>
    ///     The deployment server options.
    /// </summary>
    public class DeploymentServerOptions : IDeploymentServerOptions
    {
        /// <summary>
        ///     Gets or sets the configuration working path.
        /// </summary>
        public string ConfigurationWorkingPath { get; set; } = @"C:\ProgramData\Ticketmaster\Dsc\Configuration";

        /// <summary>
        ///     Gets or sets the container.
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        ///     Gets or sets the days to keep build history.
        /// </summary>
        public int DaysToKeepBuildHistory { get; set; } = 60;

        /// <summary>
        /// Gets or sets the default encryption certificate thumbprint.
        /// </summary>
        // TODO Add thumbprint here
        public string DefaultEncryptionCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the dsc manager endpoint.
        /// </summary>
        public string DscManagerEndpoint { get; set; } = "https://dsc.winsys.tmcs/api/v2";

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        public IDeploymentServerLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        public string NameOrConnectionString { get; set; } = "DeploymentServerContext";

        /// <summary>
        ///     Gets or sets the previous configurations stored.
        /// </summary>
        public int PreviousConfigurationsStored { get; set; } = 4;

        /// <summary>
        ///     Gets or sets the root configuration path.
        /// </summary>
        public string RootConfigurationPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory
                                                            + @"\Scripts\InvokeConfiguration.ps1";

        /// <summary>
        ///     Gets or sets a value indicating whether use job dashbaord.
        /// </summary>
        public bool UseJobDashbaord { get; set; } = true;

        /// <summary>
        ///     Gets or sets the worker count.
        /// </summary>
        public int WorkerCount { get; set; } = 4;
    }
}