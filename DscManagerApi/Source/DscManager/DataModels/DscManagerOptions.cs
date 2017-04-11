// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscManagerOptions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Http;

    using Newtonsoft.Json;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.DeploymentServer.DataModels;
    using Ticketmaster.Dsc.DscManager.DataAccess;
    using Ticketmaster.Dsc.NodeRepository.DataModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The dsc manager options.
    /// </summary>
    public class DscManagerOptions : IDscManagerOptions
    {
        /// <summary>
        /// Gets or sets the connection strings.
        /// </summary>
        [Required]
        public ICollection<ConnectionStringEntry> ConnectionStrings { get; set; } = new List<ConnectionStringEntry>
                                                                                        {
                                                                                            new ConnectionStringEntry
                                                                                                (
                                                                                                "DscManager", 
                                                                                                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\ProgramData\TempDatabase\DscManager.mdf;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True;", 
                                                                                                "System.Data.SqlClient")
                                                                                        };

        /// <summary>
        /// Gets or sets the credential repository options.
        /// </summary>
        public ICredentialRepositoryOptions CredentialRepositoryOptions { get; set; } =
            new CredentialRepositoryOptions(
                "79F06042A200894D61504BE2CAC9574688343CAC", 
                StoreLocation.CurrentUser, 
                "My", 
                "DscManager") { DatabaseInitializer = new CredentialRepositoryInitializer(), UseHttpAuth = false };

        /// <summary>
        /// Gets or sets the days to keep logs.
        /// </summary>
        public int DaysToKeepLogs { get; set; } = 14;

        /// <summary>
        /// Gets or sets the deployment server options.
        /// </summary>
        public IDeploymentServerOptions DeploymentServerOptions { get; set; } = new DeploymentServerOptions
                                                                                    {
                                                                                        UseJobDashbaord
                                                                                            =
                                                                                            true, 
                                                                                        NameOrConnectionString
                                                                                            =
                                                                                            "DscManager"
                                                                                    };

        /// <summary>
        /// Gets or sets the error detail policy.
        /// </summary>
        public IncludeErrorDetailPolicy ErrorDetailPolicy { get; set; } = IncludeErrorDetailPolicy.LocalOnly;

        /// <summary>
        /// Gets or sets the node repository options.
        /// </summary>
        public INodeRepositoryOptions NodeRepositoryOptions { get; set; } = new NodeRepositoryOptions
                                                                                {
                                                                                    NameOrConnectionString
                                                                                        =
                                                                                        "DscManager"
                                                                                };

        /// <summary>
        /// Gets or sets the reporting endpoint options.
        /// </summary>
        public IReportingEndpointOptions ReportingEndpointOptions { get; set; } = new ReportingEndpointOptions
                                                                                      {
                                                                                          NameOrConnectionString
                                                                                              =
                                                                                              "DscManager"
                                                                                      };

        /// <summary>
        /// Gets or sets a value indicating whether use api.
        /// </summary>
        public bool UseApi { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether use hangfire job server.
        /// </summary>
        public bool UseHangfireJobServer { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether use job dashboard.
        /// </summary>
        public bool UseJobDashboard { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether use slack logging.
        /// </summary>
        public bool UseSlackLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the worker count.
        /// </summary>
        public int WorkerCount { get; set; } = 4;

        /// <summary>
        /// The read from file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="IDscManagerOptions"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static IDscManagerOptions ReadFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    var options = JsonConvert.DeserializeObject<DscManagerOptions>(
                        File.ReadAllText(filename), 
                        new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto, 
                                Formatting = Formatting.Indented
                            });
                    return options;
                }
                catch (Exception e)
                {
                    throw new Exception("The configuration file exists, but could not be read or parse.", e);
                }
            }

            var defaultOptions = new DscManagerOptions();
            return defaultOptions;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public void Save(string filename)
        {
            try
            {
                File.WriteAllText(
                    filename, 
                    JsonConvert.SerializeObject(
                        this, 
                        new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto, 
                                Formatting = Formatting.Indented
                            }));
            }
            catch (Exception e)
            {
                throw new Exception("Failed to write configuration file.", e);
            }
        }
    }
}