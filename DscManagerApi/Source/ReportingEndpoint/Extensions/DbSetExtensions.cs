// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSetExtensions.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.ReportingEndpoint.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.ReportingEndpoint.ViewModels;
    using Ticketmaster.Dsc.ReportingEndpoint.DataModels;

    /// <summary>
    /// The db set extensions.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// The to configuration detail view.
        /// </summary>
        /// <param name="set">
        /// The set.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ConfigurationReportDetailView> ToConfigurationDetailView(
            this IEnumerable<ConfigurationReport> set)
        {
            var reportGroups = set.GroupBy(r => r.RunId);
            var results = new List<ConfigurationReportDetailView>();
            foreach (var group in reportGroups)
            {
                var numberOfRuns = group.Count();
                var runs = group.Select(item => item.ToViewModel());

                var first = group.First();
                var last = group.Last();
                var span = last.EndDate - first.StartDate;
                var tm = new TypeMapping(typeof(ConfigurationReport), typeof(ConfigurationReportDetailView));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(d => d.RunId, () => group.Key));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.StartDate, 
                        () => first.StartDate));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.DurationInSeconds, 
                        () => Convert.ToInt32(span.TotalSeconds)));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.EndDate, 
                        () => last.EndDate));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.NumberOfResources, 
                        () => last.NumberOfResources));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.NumberOfRuns, 
                        () => numberOfRuns));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.Status, 
                        () => last.Status));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.Target, 
                        () => last.Target));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(d => d.Type, () => last.Type));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.ConfigurationRuns, 
                        () => runs));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.ConfigurationPackageVersion, 
                        () => last.ConfigurationPackageVersion));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportDetailView>(
                        d => d.ConfigurationPackageName,
                        () => last.ConfigurationPackageName));
                results.Add(tm.Map(null) as ConfigurationReportDetailView);
            }

            return results;
        }

        /// <summary>
        /// The to configuration views.
        /// </summary>
        /// <param name="set">
        /// The set.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ConfigurationReportView> ToConfigurationViews(
            this IEnumerable<ConfigurationReport> set)
        {
            var reportGroups = set.GroupBy(r => r.RunId);
            var results = new List<ConfigurationReportView>();
            foreach (var group in reportGroups)
            {
                var g = group.OrderBy(e => e.StartDate);
                var numberOfRuns = group.Count();
                var first = g.First();
                var last = g.Last();
                var span = last.EndDate - first.StartDate;
                var tm = new TypeMapping(typeof(ConfigurationReport), typeof(ConfigurationReportView));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(d => d.RunId, () => group.Key));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.StartDate, 
                        () => first.StartDate));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.DurationInSeconds, 
                        () => Convert.ToInt32(span.TotalSeconds)));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(d => d.EndDate, () => last.EndDate));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.NumberOfResources, 
                        () => last.NumberOfResources));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.NumberOfRuns, 
                        () => numberOfRuns));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(d => d.Status, () => last.Status));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(d => d.Target, () => last.Target));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(d => d.Type, () => last.Type));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.ConfigurationPackageVersion, 
                        () => last.ConfigurationPackageVersion));
                tm.PropertyResolvers.Add(
                    new DestinationMemberPropertyResolver<ConfigurationReportView>(
                        d => d.ConfigurationPackageName,
                        () => last.ConfigurationPackageName));
                results.Add(tm.Map(null) as ConfigurationReportView);
            }

            return results;
        }
    }
}