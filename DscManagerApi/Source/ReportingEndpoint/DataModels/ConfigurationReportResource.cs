// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportResource.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    using System;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;

    /// <summary>
    ///     The configuration report resource.
    /// </summary>
    public class ConfigurationReportResource : ModelBase<ConfigurationReportResourceView>
    {
        /// <summary>
        ///     Gets or sets the duration in seconds.
        /// </summary>
        public decimal DurationInSeconds { get; set; }

        /// <summary>
        ///     Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        [JsonConverter(typeof(ConfigurationReportExceptionConverter))]
        public ConfigurationReportException Error { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether in desired state.
        /// </summary>
        public bool InDesiredState { get; set; }

        /// <summary>
        ///     Gets or sets the instance name.
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        ///     Gets or sets the module name.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        ///     Gets or sets the module version.
        /// </summary>
        public string ModuleVersion { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether reboot requested.
        /// </summary>
        public bool RebootRequested { get; set; }

        /// <summary>
        ///     Gets or sets the resource id.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        ///     Gets or sets the resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        ///     Gets or sets the source info.
        /// </summary>
        public string SourceInfo { get; set; }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether was set invoked.
        /// </summary>
        public bool? WasSetInvoked { get; set; }
    }
}