using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketmaster.CredentialRepository.Extensions
{
    using System.Data.Entity;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Web.Http;

    using Autofac;
    using Autofac.Integration.WebApi;

    using Newtonsoft.Json;

    using Owin;

    using Ticketmaster.CredentialRepository.Controllers;
    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Http;
    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt.Services;

    // ReSharper disable once InconsistentNaming
    public static class IAppBuilderExtensions
    {
        public static IAppBuilder UseCredentialRepository(this IAppBuilder app, ICredentialRepositoryOptions options)
        {
            return UseCredentialRepository(app, "/api/v2/credentials", options);
        }

        public static IAppBuilder UseCredentialRepository(this IAppBuilder app, string controllerPrefix, ICredentialRepositoryOptions options)
        {
            return app.Map(
                controllerPrefix,
                builder =>
                {
                    var config = new HttpConfiguration();

                    // Set up DI
                    var containerBuilder = new ContainerBuilder();

                    containerBuilder.RegisterInstance(options).AsImplementedInterfaces();

                    Database.SetInitializer(options.DatabaseInitializer);
                    var encryptionOptions = new EncryptionServiceOptions(options.CertificateThumbprint, options.StoreName, options.StoreLocation);
                    containerBuilder.Register(c => new CredentialRepositoryContext(options.NameOrConnectionString, new EncryptionService(encryptionOptions)))
                        .As<DbContext>()
                        .As<CredentialRepositoryContext>();

                    var container = ConfigureDependencyInjection(config, containerBuilder);
                    builder.UseAutofacMiddleware(container);
                    builder.UseAutofacWebApi(config);

                    // Set up filters
                    config.Filters.Add(new AuthorizeAttribute());
                    config.MessageHandlers.Add(new AuthenticationHandler(container));

                    // Map routes and formatting
                    config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                    config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    config.IncludeErrorDetailPolicy = options.IncludeErrorDetailPolicy;

                    config.MapHttpAttributeRoutes();
                    builder.UseWebApi(config);
                });
        }

        private static IContainer ConfigureDependencyInjection(HttpConfiguration config, ContainerBuilder containerBuilder)
        {
            // Infrastructure
            containerBuilder.RegisterInstance(config).AsSelf();
            containerBuilder.RegisterApiControllers(Assembly.GetAssembly(typeof(IAppBuilderExtensions)));
            containerBuilder.RegisterType<PermissionRepository>().AsImplementedInterfaces().AsSelf();
            containerBuilder.RegisterGeneric(typeof(Repository<>)).AsImplementedInterfaces();
            containerBuilder.RegisterGeneric(typeof(AuthorizedRepository<>)).AsImplementedInterfaces();

            // Register and return
            var container = containerBuilder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }
    }
}
