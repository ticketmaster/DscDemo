// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditAction.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Models
{
    /// <summary>
    ///     The audit action.
    /// </summary>
    public enum AuditAction
    {
        /// <summary>
        ///     The create.
        /// </summary>
        Create, 

        /// <summary>
        ///     The update.
        /// </summary>
        Update, 

        /// <summary>
        ///     The delete.
        /// </summary>
        Delete, 

        /// <summary>
        ///     The restore.
        /// </summary>
        Restore, 

        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown
    }
}