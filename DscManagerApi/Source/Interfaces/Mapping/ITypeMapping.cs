// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMapping.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The TypeMapping interface.
    /// </summary>
    public interface ITypeMapping
    {
        /// <summary>
        ///     Gets or sets the destination type.
        /// </summary>
        Type DestinationType { get; set; }

        /// <summary>
        ///     Gets or sets the property resolvers.
        /// </summary>
        ICollection<IPropertyResolver> PropertyResolvers { get; set; }

        /// <summary>
        ///     Gets or sets the source type.
        /// </summary>
        Type SourceType { get; set; }

        /// <summary>
        ///     Gets the type mappings.
        /// </summary>
        ICollection<TypeMapping> TypeMappings { get; }

        /// <summary>
        ///     Gets the standard property resolvers.
        /// </summary>
        ICollection<IValueConverter> ValueConverters { get; }

        /// <summary>
        ///     The add type mapping.
        /// </summary>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TDestination">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TypeMapping" />.
        /// </returns>
        TypeMapping AddTypeMapping<TSource, TDestination>();

        /// <summary>
        /// The add type mapping.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="TypeMapping"/>.
        /// </returns>
        TypeMapping AddTypeMapping(Type sourceType, Type destinationType);

        /// <summary>
        /// The can resolve type.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CanResolveType(Type sourceType, Type destinationType);

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Map(object source);
    }
}