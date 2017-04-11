// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildTargetView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     The build target view.
    /// </summary>
    public class BuildTargetView
    {
        /// <summary>
        ///     Gets or sets the certificate thumbprint.
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
        ///     Gets or sets the status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public string Target { get; set; }
    }
}