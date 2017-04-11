// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System.Reflection;

    /// <summary>
    ///     The PropertyResolver interface.
    /// </summary>
    public interface IPropertyResolver
    {
        /// <summary>
        ///     Gets the type.
        /// </summary>
        PropertyResolverType Type { get; }

        /// <summary>
        /// The map property.
        /// </summary>
        /// <param name="sourceProperty">
        /// The source property.
        /// </param>
        /// <param name="sourceValue">
        /// The source value.
        /// </param>
        /// <param name="destinationProperty">
        /// The destination property.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object MapProperty(PropertyInfo sourceProperty, object sourceValue, PropertyInfo destinationProperty);

        /// <summary>
        /// The should map property.
        /// </summary>
        /// <param name="sourceProperty">
        /// The source property.
        /// </param>
        /// <param name="sourceValue">
        /// The source value.
        /// </param>
        /// <param name="destinationProperty">
        /// The destination property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ShouldMapProperty(PropertyInfo sourceProperty, object sourceValue, PropertyInfo destinationProperty);
    }
}