// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscManagerContextMigrationConfiguration.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataAccess
{
    using System.Data.Entity.Migrations;

    using Ticketmaster.Dsc.NodeRepository.DataAccess;

    /// <summary>
    /// The reporting endpoint context migration configuration.
    /// </summary>
    public class DscManagerContextMigrationConfiguration : DbMigrationsConfiguration<DscManagerContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DscManagerContextMigrationConfiguration"/> class.
        /// </summary>
        public DscManagerContextMigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
        }
    }
}