// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedDbContext{T}.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System.Data.Entity;

    using Ticketmaster.Dsc.EntityFrameworkExt.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt.Services;

    /// <summary>
    /// The extended context.
    /// </summary>
    /// <typeparam name="T">
    /// The model for the EntityAudit object.
    /// </typeparam>
    public abstract class ExtendedDbContext<T> : ExtendedDbContext
        where T : EntityAudit
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDbContext{T}"/> class.
        /// </summary>
        /// <param name="contextName">
        /// The context name.
        /// </param>
        protected ExtendedDbContext(string contextName)
            : base(contextName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDbContext{T}"/> class.
        /// </summary>
        /// <param name="contextName">
        /// The context name.
        /// </param>
        /// <param name="auditService">
        /// The audit service.
        /// </param>
        /// <param name="encryptionService">
        /// The encryption service.
        /// </param>
        protected ExtendedDbContext(
            string contextName, 
            IAuditService auditService, 
            IEncryptionService encryptionService)
            : base(contextName, auditService, encryptionService)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the entity audit.
        /// </summary>
        public new DbSet<T> EntityAudit { get; set; }

        #endregion
    }
}