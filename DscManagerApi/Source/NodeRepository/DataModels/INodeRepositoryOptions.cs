// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeRepositoryOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataModels
{
    using Ticketmaster.Dsc.NodeRepository.Logging;

    /// <summary>
    ///     The NodeRepositoryOptions interface.
    /// </summary>
    public interface INodeRepositoryOptions
    {
        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        INodeRepositoryLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        string NameOrConnectionString { get; set; }
    }
}