// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppBuilderExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Extensions
{
    using System.Data.Entity;

    using Autofac;

    using Hangfire;

    using Owin;

    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.Services;

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The i app builder extensions.
    /// </summary>
    public static class IAppBuilderExtensions
    {
        /// <summary>
        /// The register reporting endpoint types.
        /// </summary>
        /// <param name="containerBuilder">
        /// The container builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public static void RegisterReportingEndpointTypes(
            this ContainerBuilder containerBuilder, 
            IReportingEndpointOptions options)
        {
            RegisterTypes(options, containerBuilder);
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="containerBuilder">
        /// The container builder.
        /// </param>
        public static void RegisterTypes(IReportingEndpointOptions options, ContainerBuilder containerBuilder)
        {
            // Infrastructure
            containerBuilder.Register(c => new ReportingEndpointContext(options.NameOrConnectionString))
                .As<ReportingEndpointContext>();
            containerBuilder.RegisterType<ReportingEndpointContextInitializer>().AsSelf().AsImplementedInterfaces();

            // containerBuilder.RegisterInstance(options.Logging).AsImplementedInterfaces();
            containerBuilder.RegisterInstance(options).AsImplementedInterfaces();

            // Services
            containerBuilder.RegisterType<NodeStatusService>().AsImplementedInterfaces();
            containerBuilder.RegisterType<ReportingEndpointCleanup>().AsImplementedInterfaces();
            containerBuilder.RegisterType<ConfigurationReportService>().AsImplementedInterfaces();
        }

        /// <summary>
        /// The use reporting endpoint component.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseReportingEndpointComponent(this IAppBuilder app, ILifetimeScope container)
        {
            var options = container.Resolve<IReportingEndpointOptions>();

            // Initialize database
            MigrationsContextFactory.NameOrConnectionString = options.NameOrConnectionString;
            var optionsLogging = options.Logging ?? new ReportingEndpointLogging();
            var logging = container.IsRegistered<IReportingEndpointLogging>()
                              ? container.Resolve<IReportingEndpointLogging>()
                              : optionsLogging;
            Database.SetInitializer(new ReportingEndpointContextInitializer(logging));

            var context = container.Resolve<ReportingEndpointContext>();
            context.Database.Initialize(false);
            context.Dispose();

            // Register for events
            var nodeStatusService = container.Resolve<INodeStatusService>();
            nodeStatusService.Start();

            SetupDbMaintenance();

            return app;
        }

        /// <summary>
        /// The setup db maintenance.
        /// </summary>
        private static void SetupDbMaintenance()
        {
            RecurringJob.AddOrUpdate<IReportingEndpointCleanup>(r => r.CleanupOldReports(), Cron.Daily(1));
        }
    }
}