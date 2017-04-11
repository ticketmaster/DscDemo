// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuditable.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt
{
    using System;

    /// <summary>
    /// The Auditable interface.
    /// </summary>
    public interface IAuditable
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the change guid.
        /// </summary>
        Guid ChangeGuid { get; set; }

        /// <summary>
        /// Gets or sets the entity guid.
        /// </summary>
        Guid EntityGuid { get; set; }

        #endregion
    }
}