// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The node request.
    /// </summary>
    public class NodeRequest
    {
        /// <summary>
        ///     Gets or sets a value indicating whether rebuild mof.
        /// </summary>
        public bool BuildMof { get; set; } = true;

        /// <summary>
        ///     Gets or sets the node data.
        /// </summary>
        [Required]
        public Hashtable NodeData { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        [Required]
        public string NodeName { get; set; }
    }
}