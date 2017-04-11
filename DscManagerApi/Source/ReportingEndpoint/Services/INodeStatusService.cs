// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeStatusService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Services
{
    /// <summary>
    /// The NodeStatusService interface.
    /// </summary>
    public interface INodeStatusService
    {
        /// <summary>
        /// The start.
        /// </summary>
        void Start();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();
    }
}