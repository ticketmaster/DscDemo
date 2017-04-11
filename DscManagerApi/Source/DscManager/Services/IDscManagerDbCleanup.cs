// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscManagerDbCleanup.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    /// <summary>
    /// The DscManagerDbCleanup interface.
    /// </summary>
    public interface IDscManagerDbCleanup
    {
        /// <summary>
        /// The cleanup old log entries.
        /// </summary>
        void CleanupOldLogEntries();
    }
}