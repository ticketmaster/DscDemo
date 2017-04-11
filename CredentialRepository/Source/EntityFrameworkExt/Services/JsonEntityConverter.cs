// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonEntityConverter.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The entity converter.
    /// </summary>
    /// <typeparam name="T">
    /// The type to convert to.
    /// </typeparam>
    public class JsonEntityConverter<T> : JsonConverter
        where T : class
    {
        #region Fields

        /// <summary>
        /// The existing object.
        /// </summary>
        private readonly T existingObject;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEntityConverter{T}"/> class.
        /// </summary>
        /// <param name="existingObject">
        /// The existing object.
        /// </param>
        public JsonEntityConverter(T existingObject)
        {
            this.existingObject = existingObject;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">
        /// Type of the object.
        /// </param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.
        /// </param>
        /// <param name="objectType">
        /// Type of the object.
        /// </param>
        /// <param name="existingValue">
        /// The existing value of object being read.
        /// </param>
        /// <param name="serializer">
        /// The calling serializer.
        /// </param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(
            JsonReader reader, 
            Type objectType, 
            object existingValue, 
            JsonSerializer serializer)
        {
            var target = this.existingObject;
            serializer.Populate(reader, target);

            return target;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="serializer">
        /// The calling serializer.
        /// </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing has not been implemented in this converter.");
        }

        #endregion
    }
}