// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeDetailView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Routing;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The node detail view.
    /// </summary>
    public class NodeDetailView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeDetailView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public NodeDetailView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the bootstrap properties.
        /// </summary>
        public IEnumerable<ConfigurationPropertyView> BootstrapProperties { get; set; }

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
        ///     Gets or sets a value indicating whether is in maintenance.
        /// </summary>
        public bool IsInMaintenance { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the local agent properties.
        /// </summary>
        public IEnumerable<ConfigurationPropertyView> LocalAgentProperties { get; set; }

        /// <summary>
        ///     Gets or sets the maintenance schedule.
        /// </summary>
        public MaintenanceScheduleView MaintenanceSchedule { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the node properties.
        /// </summary>
        public IEnumerable<ConfigurationPropertyView> NodeProperties { get; set; }

        /// <summary>
        ///     Gets or sets the product.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        ///     Gets or sets the qualified name.
        /// </summary>
        public string QualifiedName { get; set; }

        /// <summary>
        /// Gets or sets the resource version properties.
        /// </summary>
        public IEnumerable<ConfigurationPropertyView> ResourceVersionProperties { get; set; }

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

        /// <summary>
        ///     The to node data.
        /// </summary>
        /// <returns>
        ///     The <see cref="Hashtable" />.
        /// </returns>
        public Hashtable ToNodeData()
        {
            var data = new Hashtable();
            var excludedProperties = new[] { "LocalAgentProperties", "BootstrapProperties", "ResourceVersionProperties", "Links" };
            foreach (var propertyInfo in
                this.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !excludedProperties.Contains(p.Name)))
            {
                switch (propertyInfo.Name)
                {
                    case "NodeProperties":
                        var properties = propertyInfo.GetValue(this) as IEnumerable<ConfigurationPropertyView>;
                        if (properties == null)
                        {
                            break;
                        }

                        foreach (var property in properties)
                        {
                            data[property.Name] = property.Value;
                        }

                        break;
                    default:
                        data[propertyInfo.Name] = propertyInfo.GetValue(this);
                        break;
                }
            }

            var rProps = this.ResourceVersionProperties.GroupBy(s => s.Name);
            data["ResourceVersionProperties"] = rProps.Select(g => g.OrderByDescending(p => p.Scope).FirstOrDefault()).ToDictionary(s => s.Name, s => s.Value);
            // data["ResourceVersionProperties"] = this.ResourceVersionProperties.ToDictionary(s => s.Name, s => s.Value);

            return data;
        }
    }
}