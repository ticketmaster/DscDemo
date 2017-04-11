// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingEntity.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.DataModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Ticketmaster.Dsc.DscManager.ViewModels;
    using Ticketmaster.Dsc.Interfaces.Mapping;

    /// <summary>
    ///     The logging entity.
    /// </summary>
    public class LoggingEntity : ModelBase<LoggingView>
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        public string Server { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        [Index(IsUnique = false)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        [Index(IsUnique = false)]
        [StringLength(256)]
        public string Username { get; set; }
    }
}