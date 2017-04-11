// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapOptionsView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The bootstrap options view.
    /// </summary>
    public class BootstrapOptionsView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapOptionsView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public BootstrapOptionsView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the node data.
        /// </summary>
        public Hashtable NodeData { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        [Required]
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the package name.
        /// </summary>
        [Required]
        public string PackageName { get; set; }

        /// <summary>
        ///     Gets or sets the package version.
        /// </summary>
        [Required]
        public string PackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the repository uri.
        /// </summary>
        [Required]
        public string RepositoryUri { get; set; }

        [Required]
        public int BootstrapInterval { get; set; }

        /// <summary>
        ///     Gets or sets the url helper.
        /// </summary>
        protected UrlHelper UrlHelper { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            var links = new List<Link>();
            if (!string.IsNullOrEmpty(this.NodeName))
            {
                links.Add(
                    new Link(
                        "Bootstrap", 
                        this.UrlHelper.Link(
                            "BootstrapData", 
                            new Dictionary<string, object> { { "nodeName", this.NodeName } }), 
                        HttpMethod.Post));
            }

            this.Links = links;
        }
    }
}