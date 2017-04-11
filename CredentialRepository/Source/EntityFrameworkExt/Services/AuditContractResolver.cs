// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditContractResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Ticketmaster.Dsc.EntityFrameworkExt.Attributes;

    /// <summary>
    /// The audit contract resolver.
    /// </summary>
    public class AuditContractResolver : DefaultContractResolver
    {
        #region Methods

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract"/>.
        /// </summary>
        /// <param name="type">
        /// The type to create properties for.
        /// </param>
        /// <param name="memberSerialization">
        /// The member serialization mode for the type.
        /// </param>
        /// ///
        /// <returns>
        /// Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract"/>.
        /// </returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            var filteredProperties =
                properties.Where(p => p.AttributeProvider.GetAttributes(true).All(a => !(a is AuditExcludeAttribute)));

            return filteredProperties.ToList();
        }

        #endregion
    }
}