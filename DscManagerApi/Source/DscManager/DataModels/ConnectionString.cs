// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionString.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataModels
{
    using System.Configuration;
    using System.Reflection;

    /// <summary>
    /// The connection string entry.
    /// </summary>
    public class ConnectionStringEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringEntry"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="providerName">
        /// The provider name.
        /// </param>
        public ConnectionStringEntry(string name, string connectionString, string providerName)
        {
            this.Name = name;
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
            typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(ConfigurationManager.ConnectionStrings, false);
            if (ConfigurationManager.ConnectionStrings[name] != null)
            {
                ConfigurationManager.ConnectionStrings.Remove(name);
            }

            ConfigurationManager.ConnectionStrings.Add(
                new ConnectionStringSettings(name, connectionString, providerName));
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }
    }
}