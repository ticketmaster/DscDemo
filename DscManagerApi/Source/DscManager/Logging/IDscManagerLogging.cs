// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscManagerLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Logging
{
    /// <summary>
    ///     The DscManagerLogging interface.
    /// </summary>
    public interface IDscManagerLogging
    {
        /// <summary>
        /// The begin dsc manager database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void BeginDscManagerDatabaseSeed(string name);

        /// <summary>
        /// The cleanup old log entries.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="daysToKeepLogs">
        /// The days to keep logs.
        /// </param>
        void CleanupOldLogEntries(int count, int daysToKeepLogs);

        /// <summary>
        /// The end dsc manager database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void EndDscManagerDatabaseSeed(string name);

        /// <summary>
        /// The received request.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="clientIp">
        /// The client ip.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        void ReceivedRequest(string uri, string method, string clientIp, string username);

        /// <summary>
        /// The request exception.
        /// </summary>
        /// <param name="requestUrl">
        /// The request url.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="stackTrace">
        /// The stack trace.
        /// </param>
        void RequestException(string requestUrl, string message, string source, string stackTrace);

        /// <summary>
        ///     The start.
        /// </summary>
        void Start();

        /// <summary>
        /// The upgrade dsc manager database schema.
        /// </summary>
        void UpgradeDscManagerDatabaseSchema();
    }
}