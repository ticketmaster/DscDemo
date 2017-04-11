// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyType.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    /// <summary>
    ///     The property type.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        ///     The node.
        /// </summary>
        Node, 

        /// <summary>
        ///     The local agent.
        /// </summary>
        LocalAgent, 

        /// <summary>
        ///     The bootstrap.
        /// </summary>
        Bootstrap, 

        /// <summary>
        /// The resource version.
        /// </summary>
        ResourceVersion
    }
}