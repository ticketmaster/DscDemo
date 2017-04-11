// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointContextInitializer.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataAccess
{
    using System.Data.Entity;

    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The reporting endpoint context initializer.
    /// </summary>
    public class ReportingEndpointContextInitializer :
        MigrateDatabaseToLatestVersion<ReportingEndpointContext, ReportingEndpointContextMigrationConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingEndpointContextInitializer"/> class.
        /// </summary>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ReportingEndpointContextInitializer(IReportingEndpointLogging logging)
        {
            this.Logging = logging;
        }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        protected IReportingEndpointLogging Logging { get; set; }

        /// <inheritdoc />
        public override void InitializeDatabase(ReportingEndpointContext context)
        {
            if (context.Database.Exists() && !context.Database.CompatibleWithModel(false))
            {
                this.Logging.UpgradeReportingEndpointDatabaseSchema();
                this.Seed(context);
            }

            base.InitializeDatabase(context);
        }

        protected virtual void Seed(ReportingEndpointContext context)
        {
            
        }
    }
}