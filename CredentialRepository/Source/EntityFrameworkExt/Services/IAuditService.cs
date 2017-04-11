// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuditService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using Ticketmaster.Dsc.EntityFrameworkExt.Models;

    /// <summary>
    ///     The AuditService interface.
    /// </summary>
    public interface IAuditService : ISaveAction, IPostSaveAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// The restore.
        /// </summary>
        /// <param name="auditRecord">
        /// The audit record.
        /// </param>
        /// <typeparam name="T">
        /// The model that the audit record represents.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Restore<T>(EntityAudit auditRecord) where T : class, IAuditable;

        #endregion
    }
}