// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinStartup.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Owin;

using Ticketmaster.Dsc.DscManager;

[assembly: OwinStartup(typeof(OwinStartup))]

namespace Ticketmaster.Dsc.DscManager
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web.Cors;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Autofac;
    using Autofac.Integration.WebApi;

    using Hangfire;
    using Hangfire.Common;

    using Newtonsoft.Json;

    using Owin;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Extensions;
    using Ticketmaster.Dsc.DeploymentServer.Extensions;
    using Ticketmaster.Dsc.DscManager.DataAccess;
    using Ticketmaster.Dsc.DscManager.DataModels;
    using Ticketmaster.Dsc.DscManager.Http;
    using Ticketmaster.Dsc.DscManager.Logging;
    using Ticketmaster.Dsc.DscManager.Services;
    using Ticketmaster.Dsc.Interfaces;
    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Dsc.Interfaces.Http;
    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.NodeRepository.Extensions;
    using Ticketmaster.Dsc.ReportingEndpoint.Extensions;
    using Ticketmaster.Integrations.Slack.Configuration;
    using Ticketmaster.Integrations.Slack.Services;

    /// <summary>
    ///     The owin startup.
    /// </summary>
    public class OwinStartup
    {
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        public static ILifetimeScope Container { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        protected static IDscManagerOptions Options { get; set; }

        /// <summary>
        /// The get dsc manager options.
        /// </summary>
        /// <returns>
        /// The <see cref="IDscManagerOptions"/>.
        /// </returns>
        public static IDscManagerOptions GetDscManagerOptions()
        {
            return Options
                   ?? (Options = DscManagerOptions.ReadFromFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.json"));
        }

        /// <summary>
        ///     The register types.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>
        ///     The <see cref="ContainerBuilder" />.
        /// </returns>
        public static ContainerBuilder RegisterTypes(IDscManagerOptions options)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<EnvironmentProperties>().AsImplementedInterfaces().SingleInstance();

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Register API infrastructure
            containerBuilder.Register(c => new HttpConfiguration()).AsSelf().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterApiControllers(
                allAssemblies.Where(a => a.FullName.StartsWith("Ticketmaster.")).ToArray());

            var assemblies = allAssemblies.Where(a => a.FullName.StartsWith("Ticketmaster."));
            foreach (var assembly in assemblies)
            {
                foreach (var viewModel in assembly.GetTypes().Where(t => typeof(IViewModel).IsAssignableFrom(t)))
                {
                    containerBuilder.RegisterType(viewModel).AsSelf().AsImplementedInterfaces().InstancePerDependency();
                }
            }

            containerBuilder.Register(c => new ViewModelFactory(c.Resolve<IComponentContext>()))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            containerBuilder.Register(c => new DscManagerContext("DscManager")).AsSelf();
            containerBuilder.RegisterType<DscManagerContextInitializer>().AsSelf().AsImplementedInterfaces();
            containerBuilder.RegisterType<DataInitializer>().AsSelf().AsImplementedInterfaces();

            // Services
            containerBuilder.RegisterType<DatabaseLogger>().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<ConsoleLogger>().AsImplementedInterfaces();
            containerBuilder.RegisterType<DscEventManager>().AsImplementedInterfaces().SingleInstance();
            containerBuilder.RegisterType<DscLogging>().AsImplementedInterfaces();
            containerBuilder.RegisterType<ExceptionLogger>().AsImplementedInterfaces();
            if (options.UseSlackLogging)
            {
                containerBuilder.RegisterType<SlackLogger>().AsImplementedInterfaces();
            }
            containerBuilder.Register(
                c =>
                new SlackIntegrationService(
                    new SlackIntegrationOptions
                        {
                            ProxyUri = "http://squid.sys.tash1.syseng.tmcs:3128",
                            UseProxy = true,
                            SlackDefaultChannel = "#archtics-dsc-pov",
                            SlackDefaultName = "DscManager",
                            SlackDestinationUri =
                                "https://hooks.slack.com/services/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"
                        })).AsImplementedInterfaces();

            return containerBuilder;
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public static void Stop()
        {
            var eventHandlers = Container.Resolve<IEnumerable<IDscEventHandler>>();
            foreach (var handler in eventHandlers)
            {
                handler.Stop();
            }
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            var options = GetDscManagerOptions();
            var httpListener = (HttpListener)app.Properties["System.Net.HttpListener"];
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Ntlm | AuthenticationSchemes.Anonymous;

            // Set up DI
            var containerBuilder = RegisterTypes(options);
            containerBuilder.RegisterInstance(options).AsImplementedInterfaces();

            // Register types for components
            containerBuilder.RegisterNodeRepositoryTypes(options.NodeRepositoryOptions);
            containerBuilder.RegisterDeploymentServerTypes(options.DeploymentServerOptions);
            containerBuilder.RegisterReportingEndpointTypes(options.ReportingEndpointOptions);

            // Build container
            var container = containerBuilder.Build();
            Container = container;

            // Initialize DB
            MigrationsContextFactory.NameOrConnectionString = "DscManager";
            var logging = container.Resolve<IDscManagerLogging>();
            Database.SetInitializer(new DscManagerContextInitializer(logging));

            var context = container.Resolve<DscManagerContext>();
            context.Database.Initialize(true);
            var init = container.Resolve<IDataInitializer>();
            if (init.ShouldRunInitializer())
            {
                init.Initialize();
            }
            context.Dispose();

            // Configure HangFire
            GlobalConfiguration.Configuration.UseSqlServerStorage("DscManager");
            GlobalConfiguration.Configuration.UseAutofacActivator(container);
            if (options.UseJobDashboard)
            {
                app.UseHangfireDashboard();
            }

            this.SetupDbCleanup();

            if (options.UseHangfireJobServer)
            {
                var jobServerOptions = new BackgroundJobServerOptions
                                           {
                                               Queues = new[] { "mofbuilder" },
                                               WorkerCount = options.WorkerCount
                                           };
                app.UseHangfireServer(jobServerOptions);
            }

            var hangfireOptions = new BackgroundJobServerOptions
                                      {
                                          Queues = new[] { "priority", "default" },
                                          WorkerCount = 10
                                      };

            app.UseHangfireServer(hangfireOptions);

            // Resolve ViewModelFactory so we have an instance
            container.Resolve<IViewModelFactory>();

            // Set up config
            var config = container.Resolve<HttpConfiguration>();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.MessageHandlers.Add(new UrlHelperHandler(container.ComponentRegistry));

            // Filters
            var p = container.Resolve<IEnvironmentProperties>();
            if (!p.DebuggerAttached)
            {
                config.Filters.Add(new AuthorizeAttribute());
            }
            config.MessageHandlers.Add(new AuthenticationHandler(container, container.Resolve<IDscManagerLogging>()));

            // Formatters
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            config.Formatters.Add(new JsonTextMediaFormatter());

            // Policies
            config.IncludeErrorDetailPolicy = options.ErrorDetailPolicy;

            // Routes
            config.MapHttpAttributeRoutes(new CentralizedPrefixProvider("api/v{version:int}"));

            // Set up components
            app.UseNodeRepositoryComponent(container);
            app.UseDeploymentServerComponent(container);
            app.UseReportingEndpointComponent(container);

            app.UseCredentialRepository(options.CredentialRepositoryOptions);

            // Set up event infrastructure
            var eventHandlers = container.Resolve<IEnumerable<IDscEventHandler>>();
            foreach (var handler in eventHandlers)
            {
                handler.Start();
            }

            // Set up middleware
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            if (options.UseApi)
            {
                app.UseWebApi(config);
            }

            
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.EnsureInitialized();

            container.Resolve<IDscManagerLogging>().Start();
        }

        /// <summary>
        /// The setup db cleanup.
        /// </summary>
        private void SetupDbCleanup()
        {
            RecurringJob.AddOrUpdate<IDscManagerDbCleanup>(c => c.CleanupOldLogEntries(), Cron.Daily(2));
        }
    }
}