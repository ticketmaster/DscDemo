// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPropertyUpdateRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The configuration property update request.
    /// </summary>
    public class ConfigurationPropertyUpdateRequest : ConfigurationPropertyRequest
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}