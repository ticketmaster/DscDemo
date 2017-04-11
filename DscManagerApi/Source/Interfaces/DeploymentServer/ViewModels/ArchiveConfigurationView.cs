// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiveConfigurationView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The archive configuration view.
    /// </summary>
    public class ArchiveConfigurationView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveConfigurationView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public ArchiveConfigurationView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the archive timestamp.
        /// </summary>
        public DateTime ArchiveTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets or sets the checksum.
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        ///     Gets or sets the configuration document id.
        /// </summary>
        public int ConfigurationDocumentId { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; } = new List<Link>();

        /// <summary>
        ///     Gets or sets the published timestamp.
        /// </summary>
        public DateTime PublishedTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            this.Links = new List<Link>
                             {
                                 new Link(
                                     "GetDocument", 
                                     this.UrlHelper.Link(
                                         "ConfigurationGetDocument", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Get), 
                                 new Link(
                                     "Restore", 
                                     this.UrlHelper.Link(
                                         "ConfigurationRestore", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Put), 
                                 new Link(
                                     "Delete", 
                                     this.UrlHelper.Link(
                                         "ConfigurationDelete", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Delete)
                             };
        }
    }
}