// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CentralizedPrefixProvider.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Http
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;

    /// <summary>
    ///     The centralized prefix provider.
    /// </summary>
    public class CentralizedPrefixProvider : DefaultDirectRouteProvider
    {
        /// <summary>
        ///     The _centralized prefix.
        /// </summary>
        private readonly string centralizedPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralizedPrefixProvider"/> class.
        /// </summary>
        /// <param name="centralizedPrefix">
        /// The centralized prefix.
        /// </param>
        public CentralizedPrefixProvider(string centralizedPrefix)
        {
            this.centralizedPrefix = centralizedPrefix;
        }

        /// <summary>
        /// The get route prefix.
        /// </summary>
        /// <param name="controllerDescriptor">
        /// The controller descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            return existingPrefix == null ? this.centralizedPrefix : $"{this.centralizedPrefix}/{existingPrefix}";
        }
    }
}