// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DestinationMemberPropertyResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The destination member property resolver.
    /// </summary>
    /// <typeparam name="TDestination">
    /// </typeparam>
    public class DestinationMemberPropertyResolver<TDestination> : IPropertyResolver
        where TDestination : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationMemberPropertyResolver{TDestination}"/> class.
        /// </summary>
        /// <param name="destinationMember">
        /// The destination member.
        /// </param>
        /// <param name="mappedValue">
        /// The mapped value.
        /// </param>
        public DestinationMemberPropertyResolver(
            Expression<Func<TDestination, object>> destinationMember, 
            Func<object> mappedValue)
        {
            MemberExpression member = null;
            if (destinationMember.Body.NodeType == ExpressionType.Convert)
            {
                var body = destinationMember.Body as UnaryExpression;
                member = body?.Operand as MemberExpression;
            }
            else
            {
                member = destinationMember.Body as MemberExpression;
            }

            if (member?.Member is PropertyInfo)
            {
                this.PropertyName = member.Member.Name;
            }

            this.ResolveFunction = mappedValue;
        }

        /// <summary>
        ///     Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; protected set; }

        /// <summary>
        ///     Gets or sets the resolve function.
        /// </summary>
        public Func<object> ResolveFunction { get; protected set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public PropertyResolverType Type { get; } = PropertyResolverType.DestinationProperty;

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
            return this.ResolveFunction.Invoke();
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
            return destinationProperty.Name == this.PropertyName;
        }
    }
}