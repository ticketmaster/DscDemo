// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelPropertyResolver.cs" company="Ticketmaster">
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
    ///     The model property resolver.
    /// </summary>
    public class ModelPropertyResolver : IValueConverter
    {
        /// <summary>
        ///     Gets the type.
        /// </summary>
        public PropertyResolverType Type { get; } = PropertyResolverType.SourceProperty;

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
            var source = sourceValue as IModel<IViewModel>;
            return source?.Map(destinationType);
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
            var sourceEnumerableType = sourceProperty.PropertyType.Name == "IEnumerable`1"
                                           ? sourceProperty.PropertyType.GetGenericArguments()[0]
                                           : (from iface in sourceProperty.PropertyType.GetInterfaces()
                                              where
                                                  iface.IsGenericType
                                                  && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                              select iface.GetGenericArguments()[0]).FirstOrDefault();
            var destinationEnumerableType = destinationProperty.PropertyType.Name == "IEnumerable`1"
                                                ? destinationProperty.PropertyType.GetGenericArguments()[0]
                                                : (from iface in destinationProperty.PropertyType.GetInterfaces()
                                                   where
                                                       iface.IsGenericType
                                                       && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                                   select iface.GetGenericArguments()[0]).FirstOrDefault();
            if (sourceEnumerableType != null && typeof(IModel<IViewModel>).IsAssignableFrom(sourceEnumerableType)
                && destinationEnumerableType != null && typeof(IViewModel).IsAssignableFrom(destinationEnumerableType))
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
                    var mapping = new TypeMapping(sourceProperty.PropertyType, destinationEnumerableType);
                    var result = mapping.Map(item);
                    viewInstance?.Add(result);
                }

                return viewInstance;
            }

            var viewModelType = (from iface in sourceProperty.PropertyType.GetInterfaces()
                                 where iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IModel<>)
                                 select iface.GetGenericArguments()[0]).FirstOrDefault();
            var mapping2 = new TypeMapping(sourceProperty.PropertyType, viewModelType);
            return mapping2.Map(sourceProperty.GetValue(sourceValue));
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
            var isSourceModel =
                sourceType.GetInterfaces()
                    .Any(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IModel<>));
            var isDestViewModel = typeof(IViewModel).IsAssignableFrom(destinationType);

            return isSourceModel && isDestViewModel;
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
                sourceProperty.PropertyType.GetInterfaces()
                    .Any(iface => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IModel<>));
            if (directMatch)
            {
                return true;
            }

            var sourceEnumerableType = sourceProperty.PropertyType.Name == "IEnumerable`1"
                                           ? sourceProperty.PropertyType.GetGenericArguments()[0]
                                           : (from iface in sourceProperty.PropertyType.GetInterfaces()
                                              where
                                                  iface.IsGenericType
                                                  && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                              select iface.GetGenericArguments()[0]).FirstOrDefault();
            var destinationEnumerableType = destinationProperty.PropertyType.Name == "IEnumerable`1"
                                                ? destinationProperty.PropertyType.GetGenericArguments()[0]
                                                : (from iface in destinationProperty.PropertyType.GetInterfaces()
                                                   where
                                                       iface.IsGenericType
                                                       && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                                   select iface.GetGenericArguments()[0]).FirstOrDefault();
            if (sourceEnumerableType != null && typeof(IModel<IViewModel>).IsAssignableFrom(sourceEnumerableType)
                && destinationEnumerableType != null && typeof(IViewModel).IsAssignableFrom(destinationEnumerableType))
            {
                return true;
            }

            return false;
        }
    }
}