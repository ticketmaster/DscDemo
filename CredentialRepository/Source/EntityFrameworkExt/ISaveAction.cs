// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveAction.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;

    /// <summary>
    /// The SaveAction interface.
    /// </summary>
    public interface ISaveAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// The process entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        void ProcessEntity(DbEntityEntry entry);

        /// <summary>
        /// The process entity async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ProcessEntityAsync(DbEntityEntry entry);

        #endregion
    }
}