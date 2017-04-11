// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MofBuilderService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    using Hangfire;
    using Hangfire.Common;
    using Hangfire.Storage;

    using Newtonsoft.Json.Linq;

    using Ticketmaster.Common.PowerShellRunner;
    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;

    /// <summary>
    ///     The mof builder service.
    /// </summary>
    public class MofBuilderService : IMofBuilderService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MofBuilderService"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="runner">
        /// The runner.
        /// </param>
        /// <param name="configurationService">
        /// The configuration service.
        /// </param>
        /// <param name="monitoringApi">
        /// The monitoring api.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public MofBuilderService(
            DeploymentServerContext context, 
            IDeploymentServerOptions options, 
            IPowerShellRunner runner, 
            IConfigurationService configurationService, 
            IMonitoringApi monitoringApi, 
            IDeploymentServerLogging logging)
        {
            this.Context = context;
            this.BuildRepository = context.Set<Build>();
            this.Logging = logging;
            this.Options = options;
            this.Runner = runner;
            this.ConfigurationService = configurationService;
            this.MonitoringApi = monitoringApi;
        }

        /// <summary>
        ///     The build failed.
        /// </summary>
        public event EventHandler BuildFailed;

        /// <summary>
        ///     The build success.
        /// </summary>
        public event EventHandler BuildSucceeded;

        /// <summary>
        ///     The job failed.
        /// </summary>
        public event EventHandler JobFailed;

        /// <summary>
        ///     The job succeeded.
        /// </summary>
        public event EventHandler JobSucceeded;

        /// <summary>
        ///     Gets or sets the build repository.
        /// </summary>
        protected DbSet<Build> BuildRepository { get; set; }

        /// <summary>
        ///     Gets or sets the configuration service.
        /// </summary>
        protected IConfigurationService ConfigurationService { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected IDeploymentServerLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the monitoring api.
        /// </summary>
        protected IMonitoringApi MonitoringApi { get; set; }

        /// <summary>
        ///     Gets or sets the options.
        /// </summary>
        protected IDeploymentServerOptions Options { get; set; }

        /// <summary>
        ///     Gets or sets the runner.
        /// </summary>
        protected IPowerShellRunner Runner { get; set; }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<BuildDetailView> Build(Hashtable configurationData)
        {
            return await this.Build(configurationData, null, null, null);
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<BuildDetailView> Build(
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint)
        {
            var build = await this.CreateBuildImpl();
            return
                await
                this.SubmitBuild(
                    build.Id, 
                    configurationData, 
                    configurationPackageName, 
                    configurationVersion, 
                    certificateThumbprint);
        }

        /// <summary>
        ///     The cleanup builds.
        /// </summary>
        public void CleanupBuilds()
        {
            var expirationTimestamp = DateTime.UtcNow.AddDays(this.Options.DaysToKeepBuildHistory * -1);
            var expiredBuilds = this.BuildRepository.Where(b => b.RequestTimestamp < expirationTimestamp).ToList();
            this.BuildRepository.RemoveRange(expiredBuilds);
            this.Logging?.DeleteExpiredBuilds(expiredBuilds.Count(), expirationTimestamp);
            this.Context.SaveChanges();

            var enqueuedBuilds =
                this.BuildRepository.Include(b => b.Targets).Where(b => b.Status == BuildStatus.Enqueued);
            foreach (var build in enqueuedBuilds)
            {
                var jobs = build.Targets.Select(t => t.JobId).Distinct();
                var inProgress = false;
                foreach (var job in jobs)
                {
                    var status = this.MonitoringApi.JobDetails(job.ToString());
                    if (status?.History[0].StateName != "Failed")
                    {
                        inProgress = true;
                    }
                }

                if (!inProgress)
                {
                    build.Status = BuildStatus.Failed;
                    build.CompleteTimestamp = DateTime.UtcNow;
                }
            }

            this.Context.SaveChanges();
        }

        /// <summary>
        ///     The create build.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<BuildDetailView> CreateBuild()
        {
            return (await this.CreateBuildImpl()).Map<BuildDetailView>();
        }

        /// <summary>
        /// The execute build.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="configurationPackageName">
        /// The configuration package name.
        /// </param>
        /// <param name="configurationPackageVersion">
        /// The configuration package version.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        [Queue("mofbuilder")]
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public virtual void ExecuteBuild(
            IJobCancellationToken cancellationToken, 
            Hashtable configurationData, 
            int buildId, 
            string configurationPackageName, 
            string configurationPackageVersion, 
            string certificateThumbprint)
        {
            Thread.Sleep(7500);
            if (string.IsNullOrEmpty(certificateThumbprint))
            {
                certificateThumbprint = this.Options.DefaultEncryptionCertificateThumbprint;
            }

            this.ValidateCertificate(certificateThumbprint);

            var build = this.BuildRepository.Include(e => e.Targets).FirstOrDefault(e => e.Id == buildId);

            if (build == null)
            {
                this.Logging?.BuildConfigurationBuildDoesNotExist(buildId);
                var exception = new JobLoadException("Failed to retrieve a build with Build Id " + buildId + " .", null);
                this.OnJobFailed(
                    new JobEventArgs
                        {
                            WorkingPath = this.Options.ConfigurationWorkingPath, 
                            InvocationException = exception
                        });
                return;
            }

            if (build.Status == BuildStatus.NotSubmitted || build.Status == BuildStatus.Enqueued)
            {
                build.Status = BuildStatus.InProgress;
            }

            var targetList = this.GetTargetsFromConfigurationData(configurationData);
            var targets = build.Targets.Where(t => targetList.Contains(t.Target)).ToArray();

            foreach (var t in targets)
            {
                t.Status = BuildStatus.InProgress;
            }

            this.Context.SaveChanges();

            var jobIds = (from t in build.Targets select t.BuildId).Distinct();

            this.Runner.SetVariable("configurationData", configurationData);
            this.Runner.SetVariable("outputPath", this.Options.ConfigurationWorkingPath);
            this.Runner.SetVariable("configurationPackageName", configurationPackageName);
            this.Runner.SetVariable("configurationPackageVersion", configurationPackageVersion);
            this.Runner.SetVariable("certificateThumbprint", certificateThumbprint);
            this.Runner.SetVariable("dscManagerEndpoint", this.Options.DscManagerEndpoint);
            var tokenSource = new CancellationTokenSource();
            var task = this.Runner.RunScriptAsync(
                this.Options.RootConfigurationPath, 
                "-ConfigurationData $configurationData -OutputPath $outputPath -ConfigurationPackageName $configurationPackageName -ConfigurationPackageVersion $configurationPackageVersion -CertificateThumbprint $certificateThumbprint -DscManagerEndpoint $dscManagerEndpoint", tokenSource.Token);

            do
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch
                {
                    tokenSource.Cancel();
                    foreach (var target in targets)
                    {
                        target.Status = BuildStatus.Failed;
                    }

                    this.Context.SaveChanges();

                    this.OnJobFailed(
                        new JobEventArgs
                        {
                            Build = build,
                            Result = task.Result,
                            JobIds = jobIds,
                            WorkingPath = this.Options.ConfigurationWorkingPath
                        });
                    throw;
                }

                Thread.Sleep(200);
            }

            while (!task.IsCompleted);
            var result = task.Result;
            if (result.InvocationException != null)
            {
                foreach (var target in targets)
                {
                    target.Status = BuildStatus.Failed;
                }

                this.Context.SaveChanges();

                this.OnJobFailed(
                    new JobEventArgs
                        {
                            Build = build, 
                            Result = result, 
                            JobIds = jobIds, 
                            WorkingPath = this.Options.ConfigurationWorkingPath, 
                            InvocationException = result.InvocationException
                        });
                return;
            }

            var enumerable = jobIds as int[] ?? jobIds.ToArray();
            if (result.HadErrors)
            {
                foreach (var target in targets)
                {
                    target.Status = BuildStatus.Failed;
                }

                this.Context.SaveChanges();

                this.OnJobFailed(
                    new JobEventArgs
                        {
                            Build = build, 
                            Result = result, 
                            JobIds = enumerable, 
                            WorkingPath = this.Options.ConfigurationWorkingPath
                        });
                return;
            }

            var files = result.Result.ToArray();

            foreach (var target in targets)
            {
                // var file = files.FirstOrDefault(f => f.Name == target.Target + ".mof");
                var file = files.FirstOrDefault(f => (string)f.Properties["Name"].Value == target.Target);
                var targetRecord = target;
                if (targetRecord == null)
                {
                    this.Logging?.MofBuiltButNotTarget(target.Target);
                    continue;
                }

                if (file?.Properties["CertificateThumbprint"].Value != null)
                {
                    if ((string)file.Properties["CertificateThumbprint"].Value != target.CertificateThumbprint)
                    {
                        target.CertificateThumbprint = (string)file.Properties["CertificateThumbprint"].Value;
                    }
                }

                if (file?.Properties["File"].Value == null)
                {
                    this.Logging?.MofFileNotCreated(target.Target);
                    targetRecord.Status = BuildStatus.Failed;
                    continue;
                }

                this.Logging?.MofFileCreated(
                    target.Target, 
                    targetRecord.ConfigurationPackageName, 
                    targetRecord.ConfigurationPackageVersion,
                    targetRecord.CertificateThumbprint);
                targetRecord.Status = BuildStatus.Succeeded;
                var text = File.ReadAllText((string)file.Properties["File"].Value);
                this.ConfigurationService.Publish(text, target.Target, target.CertificateThumbprint);
            }

            this.Context.SaveChanges();

            this.OnJobSucceeded(
                new JobEventArgs
                    {
                        Build = build, 
                        Result = result, 
                        JobIds = enumerable, 
                        WorkingPath = this.Options.ConfigurationWorkingPath
                    });
        }

        /// <summary>
        /// The get job view.
        /// </summary>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <returns>
        /// The <see cref="JobView"/>.
        /// </returns>
        public virtual JobView GetJobView(int jobId)
        {
            var job = this.MonitoringApi.JobDetails(jobId.ToString());

            if (job == null)
            {
                return null;
            }

            var jobView = new JobView { Id = jobId };

            if (job.History.Any())
            {
                var failed = job.History.FirstOrDefault(h => h.StateName == "Failed");
                if (failed != null)
                {
                    jobView.MessageDetailed = failed.Data["ExceptionDetails"];
                    jobView.Message = failed.Data["ExceptionMessage"];
                }

                if (job.History[0].StateName == "Succeeded" || job.History[0].StateName == "Failed"
                    || job.History[0].StateName == "Deleted")
                {
                    jobView.CompleteTimestamp = job.History[0].CreatedAt;
                }

                jobView.Status = job.History[0].StateName;
            }

            return jobView;
        }

        /// <summary>
        /// The attach jobs to view.
        /// </summary>
        /// <param name="jobs">
        /// The jobs.
        /// </param>
        /// <returns>
        /// The <see cref="ICollection"/>.
        /// </returns>
        public virtual ICollection<JobView> GetJobViews(IEnumerable<int> jobs)
        {
            return jobs.Select(this.GetJobView).ToList();
        }

        /// <summary>
        /// The poll build status.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        [Queue("priority")]
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public virtual void PollBuildStatus(IJobCancellationToken token, int buildId)
        {
            var build = this.BuildRepository.Find(buildId);
            if (build == null || build.Status == BuildStatus.Failed || build.Status == BuildStatus.PartialFailure
                || build.Status == BuildStatus.Succeeded)
            {
                return;
            }

            var status = this.GetBuildStatus(build);
            if (build.Status != status)
            {
                build.Status = status;
                this.Context.SaveChanges();
                switch (status)
                {
                    case BuildStatus.Failed:
                    case BuildStatus.PartialFailure:
                        this.OnBuildFailed(new BuildEventArgs { Build = build, Status = status });
                        break;
                    case BuildStatus.Succeeded:
                        this.OnBuildSucceeded(new BuildEventArgs { Build = build, Status = status });
                        break;
                }
            }
        }

        /// <summary>
        /// The set build status.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<BuildDetailView> SetBuildStatus(int buildId, BuildStatus status)
        {
            var build = await this.BuildRepository.FindAsync(buildId);
            if (build == null)
            {
                return null;
            }

            build.Status = status;
            await this.Context.SaveChangesAsync();
            return build.Map<BuildDetailView>();
        }

        /// <summary>
        /// The submit build.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="configurationData">
        /// The configuration data.
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<BuildDetailView> SubmitBuild(
            int buildId, 
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint)
        {
            var build = await this.BuildRepository.FindAsync(buildId);
            if (build == null)
            {
                return null;
            }

            var job =
                BackgroundJob.Enqueue(
                    () =>
                    this.SubmitBuildExecute(
                        buildId, 
                        configurationData, 
                        configurationPackageName, 
                        configurationVersion, 
                        certificateThumbprint));

            build.SubmissionJobId = Convert.ToInt32(job);
            await this.Context.SaveChangesAsync();
            return build.Map<BuildDetailView>();
        }

        /// <summary>
        /// The submit build execute.
        /// </summary>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        /// <param name="configurationData">
        /// The configuration data.
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
        /// <exception cref="Exception">
        /// </exception>
        [Queue("priority")]
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void SubmitBuildExecute(
            int buildId, 
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint)
        {
            configurationData = this.ReparseJson(configurationData);
            var build = this.BuildRepository.Find(buildId);
            if (build == null)
            {
                throw new Exception("Could not find build with Id " + buildId + " in the database.");
            }

            configurationData = this.ReparseJson(configurationData);
            var targets = this.SplitConfigurationData(configurationData, configurationPackageName, configurationVersion);

            foreach (var targetGroup in targets)
            {
                var jobId =
                    BackgroundJob.Enqueue(
                        () =>
                        this.ExecuteBuild(
                            JobCancellationToken.Null, 
                            targetGroup.ConfigurationData, 
                            build.Id, 
                            targetGroup.ConfigurationPackageName, 
                            targetGroup.ConfigurationPackageVersion, 
                            certificateThumbprint));
                BackgroundJob.ContinueWith(
                    jobId, 
                    () => this.PollBuildStatus(JobCancellationToken.Null, build.Id), 
                    JobContinuationOptions.OnAnyFinishedState);

                foreach (var target in targetGroup.Targets)
                {
                    build.Targets.Add(
                        new BuildTarget
                            {
                                Build = build, 
                                JobId = Convert.ToInt32(jobId), 
                                Status = BuildStatus.Enqueued, 
                                Target = target, 
                                ConfigurationPackageName = targetGroup.ConfigurationPackageName, 
                                ConfigurationPackageVersion = targetGroup.ConfigurationPackageVersion, 
                                CertificateThumbprint = certificateThumbprint
                            });
                }
            }

            build.Status = BuildStatus.Enqueued;
            this.Context.SaveChanges();

            this.Logging.SubmittedBuildRequest(build);
        }

        /// <summary>
        /// The validate certificate.
        /// </summary>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public void ValidateCertificate(string certificateThumbprint)
        {
            if (string.IsNullOrEmpty(certificateThumbprint))
            {
                return;
            }

            var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            var certStoreUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            certStore.Open(OpenFlags.ReadOnly);
            certStoreUser.Open(OpenFlags.ReadOnly);

            var cert = certStore.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
            var userCert = certStoreUser.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);

            if (cert.Count == 0 && userCert.Count == 0)
            {
                throw new Exception(
                    "An encryption certificate with thumbprint " + certificateThumbprint
                    + " could not be found in the My location in either the LocalMachine or CurrentUser store.");
            }
        }

        /// <summary>
        ///     The create build impl.
        /// </summary>
        /// <param name="certificateThumbprint">
        ///     The certificate thumbprint.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        protected async Task<Build> CreateBuildImpl()
        {
            var build = new Build
                            {
                                RequestTimestamp = DateTime.UtcNow, 
                                Status = BuildStatus.NotSubmitted, 
                                Targets = new List<BuildTarget>()
                            };

            this.BuildRepository.Add(build);
            await this.Context.SaveChangesAsync();
            return build;
        }

        /// <summary>
        /// The get build status.
        /// </summary>
        /// <param name="build">
        /// The build.
        /// </param>
        /// <returns>
        /// The <see cref="BuildStatus"/>.
        /// </returns>
        protected BuildStatus GetBuildStatus(Build build)
        {
            if (build.Targets.All(t => t.Status == BuildStatus.NotSubmitted))
            {
                return BuildStatus.NotSubmitted;
            }

            if (build.Targets.All(t => t.Status == BuildStatus.Enqueued || t.Status == BuildStatus.NotSubmitted))
            {
                return BuildStatus.Enqueued;
            }

            if (build.Targets.All(t => t.Status == BuildStatus.Succeeded))
            {
                return BuildStatus.Succeeded;
            }

            if (build.Targets.All(t => t.Status == BuildStatus.Failed))
            {
                return BuildStatus.Failed;
            }

            if (build.Targets.All(t => t.Status == BuildStatus.Succeeded || t.Status == BuildStatus.Failed))
            {
                return BuildStatus.PartialFailure;
            }

            if (build.Targets.Any(t => t.Status == BuildStatus.InProgress))
            {
                return BuildStatus.InProgress;
            }

            return BuildStatus.Unknown;
        }

        /// <summary>
        /// The get targets from configuration data.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected virtual IEnumerable<string> GetTargetsFromConfigurationData(Hashtable configurationData)
        {
            configurationData = this.ReparseJson(configurationData);
            if (!configurationData.ContainsKey("AllNodes") || !(configurationData["AllNodes"] is IEnumerable))
            {
                throw new ArgumentException("The provided configuration data is not properly formed.");
            }

            var nodeSet = (IEnumerable)configurationData["AllNodes"];
            var nodes = nodeSet.OfType<Hashtable>();

            var hashtables = nodes as Hashtable[] ?? nodes.ToArray();
            if (hashtables.Any(d => !d.ContainsKey("NodeName")))
            {
                throw new ArgumentException(
                    "The provided configuration data contains one or more nodes which are missing the NodeName property.");
            }

            return hashtables.Select(node => (string)node["NodeName"]).ToList();
        }

        /// <summary>
        /// The on build failed.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        protected virtual void OnBuildFailed(BuildEventArgs eventArgs)
        {
            var handler = this.BuildFailed;
            handler?.Invoke(this, eventArgs);

            eventArgs.Build.Status = eventArgs.Status;
            eventArgs.Build.CompleteTimestamp = DateTime.UtcNow;
            this.Context.SaveChanges();

            this.Logging.MofBuildFailed(eventArgs.Build.Id);
        }

        /// <summary>
        /// The on build succeeded.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        protected virtual void OnBuildSucceeded(BuildEventArgs eventArgs)
        {
            var handler = this.BuildSucceeded;
            handler?.Invoke(this, eventArgs);

            eventArgs.Build.Status = eventArgs.Status;
            eventArgs.Build.CompleteTimestamp = DateTime.UtcNow;
            this.Context.SaveChanges();

            this.Logging.MofBuildSucceeded(eventArgs.Build.Id, eventArgs.Build.Targets.Select(t => t.Target));
        }

        /// <summary>
        /// The on job failed.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        protected virtual void OnJobFailed(JobEventArgs eventArgs)
        {
            var handler = this.JobFailed;
            handler?.Invoke(this, eventArgs);

            if (eventArgs.InvocationException != null)
            {
                throw eventArgs.InvocationException;
            }

            throw new MofBuildException(eventArgs.Result.Errors);
        }

        /// <summary>
        /// The on job succeeded.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        protected virtual void OnJobSucceeded(JobEventArgs eventArgs)
        {
            var handler = this.JobSucceeded;
            handler?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// The reparse json.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="Hashtable"/>.
        /// </returns>
        protected Hashtable ReparseJson(Hashtable configurationData)
        {
            if (!configurationData.ContainsKey("AllNodes"))
            {
                return configurationData;
            }

            var allNodes = configurationData["AllNodes"] as JArray;
            if (allNodes == null)
            {
                return configurationData;
            }

            var list = new List<Hashtable>();
            foreach (var token in allNodes)
            {
                var ht = token.ToObject<Hashtable>();
                var arrayList =
                    (from object key in ht.Keys
                     let property = ht[key] as JArray
                     where property != null
                     select (string)key).ToList();
                arrayList.ForEach(key => ht[key] = ((JArray)ht[key]).ToObject<object[]>());
                var hashTable = (from object key in ht.Keys
                                 let property = ht[key] as JObject
                                 where property != null && property.Type == JTokenType.Object
                                 select (string)key).ToList();
                hashTable.ForEach(key => ht[key] = ((JObject)ht[key]).ToObject<Hashtable>());
                list.Add(ht);
            }

            configurationData["AllNodes"] = list.ToArray();

            return configurationData;
        }

        /// <summary>
        /// The split configuration data.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <param name="configurationPackageName">
        /// The configuration package name.
        /// </param>
        /// <param name="configurationPackageVersion">
        /// The configuration package version.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected IEnumerable<MofBuildConfigurationData> SplitConfigurationData(
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationPackageVersion)
        {
            var servers = this.MonitoringApi.Servers().Where(s => s.Queues.Contains("mofbuilder")).ToArray();
            var numberOfJobs = servers.Length * servers.Select(s => s.WorkersCount).Sum() / 2;

            if (!configurationData.ContainsKey("AllNodes") || !(configurationData["AllNodes"] is IEnumerable))
            {
                throw new ArgumentException("The provided configuration data is not properly formed.");
            }

            var nodeSet = (IEnumerable)configurationData["AllNodes"];
            var nodes = nodeSet.OfType<Hashtable>();

            var hashtables = nodes as Hashtable[] ?? nodes.ToArray();
            if (hashtables.Any(d => !d.ContainsKey("NodeName")))
            {
                throw new ArgumentException(
                    "The provided configuration data contains one or more nodes which are missing the NodeName property.");
            }

            var splitConfigurationData = new List<MofBuildConfigurationData>();
            var maxJobCounter = hashtables.Length / numberOfJobs;

            if (maxJobCounter < 1)
            {
                maxJobCounter = 1;
            }
            else if (maxJobCounter > 10)
            {
                maxJobCounter = 10;
            }

            IEnumerable<IGrouping<BuildGroupKey, Hashtable>> nodeGroups;
            if (!string.IsNullOrEmpty(configurationPackageName) && !string.IsNullOrEmpty(configurationPackageVersion))
            {
                var ngList = new List<BuildGrouping<Hashtable>>();
                var bg = new BuildGrouping<Hashtable>(configurationPackageName, configurationPackageVersion);
                bg.AddRange(hashtables);
                ngList.Add(bg);
                nodeGroups = ngList;
            }
            else
            {
                var keys = new List<BuildGroupKey>();
                nodeGroups = hashtables.GroupBy(
                    h =>
                        {
                            var key =
                                keys.FirstOrDefault(
                                    k =>
                                    k.Name == (string)h["ConfigurationPackageName"]
                                    && k.Version == (string)h["ConfigurationPackageVersion"]);
                            if (key == null)
                            {
                                var newKey = new BuildGroupKey
                                                 {
                                                     Name = (string)h["ConfigurationPackageName"], 
                                                     Version = (string)h["ConfigurationPackageVersion"]
                                                 };
                                keys.Add(newKey);
                                return newKey;
                            }

                            return key;
                        }).ToArray();
            }

            var nodeGroupsOrdered = nodeGroups.OrderBy(g => g.Count());
            foreach (var group in nodeGroupsOrdered)
            {
                if (group.Count() <= maxJobCounter)
                {
                    var configData = new Hashtable();
                    foreach (var key in configurationData.Keys.Cast<string>().Where(key => key != "AllNodes"))
                    {
                        configData.Add(key, configurationData[key]);
                    }

                    var nodeArray = group.ToArray();
                    configData["AllNodes"] = nodeArray;
                    var targetList = from n in nodeArray select (string)n["NodeName"];
                    splitConfigurationData.Add(
                        new MofBuildConfigurationData
                            {
                                ConfigurationData = configData, 
                                Targets = targetList, 
                                ConfigurationPackageName = group.Key.Name, 
                                ConfigurationPackageVersion = group.Key.Version
                            });
                }
                else
                {
                    for (var i = 0; i < group.Count(); i += maxJobCounter)
                    {
                        var groupArray = group.ToArray();
                        var length = group.Count() - i < maxJobCounter ? group.Count() - i : maxJobCounter;
                        var buffer = new Hashtable[length];
                        Array.Copy(groupArray, i, buffer, 0, length);
                        var configData = new Hashtable { { "AllNodes", buffer } };
                        foreach (var key in configurationData.Keys.Cast<string>().Where(key => key != "AllNodes"))
                        {
                            configData.Add(key, configurationData[key]);
                        }

                        var targetList = from n in buffer select (string)n["NodeName"];

                        splitConfigurationData.Add(
                            new MofBuildConfigurationData
                                {
                                    ConfigurationData = configData, 
                                    Targets = targetList, 
                                    ConfigurationPackageName = group.Key.Name, 
                                    ConfigurationPackageVersion = group.Key.Version
                                });
                    }
                }
            }

            return splitConfigurationData;
        }
    }
}