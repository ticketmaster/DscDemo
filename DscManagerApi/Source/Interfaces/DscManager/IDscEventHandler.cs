// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscEventHandler.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    /// <summary>
    /// The DscEventHandler interface.
    /// </summary>
    public interface IDscEventHandler
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