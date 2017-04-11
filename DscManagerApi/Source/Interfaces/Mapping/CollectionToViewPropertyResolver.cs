// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionToViewPropertyResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The collection to view property resolver.
    /// </summary>
    /// <typeparam name="TEnumerable">
    /// </typeparam>
    public class CollectionToViewPropertyResolver<TEnumerable> : IPropertyResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToViewPropertyResolver{TEnumerable}"/> class.
        /// </summary>
        /// <param name="enumerableName">
        /// The enumerable name.
        /// </param>
        /// <param name="enumerableValue">
        /// The enumerable value.
        /// </param>
        public CollectionToViewPropertyResolver(
            Expression<Func<TEnumerable, object>> enumerableName, 
            Expression<Func<TEnumerable, object>> enumerableValue)
        {
            var member = enumerableName.Body as MemberExpression;
            if (member?.Member is PropertyInfo)
            {
                this.EnumerableName = member.Member.Name;
            }

            var member2 = enumerableValue.Body as MemberExpression;
            if (member2?.Member is PropertyInfo)
            {
                this.EnumerableValue = member2.Member.Name;
            }
        }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public PropertyResolverType Type { get; } = PropertyResolverType.DestinationProperty;

        /// <summary>
        ///     Gets or sets the enumerable name.
        /// </summary>
        protected string EnumerableName { get; set; }

        /// <summary>
        ///     Gets or sets the enumerable value.
        /// </summary>
        protected string EnumerableValue { get; set; }

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
            var value = sourceValue as IEnumerable<TEnumerable>;
            var nameProperty = typeof(TEnumerable).GetProperty(
                this.EnumerableName, 
                BindingFlags.Instance | BindingFlags.Public);
            var valueProperty = typeof(TEnumerable).GetProperty(
                this.EnumerableValue, 
                BindingFlags.Instance | BindingFlags.Public);
            if (value != null && nameProperty != null)
            {
                foreach (var item in value)
                {
                    var itemValue = nameProperty.GetValue(item) as string;
                    if (itemValue != null && itemValue == destinationProperty.Name)
                    {
                        return valueProperty.GetValue(item);
                    }
                }
            }

            return null;
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
            var value = sourceValue as IEnumerable<TEnumerable>;
            var nameProperty = typeof(TEnumerable).GetProperty(
                this.EnumerableName, 
                BindingFlags.Instance | BindingFlags.Public);
            if (value != null && nameProperty != null)
            {
                foreach (var item in value)
                {
                    var itemValue = nameProperty.GetValue(item) as string;
                    if (itemValue != null && itemValue == destinationProperty.Name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}