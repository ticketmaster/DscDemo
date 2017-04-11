// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableFormatter.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading;

    using Newtonsoft.Json;

    /// <summary>
    ///     The enumerable formatter.
    /// </summary>
    public class EnumerableFormatter : BufferedMediaTypeFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableFormatter"/> class.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        public EnumerableFormatter(Type property)
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
            this.Property = property;
        }

        /// <summary>
        ///     Gets or sets the property.
        /// </summary>
        protected Type Property { get; set; }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserializean object of the
        ///     specified type.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can deserialize the type; otherwise,
        ///     false.
        /// </returns>
        /// <param name="type">
        /// The type to deserialize.
        /// </param>
        public override bool CanReadType(Type type)
        {
            return typeof(IEnumerable<object>).IsAssignableFrom(type);
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can serializean object of the
        ///     specified type.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> can serialize the type; otherwise,
        ///     false.
        /// </returns>
        /// <param name="type">
        /// The type to serialize.
        /// </param>
        public override bool CanWriteType(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads synchronously from the buffered stream.
        /// </summary>
        /// <returns>
        /// An object of the given <paramref name="type"/>.
        /// </returns>
        /// <param name="type">
        /// The type of the object to deserialize.
        /// </param>
        /// <param name="readStream">
        /// The stream from which to read.
        /// </param>
        /// <param name="content">
        /// The <see cref="T:System.Net.Http.HttpContent"/>, if available. Can be null.
        /// </param>
        /// <param name="formatterLogger">
        /// The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to cancel the operation.
        /// </param>
        public override object ReadFromStream(
            Type type, 
            Stream readStream, 
            HttpContent content, 
            IFormatterLogger formatterLogger, 
            CancellationToken cancellationToken)
        {
            var reader = new StreamReader(readStream);
            var json = reader.ReadToEnd();
            if (!json.StartsWith("["))
            {
                json = "[" + json + "]";
            }

            try
            {
                return JsonConvert.DeserializeObject(json, this.Property);
            }
            catch
            {
                return null;
            }
        }
    }
}