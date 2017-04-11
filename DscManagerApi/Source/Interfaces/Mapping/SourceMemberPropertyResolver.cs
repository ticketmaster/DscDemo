// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceMemberPropertyResolver.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The source member property resolver.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    public class SourceMemberPropertyResolver<TSource> : IPropertyResolver
        where TSource : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceMemberPropertyResolver{TSource}"/> class.
        /// </summary>
        /// <param name="sourceMember">
        /// The source member.
        /// </param>
        /// <param name="mappedValue">
        /// The mapped value.
        /// </param>
        public SourceMemberPropertyResolver(
            Expression<Func<TSource, object>> sourceMember, 
            Func<TSource, object> mappedValue)
        {
            MemberExpression member = null;
            if (sourceMember.Body.NodeType == ExpressionType.Convert)
            {
                var body = sourceMember.Body as UnaryExpression;
                member = body?.Operand as MemberExpression;
            }
            else
            {
                member = sourceMember.Body as MemberExpression;
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
        public Func<TSource, object> ResolveFunction { get; protected set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public PropertyResolverType Type { get; } = PropertyResolverType.SourceProperty;

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
            var castSource = sourceValue as TSource;
            return castSource == null ? null : this.ResolveFunction.Invoke(castSource);
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
            return sourceProperty.Name == this.PropertyName;
        }
    }
}