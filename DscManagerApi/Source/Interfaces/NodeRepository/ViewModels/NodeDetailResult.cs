// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeDetailResult.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;

    /// <summary>
    ///     The node detail result.
    /// </summary>
    public class NodeDetailResult
    {
        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        public BuildView Build { get; set; }

        /// <summary>
        ///     Gets or sets the node view.
        /// </summary>
        public NodeDetailView NodeView { get; set; }
    }
}