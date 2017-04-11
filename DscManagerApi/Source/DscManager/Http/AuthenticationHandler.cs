// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationHandler.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Http
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;

    using Microsoft.Owin;

    using Ticketmaster.Dsc.DscManager.Logging;

    /// <summary>
    /// The authentication handler.
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        ///     The container.
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public AuthenticationHandler(IContainer container, IDscManagerLogging logging)
        {
            this.container = container;
            this.Logging = logging;
        }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        protected IDscManagerLogging Logging { get; set; }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>. The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">
        /// The HTTP request message to send to the server.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token to cancel operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="request"/> was null.
        /// </exception>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var builder = new ContainerBuilder();
            var requestContext = request.GetRequestContext();
            var username = "Unknown";

            var principal = requestContext.Principal as WindowsPrincipal;
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var windowsIdentity = (WindowsIdentity)principal.Identity;
                windowsIdentity.AddClaim(new Claim("idp", "NTLM"));
                builder.RegisterInstance(request.GetRequestContext().Principal).As<IPrincipal>();
                username = principal.Identity.Name;
                builder.RegisterInstance(request.GetRequestContext().Principal).As<IPrincipal>();
            }
            else
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Anonymous"), new Claim("idp", "Anonymous") };
                var identity = new ClaimsIdentity(claims);
                var newPrincipal = new ClaimsPrincipal(identity);
                request.GetRequestContext().Principal = newPrincipal;
                builder.RegisterInstance(newPrincipal).As<IPrincipal>();
                username = newPrincipal.Identity.Name;
            }

            builder.Update(this.container);

            // Log this request
            this.Logging.ReceivedRequest(
                request.RequestUri.ToString(), 
                request.Method.Method, 
                this.GetClientIp(request), 
                username);
            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// The get client IP.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetClientIp(HttpRequestMessage request = null)
        {
            if (request == null)
            {
                return null;
            }

            return request.Properties.ContainsKey("MS_OwinContext")
                       ? ((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress
                       : null;
        }
    }
}