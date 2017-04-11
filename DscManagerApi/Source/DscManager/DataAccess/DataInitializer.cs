namespace Ticketmaster.Dsc.DscManager.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.AccessControl;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.DeploymentServer.DataAccess;
    using Ticketmaster.Dsc.DscManager.DataModels;
    using Ticketmaster.Dsc.Interfaces.DscManager;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataAccess;

    public class DataInitializer : IDataInitializer
    {
        public DataInitializer(
            DscManagerContext dscContext,
            DeploymentServerContext deploymentContext,
            NodeRepositoryContext nodeContext,
            ReportingEndpointContext reportingContext,
            IDscManagerOptions options)
        {
            this.DscContext = dscContext;
            this.DeploymentContext = deploymentContext;
            this.NodeContext = nodeContext;
            this.ReportingContext = reportingContext;
            this.Options = options;
        }

        protected IDscManagerOptions Options { get; set; }

        protected DeploymentServerContext DeploymentContext { get; set; }

        protected DscManagerContext DscContext { get; set; }

        protected NodeRepositoryContext NodeContext { get; set; }

        protected ReportingEndpointContext ReportingContext { get; set; }

        public void Initialize()
        {
            // Node configuration properties
            var properties = this.NodeContext.Set<ConfigurationProperty>();
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
                                Value = this.Options.DeploymentServerOptions.DscManagerEndpoint
                            },
                        new ConfigurationProperty
                            {
                                Name = "ReportingEndpoint",
                                Target = "global",
                                Scope = PropertyScope.Global,
                                Type = PropertyType.LocalAgent,
                                Value = this.Options.DeploymentServerOptions.DscManagerEndpoint
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
                                Value = "1.0.14"
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
                                Value = "1.0.11"
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
                                Value = "1.0.19"
                            }
                    });

            this.NodeContext.SaveChanges();
        }

        public bool ShouldRunInitializer()
        {
            return !this.NodeContext.ConfigurationProperties.Any(c => c.Name == "ConfigurationPackageName");
        }
    }
}