// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptAttribute.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Attributes
{
    using System;

    /// <summary>
    ///     The encrypt attribute.
    /// </summary>
    public class EncryptAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is base 64 string.
        /// </summary>
        public bool IsBase64String { get; set; }

        #endregion
    }
}