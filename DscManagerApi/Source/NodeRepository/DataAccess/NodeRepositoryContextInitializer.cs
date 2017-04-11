// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeRepositoryContextInitializer.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.NodeRepository.Logging;
    using Ticketmaster.Dsc.Scheduling;
    using Ticketmaster.Dsc.Scheduling.Extensions;

    /// <summary>
    ///     The default initializer.
    /// </summary>
    public class NodeRepositoryContextInitializer : MigrateDatabaseToLatestVersion<NodeRepositoryContext, NodeRepositoryContextMigrationConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeRepositoryContextInitializer"/> class.
        /// </summary>
        /// <param name="logging">
        /// The logging.
        /// </param>
        public NodeRepositoryContextInitializer(INodeRepositoryLogging logging)
        {
            this.Logging = logging;
        }

        /// <summary>
        ///     Gets or sets the logging.
        /// </summary>
        protected INodeRepositoryLogging Logging { get; set; }

        /// <inheritdoc />
        public override void InitializeDatabase(NodeRepositoryContext context)
        {
            if (context.Database.Exists() && !context.Database.CompatibleWithModel(false))
            {
                this.Logging.UpgradeNodeRepositoryDatabaseSchema();
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
        protected void Seed(NodeRepositoryContext context)
        {
            this.Logging?.BeginNodeRepositoryDatabaseSeed(this.GetType().Name);

            //var nodes = context.Set<Node>();
            var properties = context.Set<ConfigurationProperty>();
            if (properties.Any())
            {
                return;
            }

            properties.AddRange(
                new List<ConfigurationProperty>
                    {
                        new ConfigurationProperty
                            {
                                Name = "ConfigurationEndpoint", 
                                Target = "global", 
                                Scope = PropertyScope.Global, 
                                Type = PropertyType.LocalAgent, 
                                Value =
                                    "http://dsc.winsys.tmcs"
                            },
                        new ConfigurationProperty
                            {
                                Name = "ReportingEndpoint", 
                                Target = "global", 
                                Scope = PropertyScope.Global, 
                                Type = PropertyType.LocalAgent, 
                                Value =
                                    "http://dsc.winsys.tmcs"
                            },
                        new ConfigurationProperty
                            {
                                Name = "MonitorInterval",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.LocalAgent,
                                Value = 180
                            },
                        new ConfigurationProperty
                            {
                                Name = "PackageName",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.LocalAgent,
                                Value = "DscLocalActions"
                            },
                        new ConfigurationProperty
                            {
                                Name = "PackageVersion",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.LocalAgent,
                                Value = "1.0"
                            },
                        new ConfigurationProperty
                            {
                                Name = "NodeAgentVersion",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.LocalAgent,
                                Value = "1.0.0.0"
                            },
                        new ConfigurationProperty
                            {
                                Name = "RepositoryUri",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Bootstrap,
                                Value =
                                    "http://psget.winsys.tmcs/api/odata"
                            },
                        new ConfigurationProperty
                            {
                                Name = "BootstrapInterval",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Bootstrap,
                                Value = 360
                            },
                        new ConfigurationProperty
                            {
                                Name = "PackageName",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Bootstrap,
                                Value = "DscBootstrap"
                            },
                        new ConfigurationProperty
                            {
                                Name = "PackageVersion",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Bootstrap,
                                Value = "1.0"
                            },
                        new ConfigurationProperty
                            {
                                Name = "ConfigurationPackageName",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Node,
                                Value = "DscConfigurations"
                            },
                        new ConfigurationProperty
                            {
                                Name = "ConfigurationPackageVersion",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.Node,
                                Value = "1.0"
                            }
                    });

            context.SaveChanges();
            /*var maint = new MaintenanceSchedule { TimeZoneId = "Pacific Standard Time" };
            maint.AddWeeklyMaintenance(
                new DateTime(1, 1, 1, 0, 0, 0), 
                new DateTime(1, 1, 1, 4, 0, 0), 
                DayOfWeek.Tuesday);

            nodes.AddRange(
                new List<Node>
                    {
                        new Node
                            {
                                Class = "dsc", 
                                ConfigurationEnvironment = "Production", 
                                Name = "dsc1techash2", 
                                NodeName = "dsc1techash2", 
                                IsInitialDeployment = false, 
                                Product = "techops", 
                                QualifiedName = "dsc1.techops.ash2.winsys.tmcs", 
                                Site = "ash2", 
                                MaintenanceSchedule = maint, 
                                Roles = new List<Role> { new Role { Name = "dsc" } }
                            }, 
                        new Node
                            {
                                Class = "sdb", 
                                ConfigurationEnvironment = "Production", 
                                Name = "sdb1arcash2", 
                                NodeName = "sdb1arcash2", 
                                IsInitialDeployment = true, 
                                Product = "arc", 
                                QualifiedName = "sdb1.arc.ash2.winsys.tmcs", 
                                Site = "ash2", 
                                MaintenanceSchedule = maint, 
                                Roles = new List<Role> { new Role { Name = "sdb" } }
                            }, 
                        new Node
                            {
                                Class = "webapp", 
                                ConfigurationEnvironment = "Production", 
                                Name = "webapp1arcphx2", 
                                NodeName = "webapp1arcphx2", 
                                IsInitialDeployment = false, 
                                Product = "arc", 
                                QualifiedName = "webapp1.arc.phx2.winsys.tmcs", 
                                Site = "phx2", 
                                MaintenanceSchedule = maint, 
                                Roles =
                                    new List<Role>
                                        {
                                            new Role { Name = "webapp" }, 
                                            new Role { Name = "ATG" }
                                        }
                            }
                    });
            context.SaveChanges();
            base.Seed(context);*/
            this.Logging?.EndNodeRepositoryDatabaseSeed(this.GetType().Name);
        }
    }
}