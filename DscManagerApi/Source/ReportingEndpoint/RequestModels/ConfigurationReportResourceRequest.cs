// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportResourceRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.RequestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    ///     The configuration report resource request.
    /// </summary>
    public class ConfigurationReportResourceRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationReportResourceRequest" /> class.
        /// </summary>
        public ConfigurationReportResourceRequest()
        {
            var mapping = new TypeMapping(
                typeof(ConfigurationReportResourceRequest), 
                typeof(ConfigurationReportResource));
            mapping.PropertyResolvers.Add(
                new DestinationPropertyFromSourcePropertyResolver
                    <ConfigurationReportResourceRequest, ConfigurationReportResource>(
                    s => s.EndDate, 
                    v => v.StartDate.AddSeconds(Convert.ToInt32(v.DurationInSeconds))));
            this.TypeMappings.Add(mapping);
        }

        /// <summary>
        ///     Gets or sets the duration in seconds.
        /// </summary>
        public decimal DurationInSeconds { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the type mappings.
        /// </summary>
        protected ICollection<TypeMapping> TypeMappings { get; set; } = new List<TypeMapping>();

        /// <summary>
        ///     The map.
        /// </summary>
        /// <typeparam name="TDestination">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TDestination" />.
        /// </returns>
        public TDestination Map<TDestination>() where TDestination : class
        {
            var mapper =
                this.TypeMappings.FirstOrDefault(
                    m => m.CanResolveType(typeof(ConfigurationReportResourceRequest), typeof(TDestination)));

            return mapper?.Map(this) as TDestination;
        }
    }
}