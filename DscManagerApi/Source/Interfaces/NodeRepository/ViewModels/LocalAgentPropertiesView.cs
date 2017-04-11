// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalAgentPropertiesView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The local agent properties view.
    /// </summary>
    public class LocalAgentPropertiesView : IViewModel
    {
        /// <summary>
        ///     Gets or sets the configuration endpoint.
        /// </summary>
        [Required]
        public string ConfigurationEndpoint { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the monitor interval.
        /// </summary>
        [Required]
        public int MonitorInterval { get; set; }

        /// <summary>
        ///     Gets or sets the node agent version.
        /// </summary>
        [Required]
        public string NodeAgentVersion { get; set; }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [Required]
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the package name.
        /// </summary>
        [Required]
        public string PackageName { get; set; }

        /// <summary>
        ///     Gets or sets the package version.
        /// </summary>
        [Required]
        public string PackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the reporting endpoint.
        /// </summary>
        [Required]
        public string ReportingEndpoint { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            this.Links = new List<Link>();
        }
    }
}