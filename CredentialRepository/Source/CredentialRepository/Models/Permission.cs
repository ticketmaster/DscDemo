// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Security.AccessControl;

    /// <summary>
    ///     The permission.
    /// </summary>
    [DataContract]
    public class Permission
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the access.
        /// </summary>
        [DataMember]
        public AccessControlType Access { get; set; }

        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        [DataMember]
        public PermissionActions Action { get; set; }

        /// <summary>
        ///     Gets or sets the entity id.
        /// </summary>
        [Index(IsUnique = false)]
        [DataMember]
        public Guid EntityGuid { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the identity.
        /// </summary>
        [DataMember]
        public string Identity { get; set; }

        /// <summary>
        ///     Gets or sets the identity provider.
        /// </summary>
        [DataMember]
        public string IdentityProvider { get; set; }

        /// <summary>
        ///     Gets or sets the area.
        /// </summary>
        [DataMember]
        public string Model { get; set; }

        #endregion
    }
}