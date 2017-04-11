// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptable.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     The Encrypt interface.
    /// </summary>
    public interface IEncryptable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the certificate thumbprint.
        /// </summary>
        string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        StoreLocation StoreLocation { get; set; }

        /// <summary>
        /// Gets or sets the store name.
        /// </summary>
        string StoreName { get; set; }

        #endregion
    }
}