// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationsContextFactory.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataAccess
{
    using System.Data.Entity.Infrastructure;

    /// <summary>
    ///     The migrations context factory.
    /// </summary>
    public class MigrationsContextFactory : IDbContextFactory<DeploymentServerContext>
    {
        /// <summary>
        ///     Gets or sets the name or connection string.
        /// </summary>
        public static string NameOrConnectionString { get; set; }

        /// <summary>
        ///     Creates a new instance of a derived <see cref="T:System.Data.Entity.DbContext" /> type.
        /// </summary>
        /// <returns> An instance of TContext </returns>
        public DeploymentServerContext Create()
        {
            return new DeploymentServerContext(NameOrConnectionString);
        }
    }
}