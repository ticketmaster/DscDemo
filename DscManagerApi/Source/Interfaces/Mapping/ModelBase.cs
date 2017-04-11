// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The model base.
    /// </summary>
    /// <typeparam name="TDefaultView">
    /// </typeparam>
    public class ModelBase<TDefaultView> : IModel<TDefaultView>
        where TDefaultView : class, IViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBase{TDefaultView}" /> class.
        /// </summary>
        protected ModelBase()
        {
            this.TypeMappings.Add(new TypeMapping(this.GetType(), typeof(TDefaultView)));
        }

        /// <summary>
        ///     Gets or sets the type mappings.
        /// </summary>
        protected ICollection<TypeMapping> TypeMappings { get; set; } = new List<TypeMapping>();

        /// <summary>
        /// The add type mapping.
        /// </summary>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="TypeMapping"/>.
        /// </returns>
        public TypeMapping AddTypeMapping(Type destinationType)
        {
            var mapping = new TypeMapping(this.GetType(), destinationType);
            this.TypeMappings.Add(mapping);
            return mapping;
        }

        /// <summary>
        ///     The add type mapping.
        /// </summary>
        /// <typeparam name="TDestination">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TypeMapping" />.
        /// </returns>
        public TypeMapping AddTypeMapping<TDestination>()
        {
            return this.AddTypeMapping(typeof(TDestination));
        }

        /// <summary>
        ///     The map.
        /// </summary>
        /// <typeparam name="TDestination">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TDestination" />.
        /// </returns>
        public TDestination Map<TDestination>() where TDestination : class
        {
            var mapping =
                this.TypeMappings.FirstOrDefault(
                    m => m.SourceType == this.GetType() && m.DestinationType == typeof(TDestination));

            return mapping?.Map(this) as TDestination;
        }

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Map(Type destinationType)
        {
            var mapping =
                this.TypeMappings.FirstOrDefault(
                    m => m.SourceType == this.GetType() && m.DestinationType == destinationType);

            return mapping?.Map(this);
        }

        /// <summary>
        ///     The to view model.
        /// </summary>
        /// <returns>
        ///     The <see cref="TDefaultView" />.
        /// </returns>
        public TDefaultView ToViewModel()
        {
            return this.Map<TDefaultView>();
        }
    }
}