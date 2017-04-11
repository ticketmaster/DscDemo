// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildTarget.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer;

    /// <summary>
    ///     The build targets.
    /// </summary>
    public class BuildTarget
    {
        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        [ForeignKey("BuildId")]
        public virtual Build Build { get; set; }

        /// <summary>
        ///     Gets or sets the build id.
        /// </summary>
        [Index(IsUnique = false)]
        public int BuildId { get; set; }

        /// <summary>
        /// Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets or sets the configuration package name.
        /// </summary>
        public string ConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the configuration version.
        /// </summary>
        public string ConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the job id.
        /// </summary>
        [Index(IsUnique = false)]
        public int JobId { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [Index(IsUnique = false)]
        public BuildStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(128)]
        public string Target { get; set; }
    }
}