// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointContextMigrationConfiguration.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataAccess
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The reporting endpoint context migration configuration.
    /// </summary>
    public class ReportingEndpointContextMigrationConfiguration : DbMigrationsConfiguration<ReportingEndpointContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingEndpointContextMigrationConfiguration"/> class.
        /// </summary>
        public ReportingEndpointContextMigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}