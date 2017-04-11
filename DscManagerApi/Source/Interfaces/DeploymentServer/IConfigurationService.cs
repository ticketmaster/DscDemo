// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer
{
    /// <summary>
    ///     The ConfigurationService interface.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        ///     The cleanup archives.
        /// </summary>
        void CleanupArchives();

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="configurationDocument">
        /// The configuration document.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        void Publish(string configurationDocument, string target, string certificateThumbprint);
    }
}