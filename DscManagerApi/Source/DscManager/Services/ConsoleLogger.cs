// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.Interfaces.DscManager;

    /// <summary>
    /// The console logger.
    /// </summary>
    public class ConsoleLogger : DscEventHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        public ConsoleLogger(IDscEventManager eventManager)
            : base(eventManager)
        {
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
        public override Task HandleEvent(DscEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Message);
            return Task.FromResult<object>(null);
        }
    }
}