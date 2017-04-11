// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscEventManager.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DscManager
{
    using System;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.DscManager.Services;

    /// <summary>
    /// The DscEventManager interface.
    /// </summary>
    public interface IDscEventManager
    {
        /// <summary>
        /// The create event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task CreateEvent(DscEventArgs eventArgs);

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="invokeAction">
        /// The invoke action.
        /// </param>
        void Register(Func<DscEventArgs, Task> invokeAction);

        /// <summary>
        /// The unregister.
        /// </summary>
        /// <param name="invokeAction">
        /// The invoke action.
        /// </param>
        void Unregister(Func<DscEventArgs, Task> invokeAction);
    }
}