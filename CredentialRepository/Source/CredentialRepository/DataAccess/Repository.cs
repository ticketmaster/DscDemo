// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Repository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        ///     The semaphore used to synchronize access in async methods.
        /// </summary>
        protected readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        /// <summary>
        ///     The Context.
        /// </summary>
        private readonly DbContext db;

        /// <summary>
        ///     The EntitySet.
        /// </summary>
        private readonly DbSet<T> set;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public Repository(DbContext context)
        {
            this.db = context;
            this.set = context.Set<T>();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether auto save.
        /// </summary>
        public bool AutoSave { get; set; } = true;

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Attach(T entity)
        {
            //this.set.Attach(entity);
            //this.db.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        ///     Create an instance of the model for this repository.
        /// </summary>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T Create()
        {
            return this.set.Create<T>();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Delete(T entity)
        {
            this.set.Remove(entity);
            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Delete(IEnumerable<T> entities)
        {
            this.set.RemoveRange(entities);
            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            this.db.Dispose();
        }

        /// <summary>
        /// The find.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return this.set.FirstOrDefault(predicate);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T Get(object key)
        {
            return this.set.Find(key);
        }

        /// <summary>
        ///     The get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        public virtual IQueryable<T> GetAll()
        {
            return this.set.AsQueryable();
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Insert(T entity)
        {
            this.set.Add(entity);
            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Insert(IEnumerable<T> entities)
        {
            this.set.AddRange(entities);
            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        ///     The save changes.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<int> SaveChanges()
        {
            await this.Semaphore.WaitAsync();

            // ReSharper disable once RedundantAssignment
            var changes = -1;
            try
            {
                changes = await this.db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
            finally
            {
                this.Semaphore.Release();
            }

            return changes;
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public virtual IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            return this.set.Where(predicate).ToList().AsQueryable();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Update(T entity)
        {
            this.db.Entry(entity).State = EntityState.Modified;
            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.db.Entry(entity).State = EntityState.Modified;
            }

            if (this.AutoSave)
            {
                return await this.SaveChanges();
            }

            return await Task.FromResult(0);
        }
    }
}