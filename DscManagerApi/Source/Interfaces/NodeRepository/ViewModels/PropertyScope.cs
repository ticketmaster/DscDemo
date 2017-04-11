// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyScope.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    /// <summary>
    ///     The property scope.
    /// </summary>
    public enum PropertyScope
    {
        /// <summary>
        ///     The global.
        /// </summary>
        Global, 

        /// <summary>
        ///     The site.
        /// </summary>
        Site, 

        /// <summary>
        ///     The configuration environment.
        /// </summary>
        ConfigurationEnvironment, 

        /// <summary>
        ///     The role.
        /// </summary>
        Role, 

        /// <summary>
        ///     The node.
        /// </summary>
        Node
    }
}