// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMappingResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The ValueConverter interface.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// The convert value.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="sourceValue">
        /// The source value.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object ConvertValue(Type sourceType, object sourceValue, Type destinationType);

        /// <summary>
        /// The should convert value.
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
        bool ShouldConvertValue(Type sourceType, Type destinationType);
    }

    /// <summary>
    ///     The type mapping resolver.
    /// </summary>
    public class TypeMappingResolver : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMappingResolver"/> class.
        /// </summary>
        /// <param name="typeMappings">
        /// The type mappings.
        /// </param>
        public TypeMappingResolver(IEnumerable<TypeMapping> typeMappings)
        {
            this.TypeMappings = typeMappings;
        }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public PropertyResolverType Type { get; } = PropertyResolverType.SourceProperty;

        /// <summary>
        ///     Gets or sets the type mappings.
        /// </summary>
        protected IEnumerable<TypeMapping> TypeMappings { get; set; }

        /// <summary>
        /// The convert value.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="sourceValue">
        /// The source value.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertValue(Type sourceType, object sourceValue, Type destinationType)
        {
            var mapper = this.TypeMappings.FirstOrDefault(
                mapping => mapping.CanResolveType(sourceType, destinationType));
            return mapper?.Map(sourceValue);
        }

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
        public object MapProperty(PropertyInfo sourceProperty, object sourceValue, PropertyInfo destinationProperty)
        {
            var sourceEnumerableType = (from iface in sourceProperty.PropertyType.GetInterfaces()
                                        where
                                            iface.IsGenericType
                                            && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                        select iface.GetGenericArguments()[0]).FirstOrDefault();
            var destinationEnumerableType = (from iface in destinationProperty.PropertyType.GetInterfaces()
                                             where
                                                 iface.IsGenericType
                                                 && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                             select iface.GetGenericArguments()[0]).FirstOrDefault();
            if (sourceEnumerableType != null && destinationEnumerableType != null)
            {
                var list = sourceProperty.GetValue(sourceValue) as IEnumerable;
                if (list == null)
                {
                    return null;
                }

                var viewList = typeof(List<>);
                var viewListGeneric = viewList.MakeGenericType(destinationEnumerableType);
                var viewInstance = Activator.CreateInstance(viewListGeneric) as IList;

                foreach (var item in list)
                {
                    var mapping =
                        this.TypeMappings.FirstOrDefault(
                            m => m.CanResolveType(sourceEnumerableType, destinationEnumerableType));
                    var result = mapping?.Map(item);
                    if (result == null)
                    {
                        continue;
                    }

                    viewInstance?.Add(result);
                }

                return viewInstance;
            }

            var mapping2 =
                this.TypeMappings.FirstOrDefault(
                    m => m.CanResolveType(sourceProperty.PropertyType, destinationProperty.PropertyType));
            return mapping2?.Map(sourceProperty.GetValue(sourceValue));
        }

        /// <summary>
        /// The should convert value.
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
        public bool ShouldConvertValue(Type sourceType, Type destinationType)
        {
            return this.TypeMappings.Any(mapping => mapping.CanResolveType(sourceType, destinationType));
        }

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
        public bool ShouldMapProperty(PropertyInfo sourceProperty, object sourceValue, PropertyInfo destinationProperty)
        {
            var directMatch =
                this.TypeMappings.Select(
                    mapping => mapping.CanResolveType(sourceProperty.PropertyType, destinationProperty.PropertyType))
                    .Any(result => result);
            if (directMatch)
            {
                return true;
            }

            var sourceEnumerableType = (from iface in sourceProperty.PropertyType.GetInterfaces()
                                        where
                                            iface.IsGenericType
                                            && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                        select iface.GetGenericArguments()[0]).FirstOrDefault();
            var destinationEnumerableType = (from iface in destinationProperty.PropertyType.GetInterfaces()
                                             where
                                                 iface.IsGenericType
                                                 && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                             select iface.GetGenericArguments()[0]).FirstOrDefault();
            if (sourceEnumerableType != null && destinationEnumerableType != null)
            {
                return
                    this.TypeMappings.Select(
                        mapping => mapping.CanResolveType(sourceEnumerableType, destinationEnumerableType))
                        .Any(result => result);
            }

            return false;
        }
    }
}