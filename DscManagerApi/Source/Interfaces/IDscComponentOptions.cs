// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDscComponentOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces
{
    using System.Web.Http;

    /// <summary>
    ///     The DscComponentOptions interface.
    /// </summary>
    public interface IDscComponentOptions
    {
        /// <summary>
        ///     Gets or sets the error detail policy.
        /// </summary>
        IncludeErrorDetailPolicy ErrorDetailPolicy { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use pretty html output.
        /// </summary>
        bool UsePrettyHtmlOutput { get; set; }
    }
}