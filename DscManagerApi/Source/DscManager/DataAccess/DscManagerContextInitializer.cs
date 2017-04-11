// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscManagerContextInitializer.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataAccess
{
    using System.Data.Entity;

    using Ticketmaster.Dsc.DscManager.Logging;

    /// <summary>
    ///     The default initializer.
    /// </summary>
    public class DscManagerContextInitializer :
        MigrateDatabaseToLatestVersion<DscManagerContext, DscManagerContextMigrationConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DscManagerContextInitializer"/> class.
        /// </summary>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public DscManagerContextInitializer(IDscManagerLogging logging)
        {
            this.Logging = logging;
        }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected IDscManagerLogging Logging { get; set; }

        /// <summary>
        /// The initialize database.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void InitializeDatabase(DscManagerContext context)
        {
            try
            {
                if (context.Database.Exists() && !context.Database.CompatibleWithModel(true))
                {
                    this.Logging.UpgradeDscManagerDatabaseSchema();
                    this.Seed(context);
                }
            }

            catch
            {
                base.InitializeDatabase(context);
                this.Seed(context);
            }

            base.InitializeDatabase(context);
        }

        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        ///     The default implementation does nothing.
        /// </summary>
        /// <param name="context">
        /// The context to seed.
        /// </param>
        protected void Seed(DscManagerContext context)
        {
            this.Logging?.BeginDscManagerDatabaseSeed(this.GetType().Name);

            this.Logging?.EndDscManagerDatabaseSeed(this.GetType().Name);
        }
    }
}