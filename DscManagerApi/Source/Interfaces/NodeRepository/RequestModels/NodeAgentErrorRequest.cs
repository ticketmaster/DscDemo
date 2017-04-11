// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeAgentErrorRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels
{
    /// <summary>
    /// The node agent error request.
    /// </summary>
    public class NodeAgentErrorRequest
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}