// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntity.cs" company="">
//   
// </copyright>
// <summary>
//   The Entity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;

    /// <summary>
    /// The Entity interface.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        ///     Gets or sets the entity guid.
        /// </summary>
        Guid EntityGuid { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }
    }
}