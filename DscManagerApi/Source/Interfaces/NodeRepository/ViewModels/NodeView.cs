// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The node view.
    /// </summary>
    public class NodeView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public NodeView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the class.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        ///     Gets or sets the configuration environment.
        /// </summary>
        public string ConfigurationEnvironment { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is initial deployment.
        /// </summary>
        public bool IsInitialDeployment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is in maintenance.
        /// </summary>
        public bool IsInMaintenance { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        ///     Gets or sets the qualified name.
        /// </summary>
        public string QualifiedName { get; set; }

        /// <summary>
        ///     Gets or sets the roles.
        /// </summary>
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        ///     Gets or sets the site.
        /// </summary>
        public string Site { get; set; }

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
                                     "Details", 
                                     this.UrlHelper.Link(
                                         "NodesGet", 
                                         new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                                     HttpMethod.Get), 
                                 new Link(
                                     "Delete", 
                                     this.UrlHelper.Link(
                                         "NodesDelete", 
                                         new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                                     HttpMethod.Delete), 
                                 new Link("Update", this.UrlHelper.Link("NodesUpdate", null), HttpMethod.Put), 
                                 new Link(
                                     "Get Node Data", 
                                     this.UrlHelper.Link(
                                         "NodesConfigurationData", 
                                         new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                                     HttpMethod.Get), 
                                 new Link(
                                     "Build Node", 
                                     this.UrlHelper.Link(
                                         "NodesBuild", 
                                         new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                                     HttpMethod.Get)
                             };
        }
    }
}