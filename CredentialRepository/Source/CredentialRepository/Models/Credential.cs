// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Credential.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.EntityFrameworkExt;
    using Ticketmaster.Dsc.EntityFrameworkExt.Attributes;

    /// <summary>
    ///     The credential.
    /// </summary>
    public class Credential : IEntity, IEncryptable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StoreLocation StoreLocation { get; set; }

        /// <summary>
        /// Gets or sets the store name.
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted key.
        /// </summary>
        [Required]
        [Encrypt(IsBase64String = true)]
        public string EncryptedKey { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted password.
        /// </summary>
        public string EncryptedPassword { get; set; }

        /// <summary>
        ///     Gets or sets the entity guid.
        /// </summary>
        public Guid EntityGuid { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [StringLength(450)]
        [Index(IsUnique = true)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        [StringLength(450)]
        [Index(IsUnique = false)]
        [Required]
        public string Username { get; set; }

        #endregion
    }
}