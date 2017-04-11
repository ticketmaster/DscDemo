// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeStatus.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;

    /// <summary>
    ///     The node status.
    /// </summary>
    [Table("NodeStatus")]
    public class NodeStatus : ModelBase<NodeStatusView>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NodeStatus" /> class.
        /// </summary>
        public NodeStatus()
        {
            var map = this.TypeMappings.FirstOrDefault(t => t.DestinationType == typeof(NodeStatusView));
            map?.PropertyResolvers.Add(
                new DestinationPropertyFromSourcePropertyResolver<NodeStatus, NodeStatusView>(
                    d => d.IsInCompliance, 
                    s => s.MofBuildConfigurationPackageVersion == s.LastApplyConfigurationVersion));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is initial deployment.
        /// </summary>
        [Index(IsUnique = false)]
        public bool IsInitialDeployment { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is in maintenance.
        /// </summary>
        [Index(IsUnique = false)]
        public bool IsInMaintenance { get; set; }

        /// <summary>
        /// Gets or sets the last apply configuration package name.
        /// </summary>
        public string LastApplyConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the last apply configuration version.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(64)]
        public string LastApplyConfigurationVersion { get; set; }

        /// <summary>
        ///     Gets or sets the last apply run timestamp.
        /// </summary>
        [Index(IsUnique = false)]
        public DateTime LastApplyRunTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the last bootstrap timestamp.
        /// </summary>
        public DateTime LastBootstrapTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the last monitoring run.
        /// </summary>
        [Index(IsUnique = false)]
        public DateTime LastMonitoringRun { get; set; }

        /// <summary>
        /// Gets or sets the mof build configuration package name.
        /// </summary>
        public string MofBuildConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the mof build configuration version.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(64)]
        public string MofBuildConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the mof build timestamp.
        /// </summary>
        public DateTime MofBuildTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Key]
        [StringLength(256)]
        public string Target { get; set; }
    }
}