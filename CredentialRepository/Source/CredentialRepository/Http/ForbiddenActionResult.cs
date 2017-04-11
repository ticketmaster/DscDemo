// <copyright file="ForbiddenActionResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ticketmaster.CredentialRepository.Http
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    ///     The forbidden action result.
    /// </summary>
    public class ForbiddenActionResult : IHttpActionResult
    {
        /// <summary>
        ///     The reason.
        /// </summary>
        private readonly string reason;

        /// <summary>
        ///     The request.
        /// </summary>
        private readonly HttpRequestMessage request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenActionResult"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public ForbiddenActionResult(HttpRequestMessage request, string reason)
        {
            this.request = request;
            this.reason = reason;
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = this.request.CreateResponse(HttpStatusCode.Forbidden, this.reason);
            return Task.FromResult(response);
        }
    }
}