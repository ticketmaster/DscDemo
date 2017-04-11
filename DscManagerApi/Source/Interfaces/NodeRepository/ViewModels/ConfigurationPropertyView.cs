// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPropertyView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The configuration property view.
    /// </summary>
    public class ConfigurationPropertyView : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationPropertyView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public ConfigurationPropertyView(UrlHelper urlHelper)
        {
            this.UrlHelper = urlHelper;
        }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PropertyScope Scope { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PropertyType Type { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        public object Value { get; set; }

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
                                         "ConfigurationPropertiesDetail", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Get), 
                                 new Link(
                                     "Delete", 
                                     this.UrlHelper.Link(
                                         "ConfigurationPropertiesDelete", 
                                         new Dictionary<string, object> { { "id", this.Id } }), 
                                     HttpMethod.Delete), 
                                 new Link(
                                     "Update", 
                                     this.UrlHelper.Link("ConfigurationPropertiesUpdate", null), 
                                     HttpMethod.Put)
                             };
        }
    }
}