// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeploymentServerContext.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataAccess
{
    using System.Data.Entity;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;

    /// <summary>
    ///     The deployment server context.
    /// </summary>
    public class DeploymentServerContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentServerContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// The name or connection string.
        /// </param>
        public DeploymentServerContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        ///     Gets or sets the builds.
        /// </summary>
        public virtual DbSet<Build> Builds { get; set; }

        /// <summary>
        ///     Gets or sets the build targets.
        /// </summary>
        public virtual DbSet<BuildTarget> BuildTargets { get; set; }

        /// <summary>
        ///     Gets or sets the configuration documents.
        /// </summary>
        public virtual DbSet<ConfigurationDocument> ConfigurationDocuments { get; set; }

        /// <summary>
        ///     Gets or sets the configurations.
        /// </summary>
        public virtual DbSet<Configuration> Configurations { get; set; }
    }
}