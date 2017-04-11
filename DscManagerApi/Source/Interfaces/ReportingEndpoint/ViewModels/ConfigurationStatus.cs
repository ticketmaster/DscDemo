// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationStatus.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels
{
    /// <summary>
    ///     The configuration status.
    /// </summary>
    public enum ConfigurationStatus
    {
        /// <summary>
        ///     The success.
        /// </summary>
        Success, 

        /// <summary>
        ///     The failure.
        /// </summary>
        Failure, 

        /// <summary>
        /// The aborted.
        /// </summary>
        Aborted
    }
}