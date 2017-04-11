// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelFactory.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Mapping
{
    using System;

    using Autofac;

    /// <summary>
    ///     The view model factory.
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public ViewModelFactory(IComponentContext container)
        {
            this.Container = container;
            Instance = this;
        }

        /// <summary>
        ///     Gets or sets the instance.
        /// </summary>
        public static IViewModelFactory Instance { get; set; }

        /// <summary>
        ///     Gets or sets the container.
        /// </summary>
        protected IComponentContext Container { get; set; }

        /// <summary>
        ///     The construct view model.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T ConstructViewModel<T>() where T : class, IViewModel
        {
            return this.ConstructViewModel(typeof(T)) as T;
        }

        /// <summary>
        /// The construct view model.
        /// </summary>
        /// <param name="viewModelType">
        /// The view model type.
        /// </param>
        /// <returns>
        /// The <see cref="IViewModel"/>.
        /// </returns>
        public virtual IViewModel ConstructViewModel(Type viewModelType)
        {
            return this.Container.Resolve(viewModelType) as IViewModel;
        }
    }
}