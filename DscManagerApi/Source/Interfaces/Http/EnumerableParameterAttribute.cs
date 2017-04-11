// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableParameterAttribute.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    /// <summary>
    ///     The enumerable parameter attribute.
    /// </summary>
    public class EnumerableParameterAttribute : ParameterBindingAttribute
    {
        /// <summary>
        /// Gets the parameter binding.
        /// </summary>
        /// <returns>
        /// The parameter binding.
        /// </returns>
        /// <param name="parameter">
        /// The parameter description.
        /// </param>
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            if (parameter == null)
            {
                throw new Exception("This attribute was not specified on a parameter.");
            }

            if (!typeof(IEnumerable<object>).IsAssignableFrom(parameter.ParameterType))
            {
                throw new Exception(
                    "The EnumerableParameter attribute must be specified on parameter that implements IEnumerable<T>.");
            }

            var validator = parameter.Configuration.Services.GetBodyModelValidator();
            return
                parameter.BindWithFormatter(
                    new List<MediaTypeFormatter> { new EnumerableFormatter(parameter.ParameterType) }, 
                    validator);
        }
    }
}