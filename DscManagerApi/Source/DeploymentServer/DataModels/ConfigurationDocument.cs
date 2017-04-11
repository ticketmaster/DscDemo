// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationDocument.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DeploymentServer.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     The configuration document.
    /// </summary>
    public class ConfigurationDocument
    {
        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        /// <summary>
        ///     Gets or sets the document.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}