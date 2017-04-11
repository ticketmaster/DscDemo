// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildEventArgs.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer;

    /// <summary>
    ///     The build event args.
    /// </summary>
    public class BuildEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        public Build Build { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        public BuildStatus Status { get; set; }
    }
}