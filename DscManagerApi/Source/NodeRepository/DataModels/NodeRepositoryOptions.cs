// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRepositoryOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataModels
{
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The node repository options.
    /// </summary>
    public class NodeRepositoryOptions : INodeRepositoryOptions
    {
        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        public INodeRepositoryLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        public string NameOrConnectionString { get; set; } = "NodeRepositoryContext";
    }
}