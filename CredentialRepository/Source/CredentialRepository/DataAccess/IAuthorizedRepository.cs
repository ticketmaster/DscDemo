// <copyright file="IAuthorizedRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ticketmaster.CredentialRepository.DataAccess
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Ticketmaster.CredentialRepository.Models;

    /// <summary>
    /// The AuthorizedRepository interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type for the model of this repository.
    /// </typeparam>
    public interface IAuthorizedRepository<T> : IRepository<T>
        where T : class, IEntity
    {
        /// <summary>
        /// The delete permission.
        /// </summary>
        /// <param name="permission">
        /// The permission to delete.
        /// </param>
        Task<int> DeletePermission(Permission permission);

        /// <summary>
        ///     The get all permissions.
        /// </summary>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        IQueryable<Permission> GetAllPermissions();

        /// <summary>
        /// The get all permissions for record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        IQueryable<Permission> GetAllPermissionsForRecord(T entity);

        /// <summary>
        /// The get permission.
        /// </summary>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="Permission"/>.
        /// </returns>
        Permission GetPermission(int permissionId);

        /// <summary>
        /// The insert permission.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        Task<int> InsertPermission(Permission permission);

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
        bool IsAuthorized(T entity, PermissionActions action);

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
        bool IsAuthorized(Guid entityPermissionGuid, string model, PermissionActions action);

        /// <summary>
        /// The search permissions.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        IQueryable<Permission> SearchPermissions(Expression<Func<Permission, bool>> predicate);

        /// <summary>
        /// The update permission.
        /// </summary>
        /// <param name="permission">
        /// The permission.
        /// </param>
        Task<int> UpdatePermission(Permission permission);
    }
}