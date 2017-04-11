// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Ticketmaster.Dsc.Interfaces.DeploymentServer.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The configuration.
    /// </summary>
    public class Configuration : ModelBase<ConfigurationView>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Configuration" /> class.
        /// </summary>
        public Configuration()
        {
            this.AddTypeMapping<ArchiveConfigurationView>();
            var map = this.AddTypeMapping<ConfigurationDocumentView>();
            map.PropertyResolvers.Add(
                new DestinationPropertyFromSourcePropertyResolver<Configuration, ConfigurationDocumentView>(
                    d => d.Document, 
                    s => s.ConfigurationDocument.Document));
        }

        /// <summary>
        ///     Gets or sets the archive timestamp.
        /// </summary>
        [Column(TypeName = "DateTime2")]
        [Index(IsUnique = false)]
        public DateTime? ArchiveTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the certificate thumbprint.
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        ///     Gets or sets the checksum.
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        ///     Gets or sets the configuration document.
        /// </summary>
        [ForeignKey("ConfigurationDocumentId")]
        public virtual ConfigurationDocument ConfigurationDocument { get; set; }

        /// <summary>
        ///     Gets or sets the configuration document id.
        /// </summary>
        public int ConfigurationDocumentId { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the published tmestamp.
        /// </summary>
        [Index(IsUnique = false)]
        [Column(TypeName = "DateTime2")]
        public DateTime PublishedTimestamp { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(128)]
        public string Target { get; set; }
    }
}