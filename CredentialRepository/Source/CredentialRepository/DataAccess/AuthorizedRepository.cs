// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizedRepository.cs" company="">
//   
// </copyright>
// <summary>
//   The authorized repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using Ticketmaster.CredentialRepository.Extensions;
    using Ticketmaster.CredentialRepository.Models;

    /// <summary>
    /// The authorized repository.
    /// </summary>
    /// <typeparam name="T">
    /// The model for the repository.
    /// </typeparam>
    public class AuthorizedRepository<T> : Repository<T>, IAuthorizedRepository<T>
        where T : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizedRepository{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="permissionRepository">
        /// The permission repository.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public AuthorizedRepository(DbContext context, PermissionRepository permissionRepository, IPrincipal user)
            : base(context)
        {
            this.PermissionRepository = permissionRepository;
            this.User = user;
        }

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        public IPrincipal User { get; protected set; }

        /// <summary>
        ///     Gets or sets the permission repository.
        /// </summary>
        protected PermissionRepository PermissionRepository { get; set; }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task<int> Delete(T entity)
        {
            if (!this.IsAuthorized(entity, PermissionActions.Delete))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            return await base.Delete(entity);
        }

        /// <summary>
        /// The delete permission.
        /// </summary>
        /// <param name="permission">
        /// The permission to delete.
        /// </param>
        /// <exception cref="UnauthorizedAccessException">
        /// Throws an UnauthorizedAccessException when access is denied.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> DeletePermission(Permission permission)
        {
            if (!this.IsAuthorized(permission.EntityGuid, typeof(T).Name, PermissionActions.DeletePermission))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            return await this.PermissionRepository.Delete(permission);
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
        public override T Find(Expression<Func<T, bool>> predicate)
        {
            var entity = base.Find(predicate);

            if (entity == null)
            {
                return null;
            }

            return !this.IsAuthorized(entity, PermissionActions.Read) ? null : entity;
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>T</cref>
        ///     </see>
        ///     .
        /// </returns>
        public override T Get(object id)
        {
            var entity = base.Get(id);

            if (entity == null)
            {
                return null;
            }

            return !this.IsAuthorized(entity, PermissionActions.Read) ? null : entity;
        }

        /// <summary>
        ///     The get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        public override IQueryable<T> GetAll()
        {
            return this.Search(c => true);
        }

        /// <summary>
        ///     The get all permissions.
        /// </summary>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        public virtual IQueryable<Permission> GetAllPermissions()
        {
            return !this.IsAuthorized(default(Guid), typeof(T).Name, PermissionActions.ReadPermission)
                       ? null
                       : this.PermissionRepository.Search(p => p.Model == typeof(T).Name || p.Model == null);
        }

        /// <summary>
        /// The get all permissions for record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public virtual IQueryable<Permission> GetAllPermissionsForRecord(T entity)
        {
            return this.IsAuthorized(entity, PermissionActions.ReadPermission)
                       ? this.PermissionRepository.Search(p => p.EntityGuid == entity.EntityGuid)
                       : null;
        }

        /// <summary>
        /// The get permission.
        /// </summary>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="Permission"/>.
        /// </returns>
        public virtual Permission GetPermission(int permissionId)
        {
            var permission = this.PermissionRepository.Get(permissionId);
            if (permission == null)
            {
                return null;
            }

            return !this.IsAuthorized(permission.EntityGuid, typeof(T).Name, PermissionActions.ReadPermission)
                       ? null
                       : permission;
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
        public override async Task<int> Insert(T entity)
        {
            if (!this.IsAuthorized(entity, PermissionActions.Create))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            var results = await base.Insert(entity);

            var permissionsToAdd = this.GetAuthorizingIdentities(entity, PermissionActions.Create);
            foreach (var newPermission in
                permissionsToAdd.Select(
                    identity =>
                    new Permission
                        {
                            Access = AccessControlType.Allow, 
                            Action = PermissionActions.All, 
                            Model = null, 
                            EntityGuid = entity.EntityGuid, 
                            Identity = identity.Identity, 
                            IdentityProvider = identity.IdentityProvider
                        }))
            {
                await this.PermissionRepository.Insert(newPermission);
            }

            return results;
        }

        /// <summary>
        /// The insert permission.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> InsertPermission(Permission permission)
        {
            if (!this.IsAuthorized(permission.EntityGuid, typeof(T).Name, PermissionActions.CreatePermission))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            permission.Identity = this.User.Identity.Name;
            permission.IdentityProvider = this.User.GetIdentityProvider();

            return await this.PermissionRepository.Insert(permission);
        }

        /// <summary>
        /// The is authorized.
        /// </summary>
        /// <param name="entityPermissionGuid">
        /// The entity permission guid.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsAuthorized(Guid entityPermissionGuid, string model, PermissionActions action)
        {
            if (!this.User.Identity.IsAuthenticated && this.User.Identity.Name != "Anonymous"
                && this.User.Identity.GetIdentityProvider() != "Anonymous")
            {
                // not authed
                return false;
            }

            var idp = this.User.GetIdentityProvider();

            var permissions =
                this.PermissionRepository.Search(
                    p =>
                    p.IdentityProvider == idp && (p.Model == model || p.Model == null)
                    && (p.Action == action || p.Action == PermissionActions.All)
                    && (p.EntityGuid == entityPermissionGuid || p.EntityGuid == default(Guid)));
            foreach (var denyPermission in permissions.Where(p => p.Access == AccessControlType.Deny))
            {
                if (this.User.IsInRole(denyPermission.Identity) || this.User.Identity.Name == denyPermission.Identity)
                {
                    return false;
                }
            }

            foreach (var permission in permissions.Where(p => p.Access == AccessControlType.Allow))
            {
                if (this.User.IsInRole(permission.Identity) || this.User.Identity.Name == permission.Identity)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The is authorized.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAuthorized(T entity, PermissionActions action)
        {
            return this.IsAuthorized(entity.EntityGuid, typeof(T).Name, action);
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
        public override IQueryable<T> Search(Expression<Func<T, bool>> predicate)
        {
            var permissions = this.GetAuthorizedEntities(typeof(T).Name, PermissionActions.Read);
            if (permissions.Any(p => p.EntityGuid == default(Guid) && p.Access == AccessControlType.Deny))
            {
                return null;
            }

            var idp = this.User.GetIdentityProvider();
            var allowGuids = new List<Guid>();
            var denyGuids = new List<Guid>();
            foreach (var permission in permissions)
            {
                if (permission.IdentityProvider != idp
                    || (permission.Identity != this.User.Identity.Name && !this.User.IsInRole(permission.Identity)))
                {
                    continue;
                }

                if (permission.Access == AccessControlType.Allow)
                {
                    allowGuids.Add(permission.EntityGuid);
                }
                else
                {
                    denyGuids.Add(permission.EntityGuid);
                }
            }

            return
                base.Search(predicate)
                    .Where(
                        e =>
                        !denyGuids.Contains(e.EntityGuid)
                        && (allowGuids.Contains(e.EntityGuid) || allowGuids.Contains(default(Guid))));
        }

        /// <summary>
        /// The search permissions.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public virtual IQueryable<Permission> SearchPermissions(Expression<Func<Permission, bool>> predicate)
        {
            var permissions = this.GetAuthorizedEntities(typeof(T).Name, PermissionActions.Read);
            if (permissions.Any(p => p.EntityGuid == default(Guid) && p.Access == AccessControlType.Deny))
            {
                return null;
            }

            var idp = this.User.GetIdentityProvider();
            var allowGuids = new List<Guid>();
            var denyGuids = new List<Guid>();
            foreach (var permission in permissions)
            {
                if (permission.IdentityProvider != idp
                    || (permission.Identity != this.User.Identity.Name && !this.User.IsInRole(permission.Identity)))
                {
                    continue;
                }

                if (permission.Access == AccessControlType.Allow)
                {
                    allowGuids.Add(permission.EntityGuid);
                }
                else
                {
                    denyGuids.Add(permission.EntityGuid);
                }
            }

            return
                this.PermissionRepository.Search(
                    p =>
                    !denyGuids.Contains(p.EntityGuid)
                    && (allowGuids.Contains(p.EntityGuid) || allowGuids.Contains(default(Guid)))).Where(predicate);
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
        public override async Task<int> Update(T entity)
        {
            if (!this.IsAuthorized(entity, PermissionActions.Create))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            return await base.Update(entity);
        }

        /// <summary>
        /// The update permission.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        /// <exception cref="UnauthorizedAccessException">
        /// Throws an UnauthorizedAccessException when access is denied.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<int> UpdatePermission(Permission permission)
        {
            if (!this.IsAuthorized(permission.EntityGuid, typeof(T).Name, PermissionActions.UpdatePermission))
            {
                throw new UnauthorizedAccessException("Access is denied.");
            }

            return await this.PermissionRepository.Update(permission);
        }

        /// <summary>
        /// The get authorized entities.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>ICollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected virtual IQueryable<Permission> GetAuthorizedEntities(string model, PermissionActions action)
        {
            return
                this.PermissionRepository.Search(
                    p =>
                    (p.Model == model || p.Model == null) && (p.Action == action || p.Action == PermissionActions.All));
        }

        /// <summary>
        /// The get authorized entities.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        protected IQueryable<Permission> GetAuthorizedEntities(T entity, PermissionActions action)
        {
            return this.GetAuthorizedEntities(typeof(T).Name, action);
        }

        /// <summary>
        /// The get authorizing identities.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected virtual IEnumerable<Permission> GetAuthorizingIdentities(string model, PermissionActions action)
        {
            var idp = this.User.GetIdentityProvider();

            var permissions =
                this.PermissionRepository.Search(
                    p =>
                    (p.Model == model || p.Model == null) && (p.Action == action || p.Action == PermissionActions.All)
                    && p.Access == AccessControlType.Allow && p.IdentityProvider == idp);
            var identities = new List<Permission>();
            foreach (var permission in permissions)
            {
                if (this.User.IsInRole(permission.Identity) && identities.All(p => p.Id != permission.Id))
                {
                    identities.Add(permission);
                }
            }

            return identities;
        }

        /// <summary>
        /// The get authorizing identities.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        protected IEnumerable<Permission> GetAuthorizingIdentities(T entity, PermissionActions action)
        {
            return this.GetAuthorizingIdentities(typeof(T).Name, action);
        }
    }
}