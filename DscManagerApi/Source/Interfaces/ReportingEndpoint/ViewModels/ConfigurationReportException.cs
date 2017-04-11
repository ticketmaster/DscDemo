// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportException.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels
{
    /// <summary>
    ///     The configuration status exception.
    /// </summary>
    public class ConfigurationReportException
    {
        /// <summary>
        ///     Gets or sets the help link.
        /// </summary>
        public string HelpLink { get; set; }

        /// <summary>
        ///     Gets or sets the h result.
        /// </summary>
        public int HResult { get; set; }

        /// <summary>
        ///     Gets or sets the inner exception.
        /// </summary>
        public ConfigurationReportException InnerException { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets the stack trace.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     Gets or sets the target site.
        /// </summary>
        public string TargetSite { get; set; }
    }
}