// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelCreationAction.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System.Data.Entity.Infrastructure;

    /// <summary>
    ///     The ModelCreationAction interface.
    /// </summary>
    public interface IModelCreationAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// The process model upon creation.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        void ProcessModelUponCreation(DbEntityEntry entry);

        #endregion
    }
}