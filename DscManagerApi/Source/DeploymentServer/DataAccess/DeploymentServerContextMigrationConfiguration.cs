// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeploymentServerContextMigrationConfiguration.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataAccess
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The reporting endpoint context migration configuration.
    /// </summary>
    public class DeploymentServerContextMigrationConfiguration : DbMigrationsConfiguration<DeploymentServerContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentServerContextMigrationConfiguration"/> class.
        /// </summary>
        public DeploymentServerContextMigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}