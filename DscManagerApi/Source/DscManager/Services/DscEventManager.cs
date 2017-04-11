// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscEventManager.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.Interfaces.DscManager;

    /// <summary>
    /// The dsc event manager.
    /// </summary>
    public class DscEventManager : IDscEventManager
    {
        /// <summary>
        /// Gets or sets the subscribers.
        /// </summary>
        protected ICollection<Func<DscEventArgs, Task>> Subscribers { get; set; } = new List<Func<DscEventArgs, Task>>()
            ;

        /// <summary>
        /// The create event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CreateEvent(DscEventArgs eventArgs)
        {
            await Task.WhenAll(this.Subscribers.Select(a => a.Invoke(eventArgs)));
        }

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="invokeAction">
        /// The invoke action.
        /// </param>
        public void Register(Func<DscEventArgs, Task> invokeAction)
        {
            this.Subscribers.Add(invokeAction);
        }

        /// <summary>
        /// The unregister.
        /// </summary>
        /// <param name="invokeAction">
        /// The invoke action.
        /// </param>
        public void Unregister(Func<DscEventArgs, Task> invokeAction)
        {
            this.Subscribers.Remove(invokeAction);
        }
    }
}