// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeStatusView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The node status view.
    /// </summary>
    public class NodeStatusView : IViewModel
    {
        /// <summary>
        ///     Gets or sets a value indicating whether is in compliance.
        /// </summary>
        public bool IsInCompliance { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is initial deployment.
        /// </summary>
        public bool IsInitialDeployment { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is in maintenance.
        /// </summary>
        public bool IsInMaintenance { get; set; }

        /// <summary>
        ///     Gets or sets the last apply configuration package name.
        /// </summary>
        public string LastApplyConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the last apply configuration version.
        /// </summary>
        public string LastApplyConfigurationVersion { get; set; }

        /// <summary>
        ///     Gets or sets the last apply run timestamp.
        /// </summary>
        public DateTime LastApplyRunTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the last bootstrap timestamp.
        /// </summary>
        public DateTime LastBootstrapTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the last monitoring run.
        /// </summary>
        public DateTime LastMonitoringRun { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the mof build configuration package name.
        /// </summary>
        public string MofBuildConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the mof build configuration version.
        /// </summary>
        public string MofBuildConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the mof build timestamp.
        /// </summary>
        public DateTime MofBuildTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            this.Links = new List<Link>();
        }
    }
}