// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapResultView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The bootstrap result view.
    /// </summary>
    public class BootstrapResultView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapResultView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public BootstrapResultView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        public BuildView Build { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        /// Gets or sets the local agent properties.
        /// </summary>
        public IEnumerable<ConfigurationPropertyView> LocalAgentProperties { get; set; }

        /// <summary>
        ///     Gets or sets the node data.
        /// </summary>
        public Hashtable NodeData { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            var links = new List<Link>
                            {
                                new Link(
                                    "Bootstrap", 
                                    this.UrlHelper.Link(
                                        "BootstrapData", 
                                        new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                                    HttpMethod.Post)
                            };
            this.Links = links;
        }
    }
}