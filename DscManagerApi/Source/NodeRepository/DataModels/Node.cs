// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;
    using Ticketmaster.Dsc.NodeRepository.DataAccess;
    using Ticketmaster.Dsc.Scheduling;

    /// <summary>
    ///     The node.
    /// </summary>
    public class Node : ModelBase<NodeView>
    {
        /// <summary>
        ///     The configuration properties populated.
        /// </summary>
        [NotMapped]
        internal bool ConfigurationPropertiesPopulated = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node" /> class.
        /// </summary>
        public Node()
        {
            var detailView = this.AddTypeMapping<NodeDetailView>();
            detailView.AddTypeMapping(typeof(MaintenanceSchedule), typeof(MaintenanceScheduleView))
                .AddSourceMemberPropertyResolver<MaintenanceSchedule>(m => m.TimeZone, m => m.TimeZoneId);
            this.TypeMappings.AddSourceMemberPropertyResolver<Node>(n => n.Roles, n => n.Roles.Select(r => r.Name));
        }

        /// <summary>
        ///     Gets or sets the bootstrap properties.
        /// </summary>
        [NotMapped]
        public ICollection<ConfigurationProperty> BootstrapProperties { get; set; } = new List<ConfigurationProperty>();

        /// <summary>
        ///     Gets or sets the class.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string Class { get; set; }

        /// <summary>
        ///     Gets or sets the configuration environment.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string ConfigurationEnvironment { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is initial deployment.
        /// </summary>
        public bool IsInitialDeployment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is in maintenance.
        /// </summary>
        public bool IsInMaintenance { get; set; }

        /// <summary>
        ///     Gets or sets the local agent properties.
        /// </summary>
        [NotMapped]
        public ICollection<ConfigurationProperty> LocalAgentProperties { get; set; } = new List<ConfigurationProperty>()
            ;

        /// <summary>
        ///     Gets or sets the maintenance schedule.
        /// </summary>
        public MaintenanceSchedule MaintenanceSchedule { get; set; } = new MaintenanceSchedule { TimeZoneId = "Pacific Standard Time" };

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [Index(IsUnique = true)]
        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the node name.
        /// </summary>
        [Index(IsUnique = true)]
        [StringLength(128)]
        [Required]
        public string NodeName { get; set; }

        /// <summary>
        ///     Gets or sets the node properties.
        /// </summary>
        [NotMapped]
        public ICollection<ConfigurationProperty> NodeProperties { get; set; } = new List<ConfigurationProperty>();

        [NotMapped]
        public ICollection<ConfigurationProperty> ResourceVersionProperties { get; set; } = new List<ConfigurationProperty>();

        /// <summary>
        ///     Gets or sets the product.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string Product { get; set; }

        /// <summary>
        ///     Gets or sets the qualified name.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string QualifiedName { get; set; }

        /// <summary>
        ///     Gets or sets the roles.
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        /// <summary>
        ///     Gets or sets the site.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        [Required]
        public string Site { get; set; }

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Compare(Hashtable configurationData)
        {
            var isDirty = false;
            var excludedProperties = new[]
                                         {
                                             "MaintenanceSchedule", "NodeProperties", "LocalAgentProperties", "BootstrapProperties",
                                             "IsInMaintenance"
                                         };
            var properties =
                this.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !excludedProperties.Contains(p.Name))
                    .ToArray();
            foreach (DictionaryEntry entry in configurationData)
            {
                // Determine if the property is on the top-level Node object
                var currentProperty = properties.FirstOrDefault(p => p.Name == (string)entry.Key);
                if (currentProperty != null && currentProperty.GetValue(this) != entry.Value)
                {
                    isDirty = true;
                    continue;
                }

                // Handle resource versions
                if ((string)entry.Key == "ResourceVersionProperties")
                {
                    var table = entry.Value as Hashtable;
                    if (table == null)
                    {
                        continue;
                    }

                    foreach (DictionaryEntry resource in table)
                    {
                        var property = this.ResourceVersionProperties.FirstOrDefault(r => r.Name == (string)resource.Key);
                        if (property == null)
                        {
                            isDirty = true;
                        }
                        else
                        {
                            if ((string)resource.Value != (string)property.Value)
                            {
                                isDirty = true;
                            }
                        }
                    }

                    var allResourceProps = this.ResourceVersionProperties.Where(r => r.Scope == PropertyScope.Node).ToList();
                    allResourceProps.ForEach(
                        r =>
                        {
                            if (!table.ContainsKey(r.Name))
                            {
                                isDirty = true;
                            }
                        });
                }

                // Handle maintenance schedules
                if ((string)entry.Key == "MaintenanceSchedule")
                {
                    var newSchedule = entry.Value as MaintenanceScheduleView;
                    if (newSchedule == null)
                    {
                        continue;
                    }

                    if (this.MaintenanceSchedule.TimeZoneId != newSchedule.TimeZone)
                    {
                        isDirty = true;
                        continue;
                    }

                    var newScheduleData = newSchedule.GetScheduleData();
                    if (this.MaintenanceSchedule.ScheduleData != newScheduleData)
                    {
                        isDirty = true;
                        continue;
                    }
                }

                // Must be a node property
                var nodeProperty = this.NodeProperties.FirstOrDefault(p => p.Name == (string)entry.Key);
                if (nodeProperty == null)
                {
                    isDirty = true;
                }
                else
                {
                    var table = entry.Value as IDictionary;
                    var destTable = nodeProperty.Value as IDictionary;
                    var isTableDirty = this.CompareDictionary(table, destTable);
                    if (isTableDirty.HasValue)
                    {
                        isDirty = isTableDirty.Value;
                    }
                    else
                    {
                        var entryValue = JsonConvert.SerializeObject(
                            entry.Value,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                        if (nodeProperty.SerializedValue != entryValue)
                        {
                            isDirty = true;
                        }
                    }
                }
            }

            return isDirty;
        }

        private bool? CompareDictionary(IDictionary source, IDictionary destination)
        {
            if (source == null || destination == null)
            {
                return null;
            }

            if (source.Cast<DictionaryEntry>().Where(de => destination.Contains(de.Key)).Any(de => destination[de.Key] != de.Value))
            {
                return false;
            }

            return destination.Cast<DictionaryEntry>().Where(de => source.Contains(de.Key)).All(de => source[de.Key] == de.Value);
        }

        /// <summary>
        /// The get detail view.
        /// </summary>
        /// <returns>
        /// The <see cref="NodeDetailView"/>.
        /// </returns>
        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="configurationData">
        /// The configuration data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public bool Merge(Hashtable configurationData, NodeRepositoryContext context)
        {
            if (!configurationData.ContainsKey("NodeName"))
            {
                throw new InvalidOperationException(
                    "Cannot process not configuration that is missing the NodeName property.");
            }

            this.Name = configurationData["NodeName"].ToString();

            var isDirty = false;
            var excludedProperties = new[]
                                         {
                                             "MaintenanceSchedule", "Roles", "NodeProperties", "LocalAgentProperties", "ResourceVersionProperties",
                                             "BootstrapProperties", "IsInMaintenance"
                                         };
            var properties =
                this.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !excludedProperties.Contains(p.Name))
                    .ToArray();
            foreach (DictionaryEntry entry in configurationData)
            {
                // Determine if the property is on the top-level Node object
                var currentProperty = properties.FirstOrDefault(p => p.Name == (string)entry.Key);
                if (currentProperty != null)
                {
                    if (currentProperty?.GetValue(this)?.ToString() != entry.Value.ToString())
                    {
                        currentProperty.SetValue(this, entry.Value);
                        isDirty = true;
                    }

                    continue;
                }

                // Handle resource versions
                if ((string)entry.Key == "ResourceVersionProperties")
                {
                    var table = (entry.Value as JObject)?.ToObject<Hashtable>();
                    if (table == null)
                    {
                        continue;
                    }

                    foreach (DictionaryEntry resource in table)
                    {
                        var property = this.ResourceVersionProperties.FirstOrDefault(r => r.Name == (string)resource.Key);
                        if (property == null)
                        {
                            var rprop = new ConfigurationProperty
                                            {
                                                Name = (string)resource.Key,
                                                Scope = PropertyScope.Node,
                                                Target = this.NodeName,
                                                Type = PropertyType.ResourceVersion,
                                                Value = (string)resource.Value
                                            };
                            this.ResourceVersionProperties.Add(rprop);
                            context.ConfigurationProperties.Add(rprop);
                            isDirty = true;
                        }
                        else
                        {
                            if ((string)resource.Value != (string)property.Value)
                            {
                                if (property.Scope == PropertyScope.Node)
                                {
                                    property.Value = (string)resource.Value;
                                }
                                else
                                {
                                    var rprop = new ConfigurationProperty
                                    {
                                        Name = (string)resource.Key,
                                        Scope = PropertyScope.Node,
                                        Target = this.NodeName,
                                        Type = PropertyType.ResourceVersion,
                                        Value = (string)resource.Value
                                    };
                                    this.ResourceVersionProperties.Add(rprop);
                                    context.ConfigurationProperties.Add(rprop);
                                }

                                isDirty = true;
                            }
                        }
                    }

                    var allResourceProps = this.ResourceVersionProperties.Where(r => r.Scope == PropertyScope.Node).ToList();
                    allResourceProps.ForEach(
                        r =>
                            {
                                if (!table.ContainsKey(r.Name))
                                {
                                    this.ResourceVersionProperties.Remove(r);
                                    isDirty = true;
                                }
                            });
                    continue;
                }

                // Handle maintenance schedules
                if ((string)entry.Key == "MaintenanceSchedule")
                {
                    // try
                    // {
                    var token = entry.Value as JToken;
                    var json = token?.ToString();
                    var newSchedule = JsonConvert.DeserializeObject<MaintenanceScheduleView>(
                        json, 
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

                    if (newSchedule == null)
                    {
                        continue;
                    }

                    if (this.MaintenanceSchedule == null)
                    {
                        this.MaintenanceSchedule = new MaintenanceSchedule();
                    }

                    if (this.MaintenanceSchedule.TimeZoneId == null
                        || this.MaintenanceSchedule.TimeZoneId != newSchedule.TimeZone)
                    {
                        this.MaintenanceSchedule.TimeZoneId = newSchedule.TimeZone;
                        isDirty = true;
                    }

                    var newScheduleData = newSchedule.GetScheduleData();
                    if (this.MaintenanceSchedule.ScheduleData != newScheduleData)
                    {
                        this.MaintenanceSchedule.ScheduleData = newScheduleData;
                        isDirty = true;
                    }

                    continue;
                }

                if ((string)entry.Key == "Roles")
                {
                    var currentRoleNames = this.Roles.Select(r => r.Name).ToArray();
                    var roles = (entry.Value as JArray)?.ToObject<IEnumerable<string>>().ToArray();

                    if (roles != null)
                    {
                        foreach (var role in roles.Where(role => !currentRoleNames.Contains(role)))
                        {
                            this.Roles.Add(new Role { Name = role, Node = this });
                            isDirty = true;
                        }

                        foreach (var removeRole in currentRoleNames.Where(role => !roles.Contains(role)))
                        {
                            var toRemove = this.Roles.FirstOrDefault(r => r.Name == removeRole);
                            this.Roles.Remove(toRemove);
                            context?.Set<Role>()?.Remove(toRemove);
                            isDirty = true;
                        }
                    }

                    continue;
                }

                // Must be a node property
                var nodeProperty = this.NodeProperties.FirstOrDefault(p => p.Name == (string)entry.Key);
                if (nodeProperty == null)
                {
                    var cProp = new ConfigurationProperty
                                    {
                                        Name = (string)entry.Key, 
                                        Target = this.Name, 
                                        Scope = PropertyScope.Node, 
                                        Type = PropertyType.Node, 
                                        Value = entry.Value
                                    };
                    this.NodeProperties.Add(cProp);
                    context?.Set<ConfigurationProperty>()?.Add(cProp);
                    isDirty = true;
                }
                else
                {
                    var localDirty = false;
                    var table = entry.Value as IDictionary;
                    var destTable = nodeProperty.Value as IDictionary;
                    var isTableDirty = this.CompareDictionary(table, destTable);
                    if (isTableDirty.HasValue)
                    {
                        localDirty = isTableDirty.Value;
                    }
                    else
                    {
                        var entryValue = JsonConvert.SerializeObject(
                            entry.Value,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                        if (nodeProperty.SerializedValue != entryValue)
                        {
                            localDirty = true;
                        }
                    }
                    if (localDirty)
                    {
                        if (nodeProperty.Scope == PropertyScope.Node)
                        {
                            nodeProperty.Value = entry.Value;
                            isDirty = true;
                        }
                        else
                        {
                            var cProp = new ConfigurationProperty
                                            {
                                                Name = (string)entry.Key,
                                                Target = this.Name,
                                                Scope = PropertyScope.Node,
                                                Type = PropertyType.Node,
                                                Value = entry.Value
                                            };
                            this.NodeProperties.Add(cProp);
                            context?.Set<ConfigurationProperty>()?.Add(cProp);
                            isDirty = true;
                        }
                    }
                }
            }

            // Check if all node properties are needed
            var propsToRemove =
                this.NodeProperties.Where(
                    nodeProp => !configurationData.ContainsKey(nodeProp.Name) && nodeProp.Scope == PropertyScope.Node)
                    .ToList();
            propsToRemove.ForEach(
                n =>
                    {
                        this.NodeProperties.Remove(n);
                        context?.Set<ConfigurationProperty>()?.Remove(n);
                        isDirty = true;
                    });

            return isDirty;
        }
    }
}