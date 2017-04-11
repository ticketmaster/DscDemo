// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportRecordDetailView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    /// The configuration report record detail view.
    /// </summary>
    public class ConfigurationReportRecordDetailView : ConfigurationReportRecordView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationReportRecordDetailView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public ConfigurationReportRecordDetailView(UrlHelper urlHelper)
            : base(urlHelper)
        {
        }

        /// <summary>
        ///     Gets or sets the resources.
        /// </summary>
        public IEnumerable<ConfigurationReportResourceView> Resources { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public override void PopulateLinks()
        {
            this.Links = new List<Link>
                             {
                                 new Link(
                                     "View Run", 
                                     this.UrlHelper.Link(
                                         "ConfigurationReportGetById", 
                                         new Dictionary<string, object> { { "id", this.RunId } }), 
                                     HttpMethod.Get)
                             };
        }
    }
}