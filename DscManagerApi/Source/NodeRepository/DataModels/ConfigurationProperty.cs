// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationProperty.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.NodeRepository.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    using Ticketmaster.Dsc.Interfaces.Mapping;
    using Ticketmaster.Dsc.Interfaces.NodeRepository.ViewModels;

    /// <summary>
    /// The configuration properties.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class ConfigurationProperty<T> : ModelBase<ConfigurationPropertyView>
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [Index("IX_ConfigurationPropertyScope", 4, IsUnique = true)]
        [StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        [Index("IX_ConfigurationPropertyScope", 2, IsUnique = true)]
        public PropertyScope Scope { get; set; }

        /// <summary>
        ///     Gets or sets the serialized value.
        /// </summary>
        public string SerializedValue
        {
            get
            {
                return JsonConvert.SerializeObject(
                    this.Value, 
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }

            set
            {
                this.Value = JsonConvert.DeserializeObject<T>(
                    value, 
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        [Index("IX_ConfigurationPropertyScope", 3, IsUnique = true)]
        [StringLength(256)]
        public string Target { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [Index("IX_ConfigurationPropertyScope", 1, IsUnique = true)]
        public PropertyType Type { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        [NotMapped]
        public T Value { get; set; }
    }

    /// <summary>
    ///     The configuration property.
    /// </summary>
    public class ConfigurationProperty : ConfigurationProperty<object>
    {
        /// <summary>
        ///     The cast value.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T CastValue<T>()
        {
            return (T)this.Value;
        }
    }
}