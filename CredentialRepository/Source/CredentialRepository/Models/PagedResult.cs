// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagedResult.cs" company="">
//   
// </copyright>
// <summary>
//   The paged result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.OData.Extensions;
    using System.Web.Http.OData.Query;

    using Newtonsoft.Json;

    /// <summary>
    /// The paged result.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="resultSize">
        /// The result size.
        /// </param>
        public PagedResult(IEnumerable<T> page, ODataQueryOptions options, int resultSize)
        {
            this.Results = page;
            this.ODataProperties = options.Request.ODataProperties();
            this.Options = options;
            this.ResultSize = resultSize;
            this.PopulateLinks();
        }

        /// <summary>
        /// The count.
        /// </summary>
        public long Count => this.Results.Count();

        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Link> Links { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public IEnumerable<T> Results { get; protected set; }

        /// <summary>
        /// The total count.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalCount => this.ODataProperties.TotalCount;

        /// <summary>
        /// Gets or sets the o data properties.
        /// </summary>
        protected HttpRequestMessageProperties ODataProperties { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        protected ODataQueryOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the result size.
        /// </summary>
        protected int ResultSize { get; set; }

        /// <summary>
        /// The get skip from uri.
        /// </summary>
        /// <returns>
        /// The <see cref="int?"/>.
        /// </returns>
        public int? GetSkipFromUri()
        {
            var query = this.Options.Request.RequestUri.ParseQueryString();
            if (query["$skip"] != null)
            {
                int result;
                int.TryParse(query["$skip"], NumberStyles.Any, new NumberFormatInfo(), out result);
                return result;
            }

            return null;
        }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public void PopulateLinks()
        {
            var links = new List<Link>();
            this.Links = links;
            if (this.ODataProperties.NextLink != null)
            {
                links.Add(new Link("Next Page", this.ODataProperties.NextLink.AbsoluteUri, HttpMethod.Get));
            }

            if (this.TotalCount.HasValue)
            {
                var lastPage = ((long)this.TotalCount - 1) / this.ResultSize;
                var skip = this.GetSkipFromUri();
                var currentPage = skip / this.ResultSize;
                if (lastPage * this.ResultSize >= skip)
                {
                    links.Add(new Link("Last Page", this.SetSkipFromUri(lastPage * this.ResultSize), HttpMethod.Get));
                }

                if (currentPage > 1)
                {
                    var previousPage = currentPage - 1;
                    links.Add(
                        new Link(
                            "Previous Page", 
                            this.SetSkipFromUri((long)previousPage * this.ResultSize), 
                            HttpMethod.Get));
                }

                var query = this.Options.Request.RequestUri.ParseQueryString();
                if (query["$skip"] != null)
                {
                    query.Remove("$skip");
                }

                links.Add(
                    new Link(
                        "First Page", 
                        Uri.UnescapeDataString(
                            this.Options.Request.RequestUri.GetLeftPart(UriPartial.Path) + "?" + string.Join("&", query)), 
                        HttpMethod.Get));
            }
        }

        /// <summary>
        /// The set skip from uri.
        /// </summary>
        /// <param name="skip">
        /// The skip.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string SetSkipFromUri(long skip)
        {
            var query = this.Options.Request.RequestUri.ParseQueryString();
            query["$skip"] = skip.ToString();

            return
                Uri.UnescapeDataString(
                    this.Options.Request.RequestUri.GetLeftPart(UriPartial.Path) + "?" + string.Join("&", query));
        }
    }
}