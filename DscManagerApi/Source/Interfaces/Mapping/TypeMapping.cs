// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapping.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The type mapping.
    /// </summary>
    public class TypeMapping : ITypeMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapping"/> class.
        /// </summary>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        public TypeMapping(Type sourceType, Type destinationType)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;

            this.SetStandardPropertyResolvers();
        }

        /// <summary>
        ///     Gets or sets the destination type.
        /// </summary>
        public Type DestinationType { get; set; }

        /// <summary>
        ///     Gets or sets the property resolvers.
        /// </summary>
        public ICollection<IPropertyResolver> PropertyResolvers { get; set; } = new List<IPropertyResolver>();

        /// <summary>
        ///     Gets or sets the source type.
        /// </summary>
        public Type SourceType { get; set; }

        /// <summary>
        ///     Gets or sets the type mappings.
        /// </summary>
        public ICollection<TypeMapping> TypeMappings { get; protected set; } = new List<TypeMapping>();

        /// <summary>
        ///     Gets or sets the standard property resolvers.
        /// </summary>
        public ICollection<IValueConverter> ValueConverters { get; set; } = new List<IValueConverter>();

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
        public TypeMapping AddTypeMapping<TSource, TDestination>()
        {
            return this.AddTypeMapping(typeof(TSource), typeof(TDestination));
        }

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
        public TypeMapping AddTypeMapping(Type sourceType, Type destinationType)
        {
            var mapping = new TypeMapping(sourceType, destinationType);
            this.TypeMappings.Add(mapping);
            return mapping;
        }

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
        public bool CanResolveType(Type sourceType, Type destinationType)
        {
            var sourceMatch = this.SourceType.IsAssignableFrom(sourceType);
            return sourceMatch && this.DestinationType.IsAssignableFrom(destinationType);
        }

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Map(object source)
        {
            var destination = typeof(IViewModel).IsAssignableFrom(this.DestinationType)
                                  ? ViewModelFactory.Instance.ConstructViewModel(this.DestinationType)
                                  : Activator.CreateInstance(this.DestinationType);
            var properties = source?.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var destinationProperties = destination.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var destPropertiesSet = new List<string>();
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    var destinationProp = destinationProperties.FirstOrDefault(p => p.Name == property.Name);
                    if (destinationProp == null)
                    {
                        continue;
                    }

                    object value = null;
                    var hasMatched = false;
                    var propValue = property.GetValue(source);
                    foreach (var expression in
                        this.PropertyResolvers.Where(r => r.Type == PropertyResolverType.SourceProperty))
                    {
                        if (expression.ShouldMapProperty(property, propValue, destinationProp))
                        {
                            value = expression.MapProperty(property, source, destinationProp);
                            hasMatched = true;
                        }
                    }

                    if (!hasMatched)
                    {
                        if (
                            this.PropertyResolvers.Where(r => r.Type == PropertyResolverType.DestinationProperty)
                                .Any(r => r.ShouldMapProperty(property, value, destinationProp)))
                        {
                            continue;
                        }

                        value = property.GetValue(source);
                    }

                    if (this.SetPropertyValue(value, destinationProp, destination))
                    {
                        destPropertiesSet.Add(destinationProp.Name);
                    }
                }
            }

            foreach (var destinationProperty in destinationProperties)
            {
                if (destPropertiesSet.Contains(destinationProperty.Name))
                {
                    continue;
                }

                object value = null;
                var sourceProp = properties?.FirstOrDefault(p => p.Name == destinationProperty.Name);
                foreach (var resolver in
                    this.PropertyResolvers.Where(r => r.Type == PropertyResolverType.DestinationProperty))
                {
                    if (resolver.ShouldMapProperty(sourceProp, source, destinationProperty))
                    {
                        value = resolver.MapProperty(sourceProp, source, destinationProperty);
                    }
                }

                this.SetPropertyValue(value, destinationProperty, destination);
            }

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(destination, new ValidationContext(destination), validationResults))
            {
                return null;
            }

            if (typeof(IViewModel).IsAssignableFrom(this.DestinationType))
            {
                var destView = (IViewModel)destination;
                destView.PopulateLinks();
            }

            return destination;
        }

        /// <summary>
        /// The get enumerable type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        private Type GetEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GenericTypeArguments.FirstOrDefault();
            }

            return (from iface in type.GetInterfaces()
                    where iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select iface.GetGenericArguments()[0]).FirstOrDefault();
        }

        /// <summary>
        /// The set property value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="destinationProperty">
        /// The destination property.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SetPropertyValue(object value, PropertyInfo destinationProperty, object destination)
        {
            if (value == null)
            {
                return false;
            }

            var sourcePropertyType = value.GetType();

            try
            {
                var sourceEnumerableType = this.GetEnumerableType(sourcePropertyType);

                var destinationEnumerableType = this.GetEnumerableType(destinationProperty.PropertyType);

                if (sourceEnumerableType == null && destinationEnumerableType == null)
                {
                    var converter =
                        this.ValueConverters.FirstOrDefault(
                            v => v.ShouldConvertValue(sourcePropertyType, destinationProperty.PropertyType));
                    if (converter != null)
                    {
                        value = converter.ConvertValue(sourcePropertyType, value, destinationProperty.PropertyType);
                    }
                }

                if (sourceEnumerableType != null && destinationEnumerableType != null)
                {
                    var converter =
                        this.ValueConverters.FirstOrDefault(
                            v => v.ShouldConvertValue(sourceEnumerableType, destinationEnumerableType));
                    if (converter != null)
                    {
                        var list = value as IEnumerable;
                        if (list == null)
                        {
                            return false;
                        }

                        var viewList = typeof(List<>);
                        var viewListGeneric = viewList.MakeGenericType(destinationEnumerableType);
                        var viewInstance = Activator.CreateInstance(viewListGeneric) as IList;

                        foreach (var item in list)
                        {
                            viewInstance?.Add(
                                converter.ConvertValue(sourceEnumerableType, item, destinationEnumerableType));
                        }

                        value = viewInstance;
                    }
                }

                var vType = value?.GetType();
                if (value == null || !destinationProperty.PropertyType.IsAssignableFrom(vType))
                {
                    if (vType == typeof(long) && destinationProperty.PropertyType == typeof(int))
                    {
                        value = Convert.ToInt32(value);
                    }
                    else
                    {
                        return false;
                    }
                }

                destinationProperty.SetValue(destination, value);

                if (typeof(IViewModel).IsAssignableFrom(destinationProperty.PropertyType))
                {
                    var destViewModel = value as IViewModel;
                    destViewModel?.PopulateLinks();
                }

                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        /// <summary>
        ///     The set standard property resolvers.
        /// </summary>
        private void SetStandardPropertyResolvers()
        {
            this.ValueConverters.Add(new ModelPropertyResolver());
            this.ValueConverters.Add(new TypeMappingResolver(this.TypeMappings));
        }
    }

    /// <summary>
    /// The type mapping.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TDestination">
    /// </typeparam>
    public class TypeMapping<TSource, TDestination> : TypeMapping
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMapping{TSource,TDestination}" /> class.
        ///     Initializes a new instance of the <see cref="TypeMapping" /> class.
        /// </summary>
        public TypeMapping()
            : base(typeof(TSource), typeof(TDestination))
        {
        }
    }
}