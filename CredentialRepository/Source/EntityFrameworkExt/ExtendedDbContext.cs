// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedDbContext.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.EntityFrameworkExt.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt.Services;

    /// <summary>
    ///     The extended context.
    /// </summary>
    public abstract class ExtendedDbContext : DbContext
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDbContext"/> class.
        /// </summary>
        /// <param name="contextName">
        /// The context name.
        /// </param>
        protected ExtendedDbContext(string contextName)
            : base(contextName)
        {
            this.SaveActions = new List<ISaveAction>();
            this.PostSaveActions = new List<IPostSaveAction>();
            this.ModelCreationActions = new List<IModelCreationAction>();
            var context = ((IObjectContextAdapter)this).ObjectContext;
            if (!context.DatabaseExists())
            {
                context.CreateDatabase();    
            }

            context.ObjectMaterialized += this.ObjectMaterialized;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedDbContext"/> class.
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
            : base(contextName)
        {
            // Pre-save, Encryption should be first, post-save encryption should be last
            this.SaveActions.Add(encryptionService);

            this.SaveActions.Add(auditService);
            this.PostSaveActions.Add(auditService);

            this.PostSaveActions.Add(encryptionService);
            this.ModelCreationActions.Add(encryptionService);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the entity audit.
        /// </summary>
        public virtual DbSet<EntityAudit> EntityAudit { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the encryption service.
        /// </summary>
        protected IEncryptionService EncryptionService { get; set; }

        /// <summary>
        ///     Gets or sets the model creation actions.
        /// </summary>
        protected ICollection<IModelCreationAction> ModelCreationActions { get; set; }

        /// <summary>
        ///     Gets or sets the post save actions.
        /// </summary>
        protected ICollection<IPostSaveAction> PostSaveActions { get; set; }

        /// <summary>
        ///     Gets or sets the save actions.
        /// </summary>
        protected ICollection<ISaveAction> SaveActions { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The register action.
        /// </summary>
        /// <param name="saveAction">
        /// The save action.
        /// </param>
        public void RegisterAction(ISaveAction saveAction)
        {
            this.SaveActions.Add(saveAction);
        }

        /// <summary>
        /// The register action.
        /// </summary>
        /// <param name="postSaveAction">
        /// The post save action.
        /// </param>
        public void RegisterAction(IPostSaveAction postSaveAction)
        {
            this.PostSaveActions.Add(postSaveAction);
        }

        /// <summary>
        /// The register action.
        /// </summary>
        /// <param name="modelCreationAction">
        /// The model creation action.
        /// </param>
        public void RegisterAction(IModelCreationAction modelCreationAction)
        {
            this.ModelCreationActions.Add(modelCreationAction);
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the underlying database. This can include
        ///     state entries for entities and/or relationships. Relationship state entries are created for
        ///     many-to-many relationships and relationships where there is no foreign key property
        ///     included in the entity class (often referred to as independent associations).
        /// </returns>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the
        ///     database.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///     concurrency violation; that is, a row has been changed in the database since it was queried.
        /// </exception>
        /// <exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///     on the same context instance.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either before or after sending commands
        ///     to the database.
        /// </exception>
        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();

            var changedEntities =
                this.ChangeTracker.Entries()
                    .Where(e => e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            var entityEntries = changedEntities as DbEntityEntry[] ?? changedEntities.ToArray();
            foreach (var entity in entityEntries)
            {
                foreach (var action in this.SaveActions)
                {
                    action.ProcessEntity(entity);
                }
            }

            var saveResult = base.SaveChanges();

            foreach (var entity in entityEntries)
            {
                foreach (var action in this.PostSaveActions)
                {
                    action.ProcessEntityPostSave(entity);
                }
            }

            return saveResult;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="cancellationToken">
        /// A <see cref="T:System.Threading.CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        ///     The task result contains the number of state entries written to the underlying database. This can include
        ///     state entries for entities and/or relationships. Relationship state entries are created for
        ///     many-to-many relationships and relationships where there is no foreign key property
        ///     included in the entity class (often referred to as independent associations).
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// Thrown if the context has been disposed.
        /// </exception>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.ChangeTracker.DetectChanges();

            var changedEntities =
                this.ChangeTracker.Entries()
                    .Where(e => e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            var entityEntries = changedEntities as DbEntityEntry[] ?? changedEntities.ToArray();
            var tasks =
                (from entity in entityEntries from action in this.SaveActions select action.ProcessEntityAsync(entity))
                    .ToArray();

            try
            {
                Task.WaitAll(tasks, cancellationToken);
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            var postTasks =
                (from entity in entityEntries
                 from action in this.PostSaveActions
                 select action.ProcessEntityPostSaveAsync(entity)).ToArray();
            Task.WaitAll(postTasks, cancellationToken);

            return result;
        }

        /// <summary>
        /// The save changes without processing.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveChangesWithoutProcessing()
        {
            return base.SaveChanges();
        }

        /// <summary>
        /// The save changes without processing async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<int> SaveChangesWithoutProcessingAsync()
        {
            return await this.SaveChangesAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The object materialized.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            foreach (var action in this.ModelCreationActions)
            {
                action.ProcessModelUponCreation(this.Entry(e.Entity));
            }
        }

        #endregion
    }
}