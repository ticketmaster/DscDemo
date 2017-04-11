// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportingEndpointContext.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataAccess
{
    using System;
    using System.Data.Entity;

    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The reporting endpoint context.
    /// </summary>
    public class ReportingEndpointContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingEndpointContext"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">
        /// The name or connection string.
        /// </param>
        public ReportingEndpointContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Gets or sets the configuration report resources.
        /// </summary>
        public DbSet<ConfigurationReportResourceEntity> ConfigurationReportResources { get; set; }

        /// <summary>
        /// Gets or sets the configuration reports.
        /// </summary>
        public DbSet<ConfigurationReport> ConfigurationReports { get; set; }

        /// <summary>
        /// Gets or sets the node status.
        /// </summary>
        public DbSet<NodeStatus> NodeStatus { get; set; }

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
        ///     More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///     classes directly.
        /// </remarks>
        /// <param name="modelBuilder">
        /// The builder that defines the model for the context being created. 
        /// </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            base.OnModelCreating(modelBuilder);
        }
    }
}