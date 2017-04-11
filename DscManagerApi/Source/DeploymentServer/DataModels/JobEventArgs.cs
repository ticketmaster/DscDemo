// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobEventArgs.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Management.Automation;

    using Ticketmaster.Common.PowerShellRunner.Models;

    /// <summary>
    ///     The job event args.
    /// </summary>
    public class JobEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets or sets the build.
        /// </summary>
        public Build Build { get; set; }

        /// <summary>
        ///     Gets or sets the invocation exception.
        /// </summary>
        public Exception InvocationException { get; set; }

        /// <summary>
        ///     Gets or sets the job ids.
        /// </summary>
        public IEnumerable<int> JobIds { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        public PowerShellResult<PSObject> Result { get; set; }

        /// <summary>
        ///     Gets or sets the working path.
        /// </summary>
        public string WorkingPath { get; set; }
    }
}