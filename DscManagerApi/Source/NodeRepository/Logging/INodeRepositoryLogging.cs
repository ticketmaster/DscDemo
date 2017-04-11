// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeRepositoryLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Logging
{
    using System.Collections;
    using System.Collections.Generic;

    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;

    /// <summary>
    ///     The NodeRepositoryLogging interface.
    /// </summary>
    public interface INodeRepositoryLogging
    {
        /// <summary>
        /// The begin database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void BeginNodeRepositoryDatabaseSeed(string name);

        /// <summary>
        ///     The bootstrap options not present.
        /// </summary>
        void BootstrapOptionsNotPresent();

        /// <summary>
        /// The bootstrap update received.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <param name="nodeData">
        /// The node data.
        /// </param>
        void BootstrapUpdateReceived(string nodeName, Hashtable nodeData);

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        void BuildRequestedNoBuildService(NodeRequest nodeRequest);

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        void BuildRequestedNoBuildService(IEnumerable<NodeDetailView> nodeRequest);

        /// <summary>
        /// The delete node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        void DeleteNode(string nodeName);

        /// <summary>
        /// The end database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void EndNodeRepositoryDatabaseSeed(string name);

        /// <summary>
        /// The node agent error.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        void NodeAgentError(string name, string errorMessage);

        /// <summary>
        /// The node changed initial deployment.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        void NodeChangedInitialDeployment(string name, bool status);

        /// <summary>
        /// The node changed is in maintenance.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        void NodeChangedIsInMaintenance(string name, bool isInMaintenance);

        /// <summary>
        /// The node created.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        void NodeCreated(string nodeName);

        /// <summary>
        /// The node updated.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        void NodeUpdated(string nodeName);

        /// <summary>
        /// The reference outside web context.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        void ReferenceOutsideWebContext(string name, string source);

        void UpgradeNodeRepositoryDatabaseSchema();
    }
}