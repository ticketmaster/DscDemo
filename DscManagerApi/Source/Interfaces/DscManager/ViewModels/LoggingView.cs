// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    /// The logging view.
    /// </summary>
    public class LoggingView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public LoggingView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the url helper.
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
                                     "View Log Entry", 
                                     this.UrlHelper.Link(
                                         "LoggingViewDetail", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Get)
                             };
        }
    }
}