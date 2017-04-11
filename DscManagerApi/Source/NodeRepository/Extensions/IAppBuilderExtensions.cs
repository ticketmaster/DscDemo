// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppBuilderExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Extensions
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Autofac;
    using Autofac.Integration.WebApi;

    using Newtonsoft.Json;

    using Owin;

    using Ticketmaster.Dsc.Interfaces;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.NodeRepository;
    using Ticketmaster.Dsc.NodeRepository.Controllers;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Logging;
    using Ticketmaster.Dsc.NodeRepository.Services;

    // ReSharper disable once InconsistentNaming
    /// <summary>
    ///     The i app builder extensions.
    /// </summary>
    public static class IAppBuilderExtensions
    {
        /// <summary>
        /// The configure dependency injection.
        /// </summary>
        /// <param name="containerBuilder">
        /// The container builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public static void RegisterNodeRepositoryTypes(
            this ContainerBuilder containerBuilder, 
            INodeRepositoryOptions options)
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
        public static ContainerBuilder RegisterTypes(INodeRepositoryOptions options)
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
        public static void RegisterTypes(INodeRepositoryOptions options, ContainerBuilder containerBuilder)
        {
            // Infrastructure
            if (options.Logging != null)
            {
                containerBuilder.RegisterInstance(options.Logging).AsImplementedInterfaces();
            }

            containerBuilder.RegisterInstance(options).AsImplementedInterfaces();
            containerBuilder.RegisterType<NodeRepositoryContextInitializer>().AsSelf().AsImplementedInterfaces();

            // Services
            containerBuilder.Register(c => new NodeRepositoryContext(options.NameOrConnectionString))
                .As<NodeRepositoryContext>();

            containerBuilder.Register(
                c =>
                new NodeConfigurationService(c.Resolve<NodeRepositoryContext>(), c.Resolve<INodeRepositoryLogging>())
                    {
                        BuildService
                            =
                            c.IsRegistered<IMofBuilderService>() ?
                            c
                            .Resolve
                            <
                            IMofBuilderService
                            >
                            (
                                ) : null
                    })
                .AsImplementedInterfaces().AsSelf();

            containerBuilder.Register(
                c =>
                new ConfigurationPropertiesController(
                    c.Resolve<NodeRepositoryContext>(),
                    c.Resolve<INodeConfigurationService>(),
                    c.Resolve<INodeRepositoryLogging>())
                    {
                        BuildService =
                            c.IsRegistered<IMofBuilderService>()
                                ? c.Resolve<IMofBuilderService>()
                                : null
                    }).AsImplementedInterfaces().AsSelf().As<ApiController>();
        }

        /// <summary>
        /// The use node repository.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <returns>
        /// The <see cref="IAppBuilder"/>.
        /// </returns>
        public static IAppBuilder UseNodeRepository(this IAppBuilder app)
        {
            return UseNodeRepository(app, "/api/v2", new NodeRepositoryOptions(), new DscComponentOptions());
        }

        /// <summary>
        /// The use node repository.
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
        public static IAppBuilder UseNodeRepository(this IAppBuilder app, INodeRepositoryOptions options)
        {
            return UseNodeRepository(app, "/api/v2", options, new DscComponentOptions());
        }

        /// <summary>
        /// The use node repository.
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
        public static IAppBuilder UseNodeRepository(
            this IAppBuilder app, 
            INodeRepositoryOptions options, 
            IDscComponentOptions componentOptions)
        {
            return UseNodeRepository(app, "/api/v2", options, componentOptions);
        }

        /// <summary>
        /// The use node repository.
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
        public static IAppBuilder UseNodeRepository(
            this IAppBuilder app, 
            string controllerPrefix, 
            INodeRepositoryOptions options, 
            IDscComponentOptions componentOptions)
        {
            return app.Map(
                controllerPrefix, 
                builder =>
                    {
                        // Set up DI
                        var containerBuilder = RegisterTypes(options);
                        containerBuilder.RegisterApiControllers(Assembly.GetAssembly(typeof(IAppBuilderExtensions)));

                        containerBuilder.RegisterType<ViewModelFactory>().AsImplementedInterfaces().SingleInstance();

                        // Register all ViewModels
                        var assemblies =
                            AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Ticketmaster."));
                        foreach (var assembly in assemblies)
                        {
                            foreach (
                                var viewModel in assembly.GetTypes().Where(t => typeof(IViewModel).IsAssignableFrom(t)))
                            {
                                containerBuilder.RegisterType(viewModel).AsSelf().AsImplementedInterfaces();
                            }
                        }

                        containerBuilder.Register(c => new ViewModelFactory(c.Resolve<IComponentContext>()))
                            .AsSelf()
                            .AsImplementedInterfaces()
                            .SingleInstance();

                        var container = containerBuilder.Build();

                        var config = new HttpConfiguration
                                         {
                                             DependencyResolver =
                                                 new AutofacWebApiDependencyResolver(container)
                                         };

                        // Initialize database
                        MigrationsContextFactory.NameOrConnectionString = options.NameOrConnectionString;
                        var optionsLogging = options.Logging ?? new NodeRepositoryLogging();
                        var logging = container.IsRegistered<INodeRepositoryLogging>()
                                          ? container.Resolve<INodeRepositoryLogging>()
                                          : optionsLogging;
                        Database.SetInitializer(new NodeRepositoryContextInitializer(logging));

                        var context = container.Resolve<NodeRepositoryContext>();
                        context.Database.Initialize(true);
                        context.Dispose();

                        // Initialize ViewModelFactory
                        container.Resolve<ViewModelFactory>();

                        // Set up filters
                        config.Filters.Add(new AuthorizeAttribute());
                        config.MessageHandlers.Add(new UrlHelperHandler(container.ComponentRegistry));

                        // Map routes and formatting
                        // config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                        config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                            ReferenceLoopHandling.Ignore;
                        config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

                        if (componentOptions.UsePrettyHtmlOutput)
                        {
                            config.Formatters.Add(new JsonTextMediaFormatter());
                        }

                        config.IncludeErrorDetailPolicy = componentOptions.ErrorDetailPolicy;

                        config.MapHttpAttributeRoutes();

                        builder.UseAutofacMiddleware(container);
                        builder.UseAutofacWebApi(config);

                        builder.UseWebApi(config);

                        config.EnsureInitialized();
                    });
        }

        /// <summary>
        /// The use node repository component.
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
        public static IAppBuilder UseNodeRepositoryComponent(this IAppBuilder app, ILifetimeScope container)
        {
            var options = container.Resolve<INodeRepositoryOptions>();
            var x = ConfigurationManager.ConnectionStrings;
            // Initialize database
            MigrationsContextFactory.NameOrConnectionString = options.NameOrConnectionString;
            var optionsLogging = options.Logging ?? new NodeRepositoryLogging();
            var logging = container.IsRegistered<INodeRepositoryLogging>()
                              ? container.Resolve<INodeRepositoryLogging>()
                              : optionsLogging;

            Database.SetInitializer(new NodeRepositoryContextInitializer(logging));

            var context = container.Resolve<NodeRepositoryContext>();
            context.Database.Initialize(true);
            context.Dispose();

            return app;
        }
    }
}