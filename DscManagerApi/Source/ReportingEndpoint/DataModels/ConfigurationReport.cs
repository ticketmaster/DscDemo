// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReport.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;

    /// <summary>
    ///     The configuration report.
    /// </summary>
    public class ConfigurationReport : ModelBase<ConfigurationReportRecordView>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationReport" /> class.
        /// </summary>
        public ConfigurationReport()
        {
            this.AddTypeMapping<ConfigurationReportRecordDetailView>();
            foreach (var map in this.TypeMappings)
            {
                map.PropertyResolvers.Add(
                    new SourceMemberPropertyResolver<ConfigurationReport>(d => d.Resources, s => s.Resources.Resources));
            }
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
        ///     Gets or sets the end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in desired state.
        /// </summary>
        public bool InDesiredState { get; set; }

        /// <summary>
        ///     Gets or sets the job id.
        /// </summary>
        [Index(IsUnique = true)]
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
        [Index(IsUnique = false)]
        public bool RebootRequested { get; set; }

        /// <summary>
        ///     Gets or sets the resources.
        /// </summary>
        public virtual ConfigurationReportResourceEntity Resources { get; set; }

        /// <summary>
        ///     Gets or sets the resources id.
        /// </summary>
        [ForeignKey("Resources")]
        public int ResourcesId { get; set; }

        /// <summary>
        ///     Gets or sets the run id.
        /// </summary>
        [Index(IsUnique = false)]
        public Guid RunId { get; set; } // From node agent

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        [Index(IsUnique = false)]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [Index(IsUnique = false)]
        public ConfigurationStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string Target { get; set; } // From node agent

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [Index(IsUnique = false)]
        public ConfigurationType Type { get; set; } // From node agent
    }
}