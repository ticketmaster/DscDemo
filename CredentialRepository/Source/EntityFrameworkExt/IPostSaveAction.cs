// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPostSaveAction.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;

    /// <summary>
    /// The PostSaveAction interface.
    /// </summary>
    public interface IPostSaveAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// The process entity post save.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        void ProcessEntityPostSave(DbEntityEntry entry);

        /// <summary>
        /// The process entity post save async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ProcessEntityPostSaveAsync(DbEntityEntry entry);

        #endregion
    }
}