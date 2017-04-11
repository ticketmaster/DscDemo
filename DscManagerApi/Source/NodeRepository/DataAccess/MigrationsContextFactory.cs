// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationsContextFactory.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataAccess
{
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// The migrations context factory.
    /// </summary>
    public class MigrationsContextFactory : IDbContextFactory<NodeRepositoryContext>
    {
        /// <summary>
        /// Gets or sets the name or connection string.
        /// </summary>
        public static string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Creates a new instance of a derived <see cref="T:System.Data.Entity.DbContext" /> type.
        /// </summary>
        /// <returns> An instance of TContext </returns>
        public NodeRepositoryContext Create()
        {
            return new NodeRepositoryContext(NameOrConnectionString);
        }
    }
}