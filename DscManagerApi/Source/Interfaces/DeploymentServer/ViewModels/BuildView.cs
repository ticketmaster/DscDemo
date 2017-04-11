// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The build view.
    /// </summary>
    public class BuildView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public BuildView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the complete timestamp.
        /// </summary>
        public DateTime CompleteTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the request timestamp.
        /// </summary>
        public DateTime RequestTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public virtual void PopulateLinks()
        {
            var links = new List<Link>
                            {
                                new Link(
                                    "Details", 
                                    this.UrlHelper.Link(
                                        "BuildsDetail", 
                                        new Dictionary<string, object> { { "id", this.Id } }), 
                                    HttpMethod.Get)
                            };

            if (this.Status == BuildStatus.Enqueued || this.Status == BuildStatus.InProgress)
            {
                links.Add(
                    new Link(
                        "Cancel Build", 
                        this.UrlHelper.Link("BuildsCancel", new Dictionary<string, object> { { "id", this.Id } }), 
                        HttpMethod.Get));
            }

            this.Links = links;
        }
    }
}