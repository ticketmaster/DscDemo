// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeConfigurationService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;

    /// <summary>
    ///     The NodeConfigurationService interface.
    /// </summary>
    public interface INodeConfigurationService
    {
        Task<BuildDetailView> CreateBuildJob(string certificateThumbprint);

        void BuildNode(int buildId, Dictionary<PropertyScope, ICollection<string>> collection);
        /// <summary>
        /// The build node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<BuildDetailView> BuildNode(NodeDetailView node);

        /// <summary>
        /// The build node.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<BuildDetailView> BuildNode(IEnumerable<NodeDetailView> nodes);

        /// <summary>
        /// The create node.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<NodeDetailResult> CreateNode(NodeRequest nodeRequest);

        /// <summary>
        /// The create node.
        /// </summary>
        /// <param name="nodeRequests">
        /// The node requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IEnumerable<NodeDetailResult>> CreateNode(IEnumerable<NodeRequest> nodeRequests);

        /// <summary>
        /// The delete node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> DeleteNode(string nodeName);

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        Hashtable GetConfigurationData(IEnumerable<NodeDetailView> nodes);

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="nodes">
        /// The nodes.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        Hashtable GetConfigurationData(IEnumerable<NodeRequest> nodes);

        /// <summary>
        /// The get configuration data.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        Hashtable GetConfigurationData(NodeDetailView node);

        /// <summary>
        /// The set maintenance for node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<NodeView> SetMaintenanceForNode(string nodeName, bool isInMaintenance);

        /// <summary>
        /// The set maintenance for node.
        /// </summary>
        /// <param name="nodeNames">
        /// The node names.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IEnumerable<NodeView>> SetMaintenanceForNode(IEnumerable<string> nodeNames, bool isInMaintenance);

        /// <summary>
        /// The update node.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<NodeDetailResult> UpdateNode(NodeRequest nodeRequest);

        /// <summary>
        /// The update node.
        /// </summary>
        /// <param name="nodeRequests">
        /// The node requests.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IEnumerable<NodeDetailResult>> UpdateNode(IEnumerable<NodeRequest> nodeRequests);
    }
}