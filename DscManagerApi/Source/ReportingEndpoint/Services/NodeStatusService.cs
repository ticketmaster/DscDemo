// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeStatusService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Services
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.DscManager.Services;
    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.RequestModels;

    /// <summary>
    /// The node status service.
    /// </summary>
    public class NodeStatusService : IDisposable, INodeStatusService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeStatusService"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public NodeStatusService(IDscEventManager eventManager, ReportingEndpointContext context)
        {
            this.EventManager = eventManager;
            this.Context = context;
            this.NodeStatusRepository = context.Set<NodeStatus>();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected ReportingEndpointContext Context { get; set; }

        /// <summary>
        /// Gets or sets the event manager.
        /// </summary>
        protected IDscEventManager EventManager { get; set; }

        /// <summary>
        /// Gets or sets the node status repository.
        /// </summary>
        protected DbSet<NodeStatus> NodeStatusRepository { get; set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.EventManager.Register(this.HandleEvent);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.EventManager.Unregister(this.HandleEvent);
        }

        /// <summary>
        /// The find node status.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected virtual async Task<NodeStatus> FindNodeStatus(string name)
        {
            var status = await this.NodeStatusRepository.FindAsync(name);
            if (status != null)
            {
                return status;
            }

            status = new NodeStatus { Target = name };
            this.NodeStatusRepository.Add(status);

            return status;
        }

        /// <summary>
        /// The handle event.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task HandleEvent(DscEventArgs eventArgs)
        {
            NodeStatus status = null;
            switch (eventArgs.Name)
            {
                case "BootstrapUpdateReceived":
                    status = await this.FindNodeStatus(eventArgs.GetMember<string>("NodeName"));
                    if (status != null)
                    {
                        status.LastBootstrapTimestamp = DateTime.UtcNow;
                        await this.Context.SaveChangesAsync();
                    }

                    break;
                case "MofFileCreated":
                    status = await this.FindNodeStatus(eventArgs.GetMember<string>("Target"));
                    if (status != null)
                    {
                        status.MofBuildTimestamp = DateTime.UtcNow;
                        status.MofBuildConfigurationPackageName = eventArgs.GetMember<string>("ConfigurationPackageName");
                        status.MofBuildConfigurationPackageVersion = eventArgs.GetMember<string>("ConfigurationPackageVersion");
                        await this.Context.SaveChangesAsync();
                    }

                    break;
                case "NodeChangingInitialDeployment":
                    status = await this.FindNodeStatus(eventArgs.GetMember<string>("Name"));
                    if (status != null)
                    {
                        status.IsInitialDeployment = eventArgs.GetMember<bool>("Status");
                        await this.Context.SaveChangesAsync();
                    }

                    break;
                case "NodeChangedIsInMaintenance":
                    status = await this.FindNodeStatus(eventArgs.GetMember<string>("Name"));
                    if (status != null)
                    {
                        status.IsInMaintenance = eventArgs.GetMember<bool>("IsInMaintenance");
                        await this.Context.SaveChangesAsync();
                    }

                    break;
                case "ConfigurationReportPosted":
                    status = await this.FindNodeStatus(eventArgs.GetMember<string>("Target"));
                    if (status != null)
                    {
                        var request = eventArgs.GetMember<ConfigurationReportRecordRequest>("Request");
                        if (request.Type == ConfigurationType.Apply)
                        {
                            status.LastApplyRunTimestamp = DateTime.UtcNow;
                            status.LastApplyConfigurationPackageName = request.ConfigurationPackageName;
                            status.LastApplyConfigurationVersion = request.ConfigurationPackageVersion;
                            await this.Context.SaveChangesAsync();
                        }
                        else if (request.Type == ConfigurationType.Monitor)
                        {
                            status.LastMonitoringRun = DateTime.UtcNow;
                            await this.Context.SaveChangesAsync();
                        }
                    }

                    break;
                case "DeleteNode":
                    status = await this.NodeStatusRepository.FindAsync(eventArgs.GetMember<string>("NodeName"));
                    if (status != null)
                    {
                        this.NodeStatusRepository.Remove(status);
                        await this.Context.SaveChangesAsync();
                    }

                    break;
            }
        }
    }
}