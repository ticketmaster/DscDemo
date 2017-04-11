// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels
{
    using System;

    /// <summary>
    ///     The job view.
    /// </summary>
    public class JobView
    {
        /// <summary>
        /// Gets or sets the complete timestamp.
        /// </summary>
        public DateTime CompleteTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the message detailed.
        /// </summary>
        public string MessageDetailed { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        public string Status { get; set; }
    }
}