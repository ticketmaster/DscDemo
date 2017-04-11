// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Build.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The build.
    /// </summary>
    public class Build : ModelBase<BuildView>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Build" /> class.
        /// </summary>
        public Build()
        {
            var detail = this.AddTypeMapping<BuildDetailView>();
            detail.AddTypeMapping(typeof(BuildTarget), typeof(BuildTargetView));
        }

        /// <summary>
        ///     Gets or sets the complete timestamp.
        /// </summary>
        [Index(IsUnique = false)]
        [Column(TypeName = "DateTime2")]
        public DateTime CompleteTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the request timestamp.
        /// </summary>
        [Index(IsUnique = false)]
        [Column(TypeName = "DateTime2")]
        public DateTime RequestTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [Index(IsUnique = false)]
        public BuildStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the targets.
        /// </summary>
        public virtual ICollection<BuildTarget> Targets { get; set; }

        public int SubmissionJobId { get; set; }
    }
}