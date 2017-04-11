// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRepositoryContext.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataAccess
{
    using System.Data.Entity;

    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.Scheduling;

    /// <summary>
    ///     The deployment server context.
    /// </summary>
    public class NodeRepositoryContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRepositoryContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// The name or connection string.
        /// </param>
        public NodeRepositoryContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        ///     Gets or sets the configuration properties.
        /// </summary>
        public DbSet<ConfigurationProperty> ConfigurationProperties { get; set; }

        /// <summary>
        ///     Gets or sets the nodes.
        /// </summary>
        public DbSet<Node> Nodes { get; set; }

        /// <summary>
        ///     Gets or sets the roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///     before the model has been locked down and used to initialize the context.  The default
        ///     implementation of this method does nothing, but it can be overridden in a derived class
        ///     such that the model can be further configured before it is locked down.
        /// </summary>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        ///     is created.  The model for that context is then cached and is for all further instances of
        ///     the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///     property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///     More control over caching is provided through use of the DbModelBuilder and NodeRepositoryContextFactory
        ///     classes directly.
        /// </remarks>
        /// <param name="modelBuilder">
        /// The builder that defines the model for the context being created.
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ComplexType<MaintenanceSchedule>().Ignore(s => s.ScheduleEntries).Ignore(s => s.TimeZone);
            base.OnModelCreating(modelBuilder);
        }
    }
}