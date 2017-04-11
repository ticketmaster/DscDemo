// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.RequestModels
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The build request.
    /// </summary>
    public class BuildRequest
    {
        /// <summary>
        ///     Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets or sets the configuration data.
        /// </summary>
        [Required]
        public Hashtable ConfigurationData { get; set; }

        /// <summary>
        /// Gets or sets the configuration package name.
        /// </summary>
        public string ConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the configuration version.
        /// </summary>
        public string ConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the number of jobs.
        /// </summary>
        public int NumberOfJobs { get; set; } = 1;
    }
}