// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeploymentServerLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Logging
{
    using System;
    using System.Collections.Generic;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;

    /// <summary>
    ///     The DeploymentServerLogging interface.
    /// </summary>
    public interface IDeploymentServerLogging
    {
        /// <summary>
        /// The begin database seed.
        /// </summary>
        /// <param name="initializer">
        /// The initializer.
        /// </param>
        void BeginDeploymentServerDatabaseSeed(string initializer);

        /// <summary>
        /// The build configuration build does not exist.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        void BuildConfigurationBuildDoesNotExist(int buildId);

        /// <summary>
        /// The configuration archived.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="configurationDocumentId">
        /// The configuration document id.
        /// </param>
        void ConfigurationArchived(string target, int configurationDocumentId);

        /// <summary>
        /// The configuration published.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="configurationDocumentId">
        /// The configuration document id.
        /// </param>
        void ConfigurationPublished(string target, int configurationDocumentId);

        /// <summary>
        /// The delete archive configurations.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        void DeleteArchiveConfigurations(string target, int count);

        /// <summary>
        /// The delete expired builds.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="expirationTimestamp">
        /// The expiration timestamp.
        /// </param>
        void DeleteExpiredBuilds(int count, DateTime expirationTimestamp);

        /// <summary>
        /// The end database seed.
        /// </summary>
        /// <param name="initializer">
        /// The initializer.
        /// </param>
        void EndDeploymentServerDatabaseSeed(string initializer);

        /// <summary>
        /// The mof build failed.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        void MofBuildFailed(int buildId);

        /// <summary>
        /// The mof build succeeded.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="select">
        /// </param>
        void MofBuildSucceeded(int buildId, IEnumerable<string> select);

        /// <summary>
        /// The mof built but not target.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        void MofBuiltButNotTarget(string target);

        /// <summary>
        /// The mof file created.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="configurationPackageName">
        /// The configuration package name.
        /// </param>
        /// <param name="configurationVersion">
        /// The configuration version.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        void MofFileCreated(
            string target, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint);

        /// <summary>
        /// The mof file not created.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        void MofFileNotCreated(string target);

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

        /// <summary>
        /// The request exception.
        /// </summary>
        /// <param name="requestUrl">
        /// The request url.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="stackTrace">
        /// The stack trace.
        /// </param>
        void RequestException(string requestUrl, string message, string source, string stackTrace);

        /// <summary>
        /// The submitted build request.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        void SubmittedBuildRequest(Build build);

        /// <summary>
        /// The unpublish configurations node removed.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        void UnpublishConfigurationsNodeRemoved(int count, string name);

        /// <summary>
        ///     The upgrade deployment server database schema.
        /// </summary>
        void UpgradeDeploymentServerDatabaseSchema();
    }
}