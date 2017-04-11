// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTextMediaFormatter.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.Interfaces.Http
{
    using System;
    using System.IO;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    ///     The json text media formatter.
    /// </summary>
    public class JsonTextMediaFormatter : JsonMediaTypeFormatter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonTextMediaFormatter" /> class.
        /// </summary>
        public JsonTextMediaFormatter()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        /// <summary>
        /// Called during serialization to write an object of the specified type to the specified stream.
        /// </summary>
        /// <param name="type">
        /// The type of the object to write.
        /// </param>
        /// <param name="value">
        /// The object to write.
        /// </param>
        /// <param name="writeStream">
        /// The stream to write to.
        /// </param>
        /// <param name="effectiveEncoding">
        /// The encoding to use when writing.
        /// </param>
        public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
        {
            var writer = new StreamWriter(writeStream);
            var serializer = this.CreateJsonSerializer();
            serializer.Formatting = Formatting.Indented;
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, value);
            var result = stringWriter.ToString();
            var text = result.Replace(" ", "&nbsp;").Replace("\r\n", "<br />\r\n");
            writer.Write(text);
            writer.Flush();
        }
    }
}