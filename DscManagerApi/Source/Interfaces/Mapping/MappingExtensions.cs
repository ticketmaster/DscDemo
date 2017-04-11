// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    ///     The mapping extensions.
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// The add property resolver.
        /// </summary>
        /// <param name="mappings">
        /// The mappings.
        /// </param>
        /// <param name="propertyResolver">
        /// The property resolver.
        /// </param>
        public static void AddPropertyResolver(
            this IEnumerable<TypeMapping> mappings, 
            IPropertyResolver propertyResolver)
        {
            foreach (var map in mappings)
            {
                map.PropertyResolvers.Add(propertyResolver);
            }
        }

        /// <summary>
        /// The add source member property resolver.
        /// </summary>
        /// <param name="mappings">
        /// The mappings.
        /// </param>
        /// <param name="sourceMember">
        /// The source member.
        /// </param>
        /// <param name="mappedValue">
        /// The mapped value.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        public static void AddSourceMemberPropertyResolver<TSource>(
            this IEnumerable<TypeMapping> mappings, 
            Expression<Func<TSource, object>> sourceMember, 
            Func<TSource, object> mappedValue) where TSource : class
        {
            foreach (var map in mappings)
            {
                map.PropertyResolvers.Add(new SourceMemberPropertyResolver<TSource>(sourceMember, mappedValue));
            }
        }

        /// <summary>
        /// The add source member property resolver.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <param name="sourceMember">
        /// The source member.
        /// </param>
        /// <param name="mappedValue">
        /// The mapped value.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        public static void AddSourceMemberPropertyResolver<TSource>(
            this TypeMapping mapping, 
            Expression<Func<TSource, object>> sourceMember, 
            Func<TSource, object> mappedValue) where TSource : class
        {
            mapping.PropertyResolvers.Add(new SourceMemberPropertyResolver<TSource>(sourceMember, mappedValue));
        }
    }
}