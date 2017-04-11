// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MofBuildConfigurationData.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Services
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     The mof build configuration data.
    /// </summary>
    public class MofBuildConfigurationData
    {
        /// <summary>
        ///     Gets or sets the configuration data.
        /// </summary>
        public Hashtable ConfigurationData { get; set; }

        /// <summary>
        /// Gets or sets the configuration package name.
        /// </summary>
        public string ConfigurationPackageName { get; set; }

        /// <summary>
        /// Gets or sets the configuration package version.
        /// </summary>
        public string ConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the targets.
        /// </summary>
        public IEnumerable<string> Targets { get; set; }
    }
}