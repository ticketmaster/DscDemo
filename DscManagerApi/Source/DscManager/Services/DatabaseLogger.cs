// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseLogger.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;

    using Ticketmaster.Dsc.DscManager.DataAccess;
    using Ticketmaster.Dsc.DscManager.DataModels;
    using Ticketmaster.Dsc.DscManager.Logging;
    using Ticketmaster.Dsc.Interfaces.DscManager;

    /// <summary>
    ///     The database logger.
    /// </summary>
    public class DatabaseLogger : DscEventHandler, IDscManagerDbCleanup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseLogger"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public DatabaseLogger(
            IDscEventManager eventManager, 
            DscManagerContext context, 
            ILifetimeScope container, 
            IDscManagerOptions options, 
            IDscManagerLogging logging)
            : base(eventManager)
        {
            this.Context = context;
            this.LoggingRepository = context.Set<LoggingEntity>();
            this.Container = container;
            this.Options = options;
            this.Logging = logging;
        }

        /// <summary>
        ///     Gets or sets the container.
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        ///     Gets or sets the logging repository.
        /// </summary>
        public DbSet<LoggingEntity> LoggingRepository { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DscManagerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected IDscManagerLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the log queue.
        /// </summary>
        protected Queue<LoggingEntity> LogQueue { get; set; } = new Queue<LoggingEntity>();

        /// <summary>
        ///     Gets or sets the options.
        /// </summary>
        protected IDscManagerOptions Options { get; set; }

        /// <summary>
        ///     Gets or sets the timer.
        /// </summary>
        protected Timer Timer { get; set; }

        /// <summary>
        ///     The cleanup old log entries.
        /// </summary>
        public void CleanupOldLogEntries()
        {
            var timeToRemove = DateTime.UtcNow.AddDays(this.Options.DaysToKeepLogs * -1);
            var toRemove = this.LoggingRepository.Where(l => l.Timestamp < timeToRemove).ToList();
            this.LoggingRepository.RemoveRange(toRemove);
            this.Context.SaveChanges();
            this.Logging.CleanupOldLogEntries(toRemove.Count(), this.Options.DaysToKeepLogs);
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
            var user = this.Container.IsRegistered<IPrincipal>()
                           ? this.Container.Resolve<IPrincipal>().Identity.Name
                           : "Unknown user";
            this.LogQueue.Enqueue(
                new LoggingEntity { Message = eventArgs.Message, Timestamp = DateTime.UtcNow, Username = user, Server = Environment.MachineName });
            return Task.FromResult<object>(null);
        }

        /// <summary>
        ///     The start.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.Timer = new Timer(this.TimerCallback, null, 0, 15000);
        }

        /// <summary>
        ///     The stop.
        /// </summary>
        public override void Stop()
        {
            this.Timer.Dispose();
            this.Timer = null;
            base.Stop();
        }

        /// <summary>
        /// The timer callback.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void TimerCallback(object state)
        {
            if (!this.LogQueue.Any())
            {
                return;
            }

            do
            {
                var entity = this.LogQueue.Dequeue();
                this.LoggingRepository.Add(entity);
            }
            while (this.LogQueue.Any());

            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    this.Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    var entry = ex.Entries.Single();
                    var val = entry.GetDatabaseValues();
                    if (val != null)
                    {
                        entry.OriginalValues.SetValues(val);
                    }
                }
            }
            while (saveFailed);
        }
    }
}