// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscComponentOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces
{
    using System.Web.Http;

    /// <summary>
    ///     The dsc component options.
    /// </summary>
    public class DscComponentOptions : IDscComponentOptions
    {
        /// <summary>
        ///     Gets or sets the error detail policy.
        /// </summary>
        public IncludeErrorDetailPolicy ErrorDetailPolicy { get; set; } = IncludeErrorDetailPolicy.LocalOnly;

        /// <summary>
        ///     Gets or sets a value indicating whether use pretty html output.
        /// </summary>
        public bool UsePrettyHtmlOutput { get; set; } = true;
    }
}