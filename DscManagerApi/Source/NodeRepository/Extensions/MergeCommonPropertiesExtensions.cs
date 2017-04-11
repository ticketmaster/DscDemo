// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeCommonPropertiesExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    ///     The merge common properties extensions.
    /// </summary>
    public static class MergeCommonPropertiesExtensions
    {
        /// <summary>
        /// The get default.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// The merge common properties.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TDest">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TDest"/>.
        /// </returns>
        public static TDest MergeCommonProperties<TSource, TDest>(this TSource source, TDest destination)
            where TSource : class where TDest : class
        {
            var properties = source.GetType().GetProperties();
            foreach (var property in properties.Where(p => !p.PropertyType.IsGenericType))
            {
                var value = property.GetValue(source);
                var destProperty = destination.GetType().GetProperty(property.Name);
                if (destProperty == null)
                {
                    continue;
                }

                var destValue = destProperty.GetValue(destination);

                if (value != destValue && value != null
                    && (value.GetType() != typeof(DateTime)
                        || (value is DateTime && (DateTime)value != DateTime.MinValue))
                    && value != GetDefault(source.GetType()))
                {
                    destProperty.SetValue(destination, value, null);
                }
            }

            return destination;
        }
    }
}