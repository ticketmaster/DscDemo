// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaintenanceScheduleView.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.Scheduling;

    /// <summary>
    ///     The maintenance schedule view.
    /// </summary>
    public class MaintenanceScheduleView
    {
        /// <summary>
        ///     Gets or sets the schedule entries.
        /// </summary>
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Objects)]
        public IEnumerable<IMaintenanceScheduleEntry> ScheduleEntries { get; set; }

        /// <summary>
        ///     Gets or sets the time zone.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        ///     The get schedule data.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetScheduleData()
        {
            return JsonConvert.SerializeObject(
                this.ScheduleEntries, 
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
        }
    }
}