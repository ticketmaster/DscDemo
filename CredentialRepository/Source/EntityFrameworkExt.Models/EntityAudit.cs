// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityAudit.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The entity audit.
    /// </summary>
    [DataContract]
    [Table("EntityAudit")]
    public class EntityAudit
    {
        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        [DataMember]
        [Required]
        public AuditAction Action { get; set; }

        /// <summary>
        ///     Gets or sets the created timestamp.
        /// </summary>
        [DataMember]
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        [DataMember]
        public string Data { get; set; }

        /// <summary>
        ///     Gets or sets the entity guid.
        /// </summary>
        [DataMember]
        public Guid EntityGuid { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        [Required]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        [DataMember]
        public string Model { get; set; }
    }
}