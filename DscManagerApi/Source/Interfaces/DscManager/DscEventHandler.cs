// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscEventHandler.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DscManager
{
    using System;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.DscManager.Services;

    /// <summary>
    /// The dsc event handler.
    /// </summary>
    public abstract class DscEventHandler : IDscEventHandler, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DscEventHandler"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        protected DscEventHandler(IDscEventManager eventManager)
        {
            this.EventManager = eventManager;
        }

        /// <summary>
        /// Gets or sets the event manager.
        /// </summary>
        protected IDscEventManager EventManager { get; set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// The handle event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public abstract Task HandleEvent(DscEventArgs eventArgs);

        /// <summary>
        /// The start.
        /// </summary>
        public virtual void Start()
        {
            this.EventManager.Register(this.HandleEvent);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public virtual void Stop()
        {
            this.EventManager.Unregister(this.HandleEvent);
        }
    }
}