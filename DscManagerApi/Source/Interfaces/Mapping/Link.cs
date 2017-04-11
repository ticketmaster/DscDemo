// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Link.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;

    /// <summary>
    ///     The link.
    /// </summary>
    [DataContract]
    public class Link
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="href">
        /// The href.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        public Link(string name, string href, HttpMethod method)
        {
            this.Name = name;
            this.Href = href;
            this.Method = method;
        }

        /// <summary>
        ///     Gets or sets the href.
        /// </summary>
        [DataMember]
        public string Href { get; protected set; }

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpMethod Method { get; protected set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; protected set; }
    }
}