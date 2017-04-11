// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildStatus.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer
{
    /// <summary>
    ///     The build status.
    /// </summary>
    public enum BuildStatus
    {
        /// <summary>
        ///     The not submitted.
        /// </summary>
        NotSubmitted, 

        /// <summary>
        ///     The enqueued.
        /// </summary>
        Enqueued, 

        /// <summary>
        ///     The in progress.
        /// </summary>
        InProgress, 

        /// <summary>
        ///     The failed.
        /// </summary>
        Failed, 

        /// <summary>
        ///     The success.
        /// </summary>
        Succeeded, 

        /// <summary>
        ///     The partial failure.
        /// </summary>
        PartialFailure, 

        /// <summary>
        ///     The unknown.
        /// </summary>
        Unknown
    }
}