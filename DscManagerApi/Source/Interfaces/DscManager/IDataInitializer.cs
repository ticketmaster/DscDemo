// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataInitializer.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DscManager
{
    /// <summary>
    /// The DataInitializer interface.
    /// </summary>
    public interface IDataInitializer
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// The should run initializer.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ShouldRunInitializer();
    }
}