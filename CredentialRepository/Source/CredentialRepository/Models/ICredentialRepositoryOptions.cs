// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICredentialRepositoryOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The CredentialRepositoryOptions interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    using System.Data.Entity;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Http;

    using Ticketmaster.CredentialRepository.DataAccess;

    /// <summary>
    ///     The CredentialRepositoryOptions interface.
    /// </summary>
    public interface ICredentialRepositoryOptions
    {
        /// <summary>
        ///     Gets the certificate thumbprint.
        /// </summary>
        string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the database initializer.
        /// </summary>
        IDatabaseInitializer<CredentialRepositoryContext> DatabaseInitializer { get; set; }

        /// <summary>
        ///     Gets or sets the include error detail policy.
        /// </summary>
        IncludeErrorDetailPolicy IncludeErrorDetailPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Gets the store location.
        /// </summary>
        StoreLocation StoreLocation { get; set; }

        /// <summary>
        ///     Gets the store name.
        /// </summary>
        string StoreName { get; set; }
    }
}