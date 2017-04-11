// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The authentication handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Http
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;

    /// <summary>
    ///     The authentication handler.
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
        public AuthenticationHandler(IContainer container)
        {
            this.container = container;
        }

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

            var principal = requestContext.Principal as WindowsPrincipal;
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var windowsIdentity = (WindowsIdentity)principal.Identity;
                windowsIdentity.AddClaim(new Claim("idp", "NTLM"));
                builder.RegisterInstance(request.GetRequestContext().Principal).As<IPrincipal>();
            }
            else
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Anonymous"), new Claim("idp", "Anonymous") };
                var identity = new ClaimsIdentity(claims);
                var newPrincipal = new ClaimsPrincipal(identity);
                request.GetRequestContext().Principal = newPrincipal;
                builder.RegisterInstance(newPrincipal).As<IPrincipal>();
            }

            builder.Update(this.container);
            return base.SendAsync(request, cancellationToken);
        }
    }
}