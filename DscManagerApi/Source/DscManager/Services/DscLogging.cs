// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscLogging.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.DscManager.Logging;
    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.Logging;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.RequestModels;

    /// <summary>
    ///     The dsc logging.
    /// </summary>
    public class DscLogging : IDeploymentServerLogging, 
                              INodeRepositoryLogging, 
                              IReportingEndpointLogging, 
                              IDscManagerLogging
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DscLogging"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        public DscLogging(IDscEventManager eventManager)
        {
            this.EventManager = eventManager;
        }

        /// <summary>
        ///     Gets or sets the event manager.
        /// </summary>
        protected IDscEventManager EventManager { get; set; }

        /// <summary>
        /// The begin database seed.
        /// </summary>
        /// <param name="initializer">
        /// The initializer.
        /// </param>
        public void BeginDeploymentServerDatabaseSeed(string initializer)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, initializer));
            eventArgs.Initializer = initializer;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The begin database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void BeginNodeRepositoryDatabaseSeed(string name)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name));
            eventArgs.Name = name;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        ///     The bootstrap options not present.
        /// </summary>
        public void BootstrapOptionsNotPresent()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
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
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, nodeName));
            eventArgs.NodeName = nodeName;
            eventArgs.NodeData = nodeData;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The build configuration build does not exist.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        public void BuildConfigurationBuildDoesNotExist(int buildId)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, buildId));
            eventArgs.BuildId = buildId;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        public void BuildRequestedNoBuildService(NodeRequest nodeRequest)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            eventArgs.NodeRequest = nodeRequest;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The build requested no build service.
        /// </summary>
        /// <param name="nodeRequest">
        /// The node request.
        /// </param>
        public void BuildRequestedNoBuildService(IEnumerable<NodeDetailView> nodeRequest)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            eventArgs.NodeRequest = nodeRequest;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The cleanup expired reports.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="daysToKeepReports">
        /// The days to keep reports.
        /// </param>
        public void CleanupExpiredReports(int count, int daysToKeepReports)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, count, daysToKeepReports));
            eventArgs.Count = count;
            eventArgs.DaysToKeepReports = daysToKeepReports;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The begin dsc manager database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void BeginDscManagerDatabaseSeed(string name)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name));
            eventArgs.Name = name;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The cleanup old log entries.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="daysToKeepLogs">
        /// The days to keep logs.
        /// </param>
        public void CleanupOldLogEntries(int count, int daysToKeepLogs)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, count, daysToKeepLogs));
            eventArgs.Count = count;
            eventArgs.DaysToKeepLogs = daysToKeepLogs;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The end dsc manager database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void EndDscManagerDatabaseSeed(string name)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name));
            eventArgs.Name = name;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The configuration archived.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="configurationDocumentId">
        /// The configuration document id.
        /// </param>
        public void ConfigurationArchived(string target, int configurationDocumentId)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target, configurationDocumentId));
            eventArgs.Target = target;
            eventArgs.ConfigurationDocumentId = configurationDocumentId;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The configuration published.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="configurationDocumentId">
        /// The configuration document id.
        /// </param>
        public void ConfigurationPublished(string target, int configurationDocumentId)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target, configurationDocumentId));
            eventArgs.Target = target;
            eventArgs.ConfigurationDocumentId = configurationDocumentId;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The configuration report posted.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public void ConfigurationReportPosted(string target, ConfigurationReportRecordRequest request)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target, request.Type, request));
            eventArgs.Target = target;
            eventArgs.Type = request.Type;
            eventArgs.Request = request;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The delete archive configurations.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public void DeleteArchiveConfigurations(string target, int count)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target, count));
            eventArgs.Target = target;
            eventArgs.Count = count;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The delete configuration reports node removed.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void DeleteConfigurationReportsNodeRemoved(int count, string nodeName)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, count, nodeName));
            eventArgs.Count = count;
            eventArgs.NodeName = nodeName;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The delete expired builds.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="expirationTimestamp">
        /// The expiration timestamp.
        /// </param>
        public void DeleteExpiredBuilds(int count, DateTime expirationTimestamp)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, count, expirationTimestamp));
            eventArgs.Count = count;
            eventArgs.ExpirationTimestamp = expirationTimestamp;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The delete node.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void DeleteNode(string nodeName)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, nodeName));
            eventArgs.NodeName = nodeName;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The end database seed.
        /// </summary>
        /// <param name="initializer">
        /// The initializer.
        /// </param>
        public void EndDeploymentServerDatabaseSeed(string initializer)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, initializer));
            eventArgs.Initializer = initializer;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The end database seed.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void EndNodeRepositoryDatabaseSeed(string name)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name));
            eventArgs.Name = name;
            this.EventManager.CreateEvent(eventArgs);
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
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name, errorMessage));
            eventArgs.Name = name;
            eventArgs.ErrorMessage = errorMessage;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The mof build failed.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        public void MofBuildFailed(int buildId)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, buildId));
            eventArgs.BuildId = buildId;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The mof build succeeded.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="targets">
        /// </param>
        public void MofBuildSucceeded(int buildId, IEnumerable<string> targets)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, buildId, targets));
            eventArgs.BuildId = buildId;
            eventArgs.Targets = targets;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The mof built but not target.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public void MofBuiltButNotTarget(string target)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target));
            eventArgs.Target = target;
            this.EventManager.CreateEvent(eventArgs);
        }
        public void MofFileCreated(string target, string configurationPackageName, string configurationVersion, string certificateThumbprint)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(
                name, 
                this.GetMessageForMethod(name, target, configurationPackageName, configurationVersion, certificateThumbprint));
            eventArgs.Target = target;
            eventArgs.ConfigurationPackageName = configurationPackageName;
            eventArgs.ConfigurationPackageVersion = configurationVersion;
            eventArgs.CertificateThumbprint = certificateThumbprint;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The mof file not created.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        public void MofFileNotCreated(string target)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, target));
            eventArgs.Target = target;
            this.EventManager.CreateEvent(eventArgs);
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
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name, status));
            eventArgs.Name = name;
            eventArgs.Status = status;
            this.EventManager.CreateEvent(eventArgs);
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
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name, isInMaintenance));
            eventArgs.Name = name;
            eventArgs.IsInMaintenance = isInMaintenance;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The node created.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void NodeCreated(string nodeName)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, nodeName));
            eventArgs.NodeName = nodeName;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The node updated.
        /// </summary>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void NodeUpdated(string nodeName)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, nodeName));
            eventArgs.NodeName = nodeName;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The received request.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="clientIp">
        /// The client ip.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        public void ReceivedRequest(string uri, string method, string clientIp, string username)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, uri, method, clientIp, username));
            eventArgs.Uri = uri;
            eventArgs.Method = method;
            eventArgs.ClientIp = clientIp;
            eventArgs.Username = username;
            this.EventManager.CreateEvent(eventArgs);
        }

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
        public void RequestException(string requestUrl, string message, string source, string stackTrace)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(
                name, 
                this.GetMessageForMethod(name, requestUrl, message, source, stackTrace));
            eventArgs.RequestUri = requestUrl;
            eventArgs.ErrorMessage = message;
            eventArgs.Source = source;
            eventArgs.StackTrace = stackTrace;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The submitted build request.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        public void SubmittedBuildRequest(Build build)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, build.Id, build.Targets.Count));
            eventArgs.BuildId = build.Id;
            eventArgs.Targets = build.Targets;
            eventArgs.TargetCount = build.Targets.Count;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        ///     The start.
        /// </summary>
        public void Start()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The upgrade dsc manager database schema.
        /// </summary>
        public void UpgradeDscManagerDatabaseSchema()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The unpublish configurations node removed.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="nodeName">
        /// The node name.
        /// </param>
        public void UnpublishConfigurationsNodeRemoved(int count, string nodeName)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name, count, nodeName));
            eventArgs.Count = count;
            eventArgs.NodeName = nodeName;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The upgrade deployment server database schema.
        /// </summary>
        public void UpgradeDeploymentServerDatabaseSchema()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        ///     The upgrade reporting endpoint database schema.
        /// </summary>
        public void UpgradeReportingEndpointDatabaseSchema()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
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
        void IDeploymentServerLogging.ReferenceOutsideWebContext(string name, string source)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name, source));
            eventArgs.Name = name;
            eventArgs.Source = source;
            this.EventManager.CreateEvent(eventArgs);
        }

        public void UpgradeNodeRepositoryDatabaseSchema()
        {
            var name = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name, this.GetMessageForMethod(name));
            this.EventManager.CreateEvent(eventArgs);
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
        void INodeRepositoryLogging.ReferenceOutsideWebContext(string name, string source)
        {
            var name2 = MethodBase.GetCurrentMethod().Name;
            dynamic eventArgs = new DscEventArgs(name2, this.GetMessageForMethod(name2, name, source));
            eventArgs.Name = name;
            eventArgs.Source = source;
            this.EventManager.CreateEvent(eventArgs);
        }

        /// <summary>
        /// The get message for method.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="paramList">
        /// The param list.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetMessageForMethod(string name, params object[] paramList)
        {
            var field =
                typeof(DscLoggingMessages).GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .FirstOrDefault(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == name);
            var message = field?.GetRawConstantValue() as string;
            if (message != null)
            {
                return Regex.Replace(
                    message, 
                    @"\{(\d{1,3})\}", 
                    match => this.GetStringValue(paramList.ElementAtOrDefault(Convert.ToInt32(match.Groups[1].Value))));
            }

            return null;
        }

        Type GetEnumerableType(Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }
            return null;
        }

        private string GetStringValue(object value)
        {
            var t = this.GetEnumerableType(value.GetType());
            if (t != null && value.GetType() != typeof(string))
            {
                var re = string.Join("\r\n", (IEnumerable<object>)value);
                return re;
            }

            return value.ToString();
        }
    }
}