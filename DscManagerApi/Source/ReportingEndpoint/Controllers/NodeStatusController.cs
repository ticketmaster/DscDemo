// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeStatusController.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Controllers
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Views;
    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The node status controller.
    /// </summary>
    [RoutePrefix("nodeStatus")]
    public class NodeStatusController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeStatusController"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public NodeStatusController(ReportingEndpointContext context)
        {
            this.Context = context;
            this.NodeStatusRepository = context.Set<NodeStatus>();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected ReportingEndpointContext Context { get; set; }

        /// <summary>
        /// Gets or sets the node status repository.
        /// </summary>
        protected DbSet<NodeStatus> NodeStatusRepository { get; set; }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{target}")]
        public async Task<IHttpActionResult> Get(string target)
        {
            var result = await this.NodeStatusRepository.FirstOrDefaultAsync(s => s.Target == target);
            if (result == null)
            {
                return this.NotFound();
            }

            return this.Ok(result.ToViewModel());
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="resultSize">
        /// The result size.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route]
        public IHttpActionResult GetAll(ODataQueryOptions<NodeStatus> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var results =
                ((IQueryable<NodeStatus>)options.ApplyTo(this.NodeStatusRepository, settings)).Select(
                    s => s.ToViewModel());
            return this.Ok(new PagedResult<NodeStatusView>(results, options, resultSize));
        }
    }
}