// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.EntityFrameworkExt.Models;

    /// <summary>
    ///     The audit service.
    /// </summary>
    public class AuditService : IAuditService
    {
        #region Fields

        /// <summary>
        /// The entity action map.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, AuditAction> entityActionMap =
            new ConcurrentDictionary<Guid, AuditAction>();

        /// <summary>
        ///     The serializer.
        /// </summary>
        private JsonSerializer serializer = new JsonSerializer { ContractResolver = new AuditContractResolver() };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditService"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public AuditService(ExtendedDbContext context)
        {
            this.Context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected ExtendedDbContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the serializer.
        /// </summary>
        protected JsonSerializer Serializer
        {
            get
            {
                return this.serializer;
            }

            set
            {
                this.serializer = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The process entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void ProcessEntity(DbEntityEntry entry)
        {
            this.ProcessEntityImpl(entry);
        }

        /// <summary>
        /// The process entity async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task ProcessEntityAsync(DbEntityEntry entry)
        {
            this.ProcessEntityImpl(entry);
            return null;
        }

        /// <summary>
        /// The process entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public virtual void ProcessEntityPostSave(DbEntityEntry entry)
        {
            this.ProcessEntityPostSaveImpl(entry);
        }

        /// <summary>
        /// The process entity async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task ProcessEntityPostSaveAsync(DbEntityEntry entry)
        {
            this.ProcessEntityPostSaveImpl(entry);
            return null;
        }

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
        public virtual T Restore<T>(EntityAudit auditRecord) where T : class, IAuditable
        {
            var currentEntity = this.Context.Set<T>().FirstOrDefault(e => e.EntityGuid == auditRecord.EntityGuid);

            if (currentEntity == null)
            {
                throw new InvalidDataException("The entity for this audit record cannot be found.");
            }

            EntityAudit record;
            using (var writer = new StringWriter())
            {
                this.Serializer.Serialize(writer, currentEntity);
                record = this.CreateAuditRecord(AuditAction.Restore, writer.ToString(), currentEntity);
            }

            var currentChangeGuid = currentEntity.ChangeGuid;

            var restoredEntity = JsonConvert.DeserializeObject<T>(
                auditRecord.Data, 
                new JsonEntityConverter<T>(currentEntity));

            if (currentChangeGuid == restoredEntity.ChangeGuid)
            {
                return currentEntity;
            }

            this.Context.Entry(restoredEntity).State = EntityState.Modified;

            this.Context.EntityAudit.Add(record);

            this.Context.SaveChangesWithoutProcessing();
            return restoredEntity;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create audit record.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="recordData">
        /// The record data.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EntityAudit"/>.
        /// </returns>
        protected virtual EntityAudit CreateAuditRecord(AuditAction action, string recordData, IAuditable entity)
        {
            return new EntityAudit
                       {
                           Action = action, 
                           Created = DateTime.UtcNow, 
                           Data = recordData, 
                           EntityGuid = entity.EntityGuid, 
                           Model = entity.GetType().Name
                       };
        }

        /// <summary>
        /// The process entity implementation.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        protected virtual void ProcessEntityPostSaveImpl(DbEntityEntry entry)
        {
            var entity = entry.Entity as IAuditable;

            if (entity == null)
            {
                return;
            }

            using (var writer = new StringWriter())
            {
                this.Serializer.Serialize(writer, entity);

                AuditAction actionValue;
                if (!this.entityActionMap.TryRemove(entity.EntityGuid, out actionValue))
                {
                    actionValue = AuditAction.Unknown;
                }

                var record = this.CreateAuditRecord(actionValue, writer.ToString(), entity);
                this.Context.EntityAudit.Add(record);
                this.Context.SaveChangesWithoutProcessing();
            }
        }

        /// <summary>
        /// The process entity implementation.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        private void ProcessEntityImpl(DbEntityEntry entry)
        {
            var entity = entry.Entity as IAuditable;

            if (entity == null)
            {
                return;
            }

            if (entity.EntityGuid == default(Guid))
            {
                entity.EntityGuid = Guid.NewGuid();
            }

            entity.ChangeGuid = Guid.NewGuid();

            if (entry.State.HasFlag(EntityState.Added))
            {
                this.entityActionMap.AddOrUpdate(
                    entity.EntityGuid, 
                    AuditAction.Create, 
                    (guid, action) => AuditAction.Create);
            }
            else if (entry.State.HasFlag(EntityState.Deleted))
            {
                this.entityActionMap.AddOrUpdate(
                    entity.EntityGuid, 
                    AuditAction.Delete, 
                    (guid, action) => AuditAction.Delete);
            }
            else if (entry.State.HasFlag(EntityState.Modified))
            {
                this.entityActionMap.AddOrUpdate(
                    entity.EntityGuid, 
                    AuditAction.Update, 
                    (guid, action) => AuditAction.Update);
            }
        }

        #endregion
    }
}