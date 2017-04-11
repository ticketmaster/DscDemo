// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System.Collections.Generic;

    /// <summary>
    ///     The ViewModel interface.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        ///     Gets or sets the links.
        /// </summary>
        IEnumerable<Link> Links { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        void PopulateLinks();
    }
}