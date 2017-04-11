// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRepositoryLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.Logging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;

    /// <summary>
    ///     The node repository logging.
    /// </summary>
    public class NodeRepositoryLogging : INodeRepositoryLogging
    {
        /// <summary>
        /// The begin database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void BeginNodeRepositoryDatabaseSeed(string name)
        {
        }

        /// <summary>
        ///     The bootstrap options not present.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void BootstrapOptionsNotPresent()
        {
        }

        /// <summary>
        /// The bootstrap update received.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        /// <param name="nodeData">
        /// The node data.
        /// </param>
        public void BootstrapUpdateReceived(string nodeName, Hashtable nodeData)
        {
        }

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        public void BuildRequestedNoBuildService(NodeRequest nodeRequest)
        {
        }

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        public void BuildRequestedNoBuildService(IEnumerable<NodeDetailView> nodeRequest)
        {
        }

        /// <summary>
        /// The delete node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void DeleteNode(string nodeName)
        {
        }

        /// <summary>
        /// The end database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void EndNodeRepositoryDatabaseSeed(string name)
        {
        }

        /// <summary>
        /// The node agent error.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        public void NodeAgentError(string name, string errorMessage)
        {
            
        }

        /// <summary>
        /// The node changed initial deployment.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        public void NodeChangedInitialDeployment(string name, bool status)
        {
        }

        /// <summary>
        /// The node changed is in maintenance.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isInMaintenance">
        /// The is in maintenance.
        /// </param>
        public void NodeChangedIsInMaintenance(string name, bool isInMaintenance)
        {
        }

        /// <summary>
        /// The node created.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void NodeCreated(string nodeName)
        {
        }

        /// <summary>
        /// The node updated.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void NodeUpdated(string nodeName)
        {
        }

        /// <summary>
        /// The reference outside web context.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public void ReferenceOutsideWebContext(string name, string source)
        {
        }

        public void UpgradeNodeRepositoryDatabaseSchema()
        {
            
        }
    }
}