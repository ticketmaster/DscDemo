// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportRecordView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Http.Routing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The configuration report record detail view.
    /// </summary>
    public class ConfigurationReportRecordView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReportRecordView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public ConfigurationReportRecordView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
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
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in desired state.
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
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the number of resources.
        /// </summary>
        public int NumberOfResources { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether reboot requested.
        /// </summary>
        public bool RebootRequested { get; set; }

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
        public ConfigurationType Type { get; set; } // From node agent

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public virtual void PopulateLinks()
        {
            this.Links = new List<Link>
                             {
                                 new Link(
                                     "View Run", 
                                     this.UrlHelper.Link(
                                         "ConfigurationReportGetById", 
                                         new Dictionary<string, object> { { "id", this.RunId } }), 
                                     HttpMethod.Get), 
                                 new Link(
                                     "View Detail", 
                                     this.UrlHelper.Link(
                                         "ConfigurationReportRecordGetById", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Get)
                             };
        }
    }
}