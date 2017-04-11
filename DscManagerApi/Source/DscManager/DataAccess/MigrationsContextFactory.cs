// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationsContextFactory.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataAccess
{
    using System.Data.Entity.Infrastructure;

    using Ticketmaster.Dsc.NodeRepository.DataAccess;

    /// <summary>
    /// The migrations context factory.
    /// </summary>
    public class MigrationsContextFactory : IDbContextFactory<DscManagerContext>
    {
        /// <summary>
        /// Gets or sets the name or connection string.
        /// </summary>
        public static string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Creates a new instance of a derived <see cref="T:System.Data.Entity.DbContext" /> type.
        /// </summary>
        /// <returns> An instance of TContext </returns>
        public DscManagerContext Create()
        {
            return new DscManagerContext(NameOrConnectionString);
        }
    }
}