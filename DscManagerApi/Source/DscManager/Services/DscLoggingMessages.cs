// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscLoggingMessages.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    /// <summary>
    ///     The dsc logging messages.
    /// </summary>
    public static class DscLoggingMessages
    {
        /// <summary>
        ///     The begin deployment server database seed.
        /// </summary>
        public const string BeginDeploymentServerDatabaseSeed =
            "Beginning database seed for DeploymentServer with initializer: {0}.";

        /// <summary>
        ///     The begin dsc manager database seed.
        /// </summary>
        public const string BeginDscManagerDatabaseSeed = "Seeding for database DscManager is starting.";

        /// <summary>
        ///     The begin node repository database seed.
        /// </summary>
        public const string BeginNodeRepositoryDatabaseSeed =
            "Beginning database seed for NodeRepository with initializer: {0}.";

        /// <summary>
        ///     The bootstrap options not present.
        /// </summary>
        public const string BootstrapOptionsNotPresent =
            "The minimum required bootstrap options are not present in the database.";

        /// <summary>
        ///     The bootstrap update received.
        /// </summary>
        public const string BootstrapUpdateReceived = "The target {0} has bootstrapped and provided an update.";

        /// <summary>
        ///     The build configuration build does not exist.
        /// </summary>
        public const string BuildConfigurationBuildDoesNotExist =
            "While attempting to execute a job to build a configuration with Build Id {0}, the build does not exist in the database.";

        /// <summary>
        ///     The build requested no build service.
        /// </summary>
        public const string BuildRequestedNoBuildService =
            "The NodeRepository component requested that a build be processed, but there is no build service available. This is typically part of the DeploymentServer component.";

        /// <summary>
        ///     The cleanup expired reports.
        /// </summary>
        public const string CleanupExpiredReports =
            "{0} report(s) have been cleaned up. These report(s) were older than {1} days.";

        /// <summary>
        ///     The cleanup old log entries.
        /// </summary>
        public const string CleanupOldLogEntries =
            "{0} log entries were deleted because they were older than {1} day(s).";

        /// <summary>
        ///     The configuration archived.
        /// </summary>
        public const string ConfigurationArchived = "A configuration with Id {1} for targt {0} was archived.";

        /// <summary>
        ///     The configuration published.
        /// </summary>
        public const string ConfigurationPublished = "A configuration with Id {1} for target {0} was published.";

        /// <summary>
        ///     The configuration report posted.
        /// </summary>
        public const string ConfigurationReportPosted = "A new configuration report for {0} has been received. The type of report is {1}.";

        /// <summary>
        ///     The delete archive configurations.
        /// </summary>
        public const string DeleteArchiveConfigurations = "{1} archive configuration(s) were deleted for target {0}.";

        /// <summary>
        ///     The delete configuration reports node removed.
        /// </summary>
        public const string DeleteConfigurationReportsNodeRemoved =
            "{0} configuration reports have been deleted because the node {1} was deleted.";

        /// <summary>
        ///     The delete expired builds.
        /// </summary>
        public const string DeleteExpiredBuilds =
            "{0} expired build(s) were deleted. These build(s) were older than {1}.";

        /// <summary>
        ///     The delete node.
        /// </summary>
        public const string DeleteNode = "The node {0} has been deleted.";

        /// <summary>
        ///     The end deployment server database seed.
        /// </summary>
        public const string EndDeploymentServerDatabaseSeed = "Ending database seed operation for DeploymentServer.";

        /// <summary>
        ///     The end dsc manager database seed.
        /// </summary>
        public const string EndDscManagerDatabaseSeed = "Seeding for database DscManager has completed.";

        /// <summary>
        ///     The end node repository database seed.
        /// </summary>
        public const string EndNodeRepositoryDatabaseSeed = "Ending database seed operation for NodeRepository.";

        /// <summary>
        ///     The mof build failed.
        /// </summary>
        public const string MofBuildFailed = "A Mof Build job with Id {0} failed.";

        /// <summary>
        ///     The mof build succeeded.
        /// </summary>
        public const string MofBuildSucceeded =
            "A Mof Build job with Id {0} succeeded. The following targets were built:\r\n{1}";

        /// <summary>
        ///     The mof built but not target.
        /// </summary>
        public const string MofBuiltButNotTarget =
            "A Mof Build succeeded, but no MOF file for target {0} was generated.";

        /// <summary>
        ///     The mof file created.
        /// </summary>
        public const string MofFileCreated =
            "A MOF file was created for target {0} with configuration package {1}, version {2}.";

        /// <summary>
        ///     The mof file not created.
        /// </summary>
        public const string MofFileNotCreated = "A MOF file failed to be created for target {0}.";

        /// <summary>
        ///     The node agent error.
        /// </summary>
        public const string NodeAgentError = "The agent for node {0} reported the following error:\r\n{1}";

        /// <summary>
        ///     The node changed is in maintenance.
        /// </summary>
        public const string NodeChangedIsInMaintenance = "The node {0} is changing its IsInMaintenance status to {1}.";

        /// <summary>
        ///     The node changing initial deployment.
        /// </summary>
        public const string NodeChangingInitialDeployment =
            "The node {0} is changing its InitialDeployment status to {1}.";

        /// <summary>
        ///     The node created.
        /// </summary>
        public const string NodeCreated = "The node {0} has been created.";

        /// <summary>
        ///     The node updated.
        /// </summary>
        public const string NodeUpdated = "The node {0} has been updated.";

        /// <summary>
        ///     The received request.
        /// </summary>
        public const string ReceivedRequest = "Received request for {1} {0} from user {3} at IP {2}.";

        /// <summary>
        ///     The reference outside web context.
        /// </summary>
        public const string ReferenceOutsideWebContext =
            "A reference for {0} from {1} was made outside the context of a web request.";

        /// <summary>
        ///     The request exception.
        /// </summary>
        public const string RequestException =
            "An error was encountered when processing the request for Uri {0}.\r\nMessage: {1}\r\nSource: {2}\r\nStack Trace:{3}";

        /// <summary>
        ///     The start.
        /// </summary>
        public const string Start = "The DSC Manager API has started.";

        /// <summary>
        /// The submitted build request.
        /// </summary>
        public const string SubmittedBuildRequest =
            "A build request is being submitted for Build Id {0}. There are {1} targets in this build.";

        /// <summary>
        ///     The unpublish configurations node removed.
        /// </summary>
        public const string UnpublishConfigurationsNodeRemoved =
            "{0} configurations have been deleted because the node {1} was deleted.";

        /// <summary>
        ///     The upgrade database schema.
        /// </summary>
        public const string UpgradeDatabaseSchema = "The schema for the ReportingEndpoint database has been updated.";

        /// <summary>
        ///     The upgrade deployment server database schema.
        /// </summary>
        public const string UpgradeDeploymentServerDatabaseSchema =
            "The schema for the DeploymentServer database has been updated.";

        /// <summary>
        ///     The upgrade dsc manager database schema.
        /// </summary>
        public const string UpgradeDscManagerDatabaseSchema = "The schema for the DscManager database has been updated.";

        /// <summary>
        ///     The upgrade node repository database schema.
        /// </summary>
        public const string UpgradeNodeRepositoryDatabaseSchema =
            "The schema for the ReportingEndpoint database has been updated.";
    }
}