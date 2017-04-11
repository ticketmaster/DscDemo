// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Role.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     The role.
    /// </summary>
    public class Role
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [Index("IX_DuplicateRole", 1, IsUnique = true)]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the node.
        /// </summary>
        public virtual Node Node { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        [ForeignKey("Node")]
        [Index("IX_DuplicateRole", 2, IsUnique = true)]
        public int NodeId { get; set; }
    }
}