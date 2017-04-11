// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DscEventArgs.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.DscManager.Services
{
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// The dsc event args.
    /// </summary>
    public class DscEventArgs : DynamicObject
    {
        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DscEventArgs"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public DscEventArgs(string name, string message)
        {
            this.Name = name;
            this.Message = message;
        }

        /// <summary>
        /// The count.
        /// </summary>
        public int Count => this.dictionary.Count;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The get member.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetMember(string name)
        {
            return this.GetMember<object>(name);
        }

        /// <summary>
        /// The get member.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T GetMember<T>(string name)
        {
            if (!this.dictionary.ContainsKey(name.ToLower()))
            {
                return default(T);
            }

            return (T)this.dictionary[name.ToLower()];
        }

        /// <summary>
        /// The try get member.
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();
            return this.dictionary.TryGetValue(name, out result);
        }

        /// <summary>
        /// The try set member.
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }
}