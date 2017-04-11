// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.RequestModels
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;

    /// <summary>
    ///     The configuration request.
    /// </summary>
    public class ConfigurationRequest
    {
        /// <summary>
        ///     Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets or sets the document.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        ///     The generate checksum.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string GenerateChecksum()
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(this.Document ?? string.Empty)))
                {
                    var sha = new SHA256Managed();
                    var checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", string.Empty);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     The to model.
        /// </summary>
        /// <returns>
        ///     The <see cref="Configuration" />.
        /// </returns>
        public Configuration ToModel()
        {
            return new Configuration
                       {
                           ConfigurationDocument = new ConfigurationDocument { Document = this.Document }, 
                           Checksum = this.GenerateChecksum(), 
                           PublishedTimestamp = DateTime.UtcNow, 
                           Target = this.Target,
                           CertificateThumbprint = this.CertificateThumbprint
                       };
        }
    }
}