// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPropertyRequest.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;

    /// <summary>
    ///     The configuration property request.
    /// </summary>
    public class ConfigurationPropertyRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether build mof.
        /// </summary>
        public bool BuildMof { get; set; } = true;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public PropertyScope Scope { get; set; }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Required]
        public string Target { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public PropertyType Type { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        [Required]
        public object Value { get; set; }
    }
}