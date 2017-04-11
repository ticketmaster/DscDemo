// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelFactory.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;

    /// <summary>
    ///     The ViewModelFactory interface.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        ///     The construct view model.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        T ConstructViewModel<T>() where T : class, IViewModel;

        /// <summary>
        /// The construct view model.
        /// </summary>
        /// <param name="viewModelType">
        /// The view model type.
        /// </param>
        /// <returns>
        /// The <see cref="IViewModel"/>.
        /// </returns>
        IViewModel ConstructViewModel(Type viewModelType);
    }
}