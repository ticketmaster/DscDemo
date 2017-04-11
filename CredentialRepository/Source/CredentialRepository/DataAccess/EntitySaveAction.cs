// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySaveAction.cs" company="">
//   
// </copyright>
// <summary>
//   The entity save action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;

    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt;

    /// <summary>
    /// The entity save action.
    /// </summary>
    public class EntitySaveAction : ISaveAction
    {
        /// <summary>
        /// The process entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void ProcessEntity(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEntity;
            if (entity != null && entity.EntityGuid == default(Guid))
            {
                entity.EntityGuid = Guid.NewGuid();
            }
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
            var entity = entry.Entity as IEntity;
            if (entity != null && entity.EntityGuid == default(Guid))
            {
                entity.EntityGuid = Guid.NewGuid();
            }

            return Task.FromResult<object>(null);
        }
    }
}