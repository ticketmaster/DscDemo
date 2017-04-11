// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportRecordRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.RequestModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    ///     The configuration report record request.
    /// </summary>
    public class ConfigurationReportRecordRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationReportRecordRequest" /> class.
        /// </summary>
        public ConfigurationReportRecordRequest()
        {
            var mapping = new TypeMapping(typeof(ConfigurationReportRecordRequest), typeof(ConfigurationReport));
            mapping.PropertyResolvers.Add(
                new DestinationPropertyFromSourcePropertyResolver<ConfigurationReportRecordRequest, ConfigurationReport>
                    (s => s.EndDate, v => v.StartDate.AddSeconds(v.DurationInSeconds)));
            mapping.PropertyResolvers.Add(
                new DestinationPropertyFromSourcePropertyResolver<ConfigurationReportRecordRequest, ConfigurationReport>
                    (
                    d => d.Resources, 
                    s =>
                        {
                            return new ConfigurationReportResourceEntity
                                       {
                                           Resources =
                                               s.Resources.Select(
                                                   res =>
                                                   res
                                                       .Map
                                                       <ConfigurationReportResource>())
                                       };
                        }));
            this.TypeMappings.Add(mapping);
        }

        /// <summary>
        ///     Gets or sets the configuration package name.
        /// </summary>
        public string ConfigurationPackageName { get; set; }

        /// <summary>
        ///     Gets or sets the configuration version.
        /// </summary>
        public string ConfigurationPackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the duration in seconds.
        /// </summary>
        public int DurationInSeconds { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether in desired state.
        /// </summary>
        public bool InDesiredState { get; set; }

        /// <summary>
        ///     Gets or sets the job id.
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        ///     Gets or sets the lcm version.
        /// </summary>
        public string LcmVersion { get; set; }

        /// <summary>
        ///     Gets or sets the number of resources.
        /// </summary>
        public int NumberOfResources { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether reboot requested.
        /// </summary>
        public bool RebootRequested { get; set; }

        /// <summary>
        ///     Gets or sets the resources.
        /// </summary>
        public IEnumerable<ConfigurationReportResourceRequest> Resources { get; set; }

        /// <summary>
        ///     Gets or sets the run id.
        /// </summary>
        public Guid RunId { get; set; } // From node agent

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Required]
        public string Target { get; set; } // From node agent

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationType Type { get; set; } // From node agent*/

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
                    m => m.CanResolveType(typeof(ConfigurationReportRecordRequest), typeof(TDestination)));

            return mapper?.Map(this) as TDestination;
        }
    }
}