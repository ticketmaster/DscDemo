// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionServiceOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     The encryption service options.
    /// </summary>
    public class EncryptionServiceOptions : IEncryptionServiceOptions
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionServiceOptions"/> class.
        /// </summary>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        public EncryptionServiceOptions(string certificateThumbprint, string storeName, StoreLocation storeLocation)
        {
            this.CertificateThumbprint = certificateThumbprint;
            this.StoreName = storeName;
            this.StoreLocation = storeLocation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets the store location.
        /// </summary>
        public StoreLocation StoreLocation { get; }

        /// <summary>
        ///     Gets the store name.
        /// </summary>
        public string StoreName { get; }

        #endregion
    }
}