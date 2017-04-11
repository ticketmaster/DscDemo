// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppBuilderExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Extensions
{
    using System.Data.Entity;
    using System.Web.Http;

    using Autofac;
    using Autofac.Integration.WebApi;

    using Hangfire;
    using Hangfire.Storage;

    using Newtonsoft.Json;

    using Owin;

    using Ticketmaster.Common.PowerShellRunner;
    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.DeploymentServer.Services;
    using Ticketmaster.Dsc.Interfaces;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.Http;

    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     The i app builder extensions.
    /// </summary>
    public static class IAppBuilderExtensions
    {
        /// <summary>
        /// The register deployment server types.
        /// </summary>
        /// <param name="containerBuilder">
        /// The container builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public static void RegisterDeploymentServerTypes(
            this ContainerBuilder containerBuilder, 
            IDeploymentServerOptions options)
        {
            RegisterTypes(options, containerBuilder);
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="ContainerBuilder"/>.
        /// </returns>
        public static ContainerBuilder RegisterTypes(IDeploymentServerOptions options)
        {
            var containerBuilder = new ContainerBuilder();
            RegisterTypes(options, containerBuilder);
            return containerBuilder;
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
        public static void RegisterTypes(IDeploymentServerOptions options, ContainerBuilder containerBuilder)
        {
            // Infrastructure
            containerBuilder.Register(c => new DeploymentServerContext(options.NameOrConnectionString))
                .As<DeploymentServerContext>();
            containerBuilder.RegisterType<DeploymentServerContextInitializer>().AsSelf().AsImplementedInterfaces();

            // containerBuilder.RegisterInstance(options.Logging).AsImplementedInterfaces();
            containerBuilder.RegisterInstance(options).AsImplementedInterfaces();

            // Services
            containerBuilder.RegisterType<MofBuilderService>().AsSelf().AsImplementedInterfaces();
            containerBuilder.RegisterType<ConfigurationService>().AsImplementedInterfaces();
            containerBuilder.Register(c => new PowerShellRunner(null)).AsImplementedInterfaces();

            // Hangfire
            containerBuilder.Register(c => JobStorage.Current.GetMonitoringApi())
                .As<IMonitoringApi>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        /// <summary>
        /// The use deployment server.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseDeploymentServer(this IAppBuilder app)
        {
            return UseDeploymentServer(app, "/api/v2", new DeploymentServerOptions(), new DscComponentOptions());
        }

        /// <summary>
        /// The use deployment server.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseDeploymentServer(this IAppBuilder app, IDeploymentServerOptions options)
        {
            return UseDeploymentServer(app, "/api/v2", options, new DscComponentOptions());
        }

        /// <summary>
        /// The use deployment server.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="componentOptions">
        /// The component options.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseDeploymentServer(
            this IAppBuilder app, 
            IDeploymentServerOptions options, 
            IDscComponentOptions componentOptions)
        {
            return UseDeploymentServer(app, "/api/v2", options, componentOptions);
        }

        /// <summary>
        /// The use deployment server.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="controllerPrefix">
        /// The controller prefix.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="componentOptions">
        /// The component options.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseDeploymentServer(
            this IAppBuilder app, 
            string controllerPrefix, 
            IDeploymentServerOptions options, 
            IDscComponentOptions componentOptions)
        {
            return app.Map(
                controllerPrefix, 
                builder =>
                    {
                        var config = new HttpConfiguration();

                        // Set up DI
                        var containerBuilder = RegisterTypes(options);
                        var container = containerBuilder.Build();
                        config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                        // Logging
                        var optionsLogging = options.Logging ?? new DeploymentServerLogging();
                        var logging = container.IsRegistered<IDeploymentServerLogging>()
                                          ? container.Resolve<IDeploymentServerLogging>()
                                          : optionsLogging;

                        // Initialize database
                        MigrationsContextFactory.NameOrConnectionString = options.NameOrConnectionString;
                        Database.SetInitializer(new DeploymentServerContextInitializer(logging));
                        var context = container.Resolve<DeploymentServerContext>();
                        context.Database.Initialize(true);
                        context.Dispose();

                        // Configure HangFire DB
                        GlobalConfiguration.Configuration.UseSqlServerStorage(options.NameOrConnectionString);

                        // Configure HangFire
                        GlobalConfiguration.Configuration.UseAutofacActivator(container);
                        builder.UseHangfireServer(
                            new BackgroundJobServerOptions
                                {
                                    Queues = new[] { "mofbuilder", "default" }, 
                                    WorkerCount = options.WorkerCount
                                });
                        if (options.UseJobDashbaord)
                        {
                            app.UseHangfireDashboard();
                        }

                        // Configure cleanup jobs
                        SetupDbMaintenance();

                        // Set up filters
                        config.Filters.Add(new AuthorizeAttribute());
                        config.MessageHandlers.Add(new UrlHelperHandler(container.ComponentRegistry));
                        if (componentOptions.UsePrettyHtmlOutput)
                        {
                            config.Formatters.Add(new JsonTextMediaFormatter());
                        }

                        // Map routes and formatting
                        config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                            ReferenceLoopHandling.Ignore;

                        config.IncludeErrorDetailPolicy = componentOptions.ErrorDetailPolicy;

                        config.MapHttpAttributeRoutes();

                        builder.UseAutofacMiddleware(container);
                        builder.UseAutofacWebApi(config);
                        builder.UseWebApi(config);
                    });
        }

        /// <summary>
        /// The use deployment server component.
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
        public static IAppBuilder UseDeploymentServerComponent(this IAppBuilder app, ILifetimeScope container)
        {
            var options = container.Resolve<IDeploymentServerOptions>();

            // Initialize database
            var optionsLogging = options.Logging ?? new DeploymentServerLogging();
            var logging = container.IsRegistered<IDeploymentServerLogging>()
                              ? container.Resolve<IDeploymentServerLogging>()
                              : optionsLogging;
            MigrationsContextFactory.NameOrConnectionString = options.NameOrConnectionString;
            Database.SetInitializer(new DeploymentServerContextInitializer(logging));

            var context = container.Resolve<DeploymentServerContext>();
            context.Database.Initialize(true);
            context.Dispose();

            SetupDbMaintenance();

            return app;
        }

        /// <summary>
        ///     The setup db maintenance.
        /// </summary>
        private static void SetupDbMaintenance()
        {
            RecurringJob.AddOrUpdate<IMofBuilderService>(service => service.CleanupBuilds(), Cron.Daily(1));
            RecurringJob.AddOrUpdate<IConfigurationService>(service => service.CleanupArchives(), Cron.Daily(1));
        }
    }
}