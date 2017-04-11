// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialRepositoryContext.cs" company="">
//   
// </copyright>
// <summary>
//   The credential repository context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System.Data.Entity;

    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt;
    using Ticketmaster.Dsc.EntityFrameworkExt.Services;

    /// <summary>
    /// The credential repository context.
    /// </summary>
    public class CredentialRepositoryContext : ExtendedDbContext
    {
        public CredentialRepositoryContext(string connectionString, IEncryptionService encryptionService)
            : base(connectionString)
        {
            this.SaveActions.Add(encryptionService);
            this.PostSaveActions.Add(encryptionService);
            this.ModelCreationActions.Add(encryptionService);

            this.SaveActions.Add(new EntitySaveAction());
        }

        protected IDatabaseInitializer<CredentialRepositoryContext> Initializer { get; set; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        public DbSet<Credential> Credentials { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }
    }
}