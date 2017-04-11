// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionLogger.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Http
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;

    using Ticketmaster.Dsc.DscManager.Logging;

    /// <summary>
    /// The exception logger.
    /// </summary>
    public class ExceptionLogger : IExceptionLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLogger"/> class.
        /// </summary>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ExceptionLogger(IDscManagerLogging logging)
        {
            this.Logging = logging;
        }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        public IDscManagerLogging Logging { get; set; }

        /// <summary>
        /// Logs an unhandled exception.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous exception logging operation.
        /// </returns>
        /// <param name="context">
        /// The exception logger context.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            this.LogExceptionRecursive(context.Request.RequestUri.ToString(), context.Exception);
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// The log exception recursive.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LogExceptionRecursive(string uri, Exception e)
        {
            this.Logging.RequestException(uri, e.Message, e.Source, e.StackTrace);
            if (e.InnerException != null)
            {
                this.LogExceptionRecursive(uri, e.InnerException);
            }
        }
    }
}