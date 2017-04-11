// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeploymentServerContextInitializer.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;

    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DeploymentServer.Logging;
    using Ticketmaster.Dsc.Interfaces.DeploymentServer;

    /// <summary>
    ///     The deployment server context initializer.
    /// </summary>
    public class DeploymentServerContextInitializer : MigrateDatabaseToLatestVersion<DeploymentServerContext, DeploymentServerContextMigrationConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentServerContextInitializer"/> class.
        /// </summary>
        /// <param name="deploymentServerLogging">
        /// The deployment server logging.
        /// </param>
        public DeploymentServerContextInitializer(IDeploymentServerLogging deploymentServerLogging)
        {
            this.DeploymentServerLogging = deploymentServerLogging;
        }

        /// <summary>
        ///     Gets or sets the deployment server logging.
        /// </summary>
        protected IDeploymentServerLogging DeploymentServerLogging { get; set; }

        public override void InitializeDatabase(DeploymentServerContext context)
        {
            if (context.Database.Exists() && !context.Database.CompatibleWithModel(false))
            {
                this.DeploymentServerLogging.UpgradeDeploymentServerDatabaseSchema();
                this.Seed(context);
            }

            base.InitializeDatabase(context);
        }

        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        ///     The default implementation does nothing.
        /// </summary>
        /// <param name="context">
        /// The context to seed.
        /// </param>
        protected void Seed(DeploymentServerContext context)
        {
            this.DeploymentServerLogging?.BeginDeploymentServerDatabaseSeed(this.GetType().Name);

            /*var set = context.Set<Configuration>();

            var mof = File.ReadAllText(@"{0AB5FA3D-57AB-11E5-80C6-E0DB5501AFDD}.txt");

            var configs = new List<Configuration>();
            for (var i = 1; i <= 1000; i++)
            {
                var config = set.Create();
                config.Target = "Test" + i;
                config.Checksum = "A";
                config.PublishedTimestamp = DateTime.UtcNow;
                config.ConfigurationDocument = new ConfigurationDocument { Document = mof };
                configs.Add(config);

                if ((i & 1) == 1)
                {
                    var archive = set.Create();
                    archive.Target = "Test" + i;
                    archive.Checksum = "A";
                    archive.PublishedTimestamp = DateTime.UtcNow;
                    archive.ArchiveTimestamp = DateTime.UtcNow;
                    archive.ConfigurationDocument = new ConfigurationDocument { Document = mof };
                    configs.Add(archive);
                }
            }

            context.Set<Configuration>().AddRange(configs);

            // Builds
            var builds = new List<Build>();
            for (var i = 1; i <= 1000; i++)
            {
                var rng = new Random();

                var build = new Build
                                {
                                    RequestTimestamp = DateTime.UtcNow.AddHours(rng.Next(-72, 0)), 
                                    CompleteTimestamp = DateTime.UtcNow, 
                                    Status = (BuildStatus)rng.Next(0, 3)
                                };

                build.Targets = new List<BuildTarget>();
                for (var tc = 1; tc <= rng.Next(1, 50); tc++)
                {
                    var job = rng.Next(1, 100);
                    build.Targets.Add(new BuildTarget { Build = build, JobId = job, Target = "Target" + tc });
                }

                builds.Add(build);
            }

            context.Set<Build>().AddRange(builds);
            context.SaveChanges();
            base.Seed(context);*/
            this.DeploymentServerLogging?.EndDeploymentServerDatabaseSeed(this.GetType().Name);
        }
    }
}