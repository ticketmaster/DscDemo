// <copyright file="PermissionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Ticketmaster.CredentialRepository.Models;

    /// <summary>
    ///     The permission repository.
    /// </summary>
    public class PermissionRepository : Repository<Permission>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public PermissionRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="permissionEntity">
        /// The permission entity.
        /// </param>
        public override async Task<int> Insert(Permission permissionEntity)
        {
                // look for duplicates
                if (this.Search(
                    p =>
                    p.Access == permissionEntity.Access && p.Action == permissionEntity.Action
                    && p.Model == permissionEntity.Model && p.EntityGuid == permissionEntity.EntityGuid
                    && p.Identity == permissionEntity.Identity
                    && p.IdentityProvider == permissionEntity.IdentityProvider).Any())
            {
                return 0;
            }

           return await base.Insert(permissionEntity);
        }
    }
}