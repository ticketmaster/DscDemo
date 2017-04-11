// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPropertyResult.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;

    /// <summary>
    /// The configuration property result.
    /// </summary>
    public class ConfigurationPropertyResult
    {
        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        public BuildView Build { get; set; }

        /// <summary>
        /// Gets or sets the configuration property.
        /// </summary>
        public ConfigurationPropertyView ConfigurationProperty { get; set; }
    }
}