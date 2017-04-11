// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionActions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    /// <summary>
    /// The permission actions.
    /// </summary>
    public enum PermissionActions
    {
        /// <summary>
        /// The all.
        /// </summary>
        All, 

        /// <summary>
        /// The create.
        /// </summary>
        Create, 

        /// <summary>
        /// The read.
        /// </summary>
        Read, 

        /// <summary>
        /// The update.
        /// </summary>
        Update, 

        /// <summary>
        /// The delete.
        /// </summary>
        Delete, 

        /// <summary>
        /// The revert.
        /// </summary>
        Restore, 

        /// <summary>
        /// The build.
        /// </summary>
        Build, 

        /// <summary>
        /// The create permission.
        /// </summary>
        CreatePermission, 

        /// <summary>
        /// The read permission.
        /// </summary>
        ReadPermission, 

        /// <summary>
        /// The update permission.
        /// </summary>
        UpdatePermission, 

        /// <summary>
        /// The delete permission.
        /// </summary>
        DeletePermission
    }
}