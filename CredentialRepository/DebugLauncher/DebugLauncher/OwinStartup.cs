using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DebugLauncher;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(OwinStartup))]
namespace DebugLauncher
{
    using System.Data.Entity;
    using System.Net;
    using System.Security.AccessControl;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Http;

    using Owin;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Extensions;
    using Ticketmaster.CredentialRepository.Models;

    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpListener = (HttpListener)app.Properties["System.Net.HttpListener"];
            httpListener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            var options = new CredentialRepositoryOptions(
                "62553233752FA5772F793B80C7B4DA9C85E590A3",
                StoreLocation.LocalMachine,
                "My",
                "CredentialRepositoryContext") { IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always, DatabaseInitializer = new ContextInitializer() };
            app.UseCredentialRepository(options);
        }
    }

    public class ContextInitializer : CreateDatabaseIfNotExists<CredentialRepositoryContext>
    {
        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        ///             The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed. </param>
        protected override void Seed(CredentialRepositoryContext context)
        {
            var permissions = new List<Permission> { new Permission { Access = AccessControlType.Allow, Action = PermissionActions.All, EntityGuid = new Guid(), Identity = "Mike.Walker", IdentityProvider = "LYV", Model = "Credential"} };
            context.Permissions.AddRange(permissions);
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
