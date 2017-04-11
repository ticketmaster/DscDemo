// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnvironmentProperties.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces
{
    /// <summary>
    /// The EnvironmentProperties interface.
    /// </summary>
    public interface IEnvironmentProperties
    {
        /// <summary>
        /// Gets a value indicating whether debug build.
        /// </summary>
        bool DebugBuild { get; }

        /// <summary>
        /// Gets a value indicating whether debugger attached.
        /// </summary>
        bool DebuggerAttached { get; }
    }
}