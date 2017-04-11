// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildDetailView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels
{
    using System.Collections.Generic;
    using System.Web.Http.Routing;

    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The build detail view.
    /// </summary>
    public class BuildDetailView : BuildView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDetailView"/> class.
        /// </summary>
        /// <param name="urlHelper">
        /// The url helper.
        /// </param>
        public BuildDetailView(UrlHelper urlHelper)
            : base(urlHelper)
        {
        }

        /// <summary>
        ///     Gets or sets the jobs.
        /// </summary>
        public ICollection<JobView> Jobs { get; set; } = new List<JobView>();

        /// <summary>
        ///     Gets or sets the targets.
        /// </summary>
        public ICollection<BuildTargetView> Targets { get; set; } = new List<BuildTargetView>();

        public int SubmissionJobId { get; set; }

        public JobView SubmissionJob { get; set; }

        /// <summary>
        ///     The populate links.
        /// </summary>
        public override void PopulateLinks()
        {
            var links = new List<Link>();

            if (this.Status == BuildStatus.Enqueued || this.Status == BuildStatus.InProgress)
            {
                links.Add(
                    new Link(
                        "Cancel Build", 
                        this.UrlHelper.Link("BuildsCancel", new Dictionary<string, object> { { "id", this.Id } }), 
                        HttpMethod.Get));
            }

            this.Links = links;
        }
    }
}