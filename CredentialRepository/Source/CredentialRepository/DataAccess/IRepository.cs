// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// The Repository interface.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type representing the model.
    /// </typeparam>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        ///     Gets or sets the auto save.
        /// </summary>
        bool AutoSave { get; set; }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Attach(TEntity entity);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Delete(TEntity entity);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Get(object key);

        /// <summary>
        ///     The get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Insert(TEntity entity);

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Insert(IEnumerable<TEntity> entities);

        /// <summary>
        ///     The save changes.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<int> SaveChanges();

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        IQueryable<TEntity> Search(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Update(TEntity entity);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> Update(IEnumerable<TEntity> entities);
    }
}