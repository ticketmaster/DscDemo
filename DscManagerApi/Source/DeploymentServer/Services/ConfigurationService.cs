// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.Services
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.DeploymentServer.RequestModels;
    using Ticketmaster.Dsc.DscManager.Services;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;
    using Ticketmaster.Dsc.Interfaces.DscManager;

    /// <summary>
    ///     The configuration service.
    /// </summary>
    public class ConfigurationService : DscEventHandler, IConfigurationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        /// <param name="eventManager">
        /// The event manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public ConfigurationService(
            IDscEventManager eventManager, 
            DeploymentServerContext context, 
            IDeploymentServerOptions options, 
            IDeploymentServerLogging logging)
            : base(eventManager)
        {
            this.Context = context;
            this.Repository = context.Set<Configuration>();
            this.Options = options;
            this.Logging = logging;
        }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        protected DeploymentServerContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected IDeploymentServerLogging Logging { get; set; }

        /// <summary>
        ///     Gets or sets the options.
        /// </summary>
        protected IDeploymentServerOptions Options { get; set; }

        /// <summary>
        ///     Gets or sets the repository.
        /// </summary>
        protected DbSet<Configuration> Repository { get; set; }

        /// <summary>
        ///     The cleanup archives.
        /// </summary>
        public void CleanupArchives()
        {
            var archives =
                this.Repository.Where(a => a.ArchiveTimestamp != null)
                    .OrderByDescending(a => a.ArchiveTimestamp)
                    .GroupBy(a => a.Target);
            foreach (var archive in archives.Where(g => g.Count() > this.Options.PreviousConfigurationsStored).ToList())
            {
                var toRemove = archive.Skip(this.Options.PreviousConfigurationsStored).ToArray();
                this.Logging?.DeleteArchiveConfigurations(archive.Key, toRemove.Length);
                this.Repository.RemoveRange(toRemove);
                this.Context.SaveChanges();
            }
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
        public override async Task HandleEvent(DscEventArgs eventArgs)
        {
            if (eventArgs.Name == "DeleteNode")
            {
                var name = eventArgs.GetMember<string>("NodeName");
                var configs = this.Repository.Where(r => r.Target == name);
                if (configs.Any())
                {
                    this.Repository.RemoveRange(configs);
                    this.Logging.UnpublishConfigurationsNodeRemoved(configs.Count(), name);
                    await this.Context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        public void Publish(string document, string target, string certificateThumbprint)
        {
            var configuration = new ConfigurationRequest { Document = document, Target = target, CertificateThumbprint = certificateThumbprint};
            var entities = this.Repository.Where(c => c.Target == configuration.Target && c.ArchiveTimestamp == null);
            foreach (var entity in entities)
            {
                entity.ArchiveTimestamp = DateTime.UtcNow;
                this.Context.Entry(entity).State = EntityState.Modified;
                this.Logging?.ConfigurationArchived(entity.Target, entity.ConfigurationDocumentId);
            }

            var config = configuration.ToModel();
            this.Repository.Add(config);
            this.Context.SaveChanges();
            this.Logging?.ConfigurationPublished(config.Target, config.ConfigurationDocumentId);
        }

        /// <summary>
        /// The on configuration published.
        /// </summary>
        /// <param name="configurationEventArgs">
        /// The configuration event args.
        /// </param>
        protected virtual void OnConfigurationPublished(PublishedConfigurationEventArgs configurationEventArgs)
        {
        }
    }
}