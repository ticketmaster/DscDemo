// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionServiceOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     The EncryptionServiceOptions interface.
    /// </summary>
    public interface IEncryptionServiceOptions
    {
        #region Public Properties

        /// <summary>
        ///     Gets the certificate thumbprint.
        /// </summary>
        string CertificateThumbprint { get; }

        /// <summary>
        /// Gets the store location.
        /// </summary>
        StoreLocation StoreLocation { get; }

        /// <summary>
        /// Gets the store name.
        /// </summary>
        string StoreName { get; }

        #endregion
    }
}