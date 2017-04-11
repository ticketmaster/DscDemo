// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationReportResourceEntity.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.DataModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    /// <summary>
    ///     The configuration report resource entity.
    /// </summary>
    public class ConfigurationReportResourceEntity
    {
        /// <summary>
        ///     Gets or sets the configuration resources.
        /// </summary>
        public string ConfigurationResources { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the resources.
        /// </summary>
        [NotMapped]
        public IEnumerable<ConfigurationReportResource> Resources
        {
            get
            {
                return
                    JsonConvert.DeserializeObject<IEnumerable<ConfigurationReportResource>>(this.ConfigurationResources);
            }

            set
            {
                this.ConfigurationResources = JsonConvert.SerializeObject(value);
            }
        }
    }
}