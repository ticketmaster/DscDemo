// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRepositoryContextMigrationConfiguration.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataAccess
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The reporting endpoint context migration configuration.
    /// </summary>
    public class NodeRepositoryContextMigrationConfiguration : DbMigrationsConfiguration<NodeRepositoryContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRepositoryContextMigrationConfiguration"/> class.
        /// </summary>
        public NodeRepositoryContextMigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}