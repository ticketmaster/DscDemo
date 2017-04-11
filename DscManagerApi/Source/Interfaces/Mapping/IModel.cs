// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModel.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;

    /// <summary>
    /// The Model interface.
    /// </summary>
    /// <typeparam name="TDefaultViewModel">
    /// </typeparam>
    public interface IModel<out TDefaultViewModel>
        where TDefaultViewModel : IViewModel
    {
        /// <summary>
        ///     The map.
        /// </summary>
        /// <typeparam name="TDestination">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TDestination" />.
        /// </returns>
        TDestination Map<TDestination>() where TDestination : class;

        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Map(Type destinationType);

        /// <summary>
        ///     The to view model.
        /// </summary>
        /// <returns>
        ///     The <see cref="TDefaultViewModel" />.
        /// </returns>
        TDefaultViewModel ToViewModel();
    }
}