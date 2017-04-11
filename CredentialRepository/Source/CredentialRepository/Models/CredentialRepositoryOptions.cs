// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialRepositoryOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The credential repository options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ticketmaster.CredentialRepository.Models
{
    using System.Data.Entity;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Http;

    using Ticketmaster.CredentialRepository.DataAccess;

    /// <summary>
    ///     The credential repository options.
    /// </summary>
    public class CredentialRepositoryOptions : ICredentialRepositoryOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialRepositoryOptions"/> class.
        /// </summary>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <param name="storeLocation">
        /// The store location.
        /// </param>
        /// <param name="storeName">
        /// The store name.
        /// </param>
        /// <param name="nameOrConnectionString">
        /// The name Or Connection String.
        /// </param>
        public CredentialRepositoryOptions(
            string certificateThumbprint, 
            StoreLocation storeLocation, 
            string storeName, 
            string nameOrConnectionString)
        {
            this.CertificateThumbprint = certificateThumbprint;
            this.StoreLocation = storeLocation;
            this.StoreName = storeName;
            this.NameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        ///     Gets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the database initializer.
        /// </summary>
        public IDatabaseInitializer<CredentialRepositoryContext> DatabaseInitializer { get; set; } = new CreateDatabaseIfNotExists<CredentialRepositoryContext>();

        /// <summary>
        ///     Gets or sets the include error detail policy.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public IncludeErrorDetailPolicy IncludeErrorDetailPolicy { get; set; } = IncludeErrorDetailPolicy.LocalOnly;

        /// <summary>
        /// Gets or sets the name or connection string.
        /// </summary>
        public string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Gets the store location.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StoreLocation StoreLocation { get; set; }

        /// <summary>
        ///     Gets the store name.
        /// </summary>
        public string StoreName { get; set; }
    }
}