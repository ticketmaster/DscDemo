// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMofBuilderService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.DeploymentServer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Hangfire;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;

    /// <summary>
    ///     The MofBuilderService interface.
    /// </summary>
    public interface IMofBuilderService
    {
        /// <summary>
        ///     The build failed.
        /// </summary>
        event EventHandler BuildFailed;

        /// <summary>
        ///     The build success.
        /// </summary>
        event EventHandler BuildSucceeded;

        /// <summary>
        ///     The job failed.
        /// </summary>
        event EventHandler JobFailed;

        /// <summary>
        ///     The job succeeded.
        /// </summary>
        event EventHandler JobSucceeded;

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<BuildDetailView> Build(Hashtable configurationData);

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
        Task<BuildDetailView> Build(
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint);

        /// <summary>
        ///     The cleanup builds.
        /// </summary>
        void CleanupBuilds();

        /// <summary>
        ///     The create build.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<BuildDetailView> CreateBuild();

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
        void ExecuteBuild(
            IJobCancellationToken cancellationToken, 
            Hashtable configurationData, 
            int buildId, 
            string configurationPackageName, 
            string configurationPackageVersion, 
            string certificateThumbprint);

        /// <summary>
        /// The get job view.
        /// </summary>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <returns>
        /// The <see cref="JobView"/>.
        /// </returns>
        JobView GetJobView(int jobId);

        /// <summary>
        /// The attach jobs to view.
        /// </summary>
        /// <param name="jobs">
        /// The jobs.
        /// </param>
        /// <returns>
        /// The <see cref="ICollection"/>.
        /// </returns>
        ICollection<JobView> GetJobViews(IEnumerable<int> jobs);

        /// <summary>
        /// The poll build status.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="buildId">
        /// The build id.
        /// </param>
        void PollBuildStatus(IJobCancellationToken token, int buildId);

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
        Task<BuildDetailView> SetBuildStatus(int buildId, BuildStatus status);

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
        Task<BuildDetailView> SubmitBuild(
            int buildId, 
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint);

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
        void SubmitBuildExecute(
            int buildId, 
            Hashtable configurationData, 
            string configurationPackageName, 
            string configurationVersion, 
            string certificateThumbprint);

        /// <summary>
        /// The validate certificate.
        /// </summary>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        void ValidateCertificate(string certificateThumbprint);
    }
}