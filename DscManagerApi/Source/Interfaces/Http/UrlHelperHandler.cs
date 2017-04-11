// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlHelperHandler.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Http
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Routing;

    using Autofac;
    using Autofac.Core;

    /// <summary>
    ///     The url helper handler.
    /// </summary>
    public class UrlHelperHandler : DelegatingHandler
    {
        /// <summary>
        ///     The container.
        /// </summary>
        private readonly IComponentRegistry container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlHelperHandler"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public UrlHelperHandler(IComponentRegistry container)
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
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(request.GetRequestContext().Url)
                .As<UrlHelper>()
                .InstancePerRequest()
                .SingleInstance();
            builder.Update(this.container);
            var result = await base.SendAsync(request, cancellationToken);
            return result;
        }
    }
}