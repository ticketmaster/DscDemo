// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    using Ticketmaster.Dsc.EntityFrameworkExt.Attributes;

    /// <summary>
    ///     The encryption service.
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        #region Fields

        /// <summary>
        ///     The options.
        /// </summary>
        private readonly IEncryptionServiceOptions options;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionService"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public EncryptionService(IEncryptionServiceOptions options)
        {
            this.options = options;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The decrypt.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Decrypt(string value)
        {
            return this.Decrypt(
                value, 
                this.options.StoreName, 
                this.options.StoreLocation, 
                this.options.CertificateThumbprint);
        }

        /// <summary>
        /// The decrypt.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Decrypt(string value, string storeName, StoreLocation storeLocation, string certificateThumbprint)
        {
            return this.DecryptImpl(value, false, storeName, storeLocation, certificateThumbprint);
        }

        /// <summary>
        /// The decrypt base 64 string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecryptBase64String(string value)
        {
            return this.DecryptBase64String(
                value, 
                this.options.StoreName, 
                this.options.StoreLocation, 
                this.options.CertificateThumbprint);
        }

        /// <summary>
        /// The decrypt base 64 string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecryptBase64String(
            string value, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint)
        {
            return this.DecryptImpl(value, true, storeName, storeLocation, certificateThumbprint);
        }

        /// <summary>
        /// The decrypt entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void DecryptEntity(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                throw new Exception("The model must implement the IEncryptable interface to support encryption.");
            }

            // Get all the properties that are encryptable and decrypt them
            var encryptedProperties =
                entity.GetType()
                    .GetProperties()
                    .Where(
                        p =>
                        p.GetCustomAttributes(typeof(EncryptAttribute), true).Any(a => p.PropertyType == typeof(string)));

            foreach (var property in encryptedProperties)
            {
                var encryptedValue = property.GetValue(entity) as string;
                if (string.IsNullOrEmpty(encryptedValue))
                {
                    continue;
                }

                var isBase64 = this.GetAttributeProperty<bool>(property, "IsBase64String");
                var value = isBase64
                                ? this.DecryptBase64String(
                                    encryptedValue, 
                                    entity.StoreName, 
                                    entity.StoreLocation, 
                                    entity.CertificateThumbprint)
                                : this.Decrypt(
                                    encryptedValue, 
                                    entity.StoreName, 
                                    entity.StoreLocation, 
                                    entity.CertificateThumbprint);
                entry.Property(property.Name).OriginalValue = value;
                entry.Property(property.Name).IsModified = false;
            }
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Encrypt(string value)
        {
            return this.Encrypt(
                value, 
                this.options.StoreName, 
                this.options.StoreLocation, 
                this.options.CertificateThumbprint);
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Encrypt(string value, string storeName, StoreLocation storeLocation, string certificateThumbprint)
        {
            return this.EncryptImpl(value, false, storeName, storeLocation, certificateThumbprint);
        }

        /// <summary>
        /// The encrypt key.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string EncryptBase64String(string value)
        {
            return this.EncryptBase64String(
                value, 
                this.options.StoreName, 
                this.options.StoreLocation, 
                this.options.CertificateThumbprint);
        }

        /// <summary>
        /// The encrypt base 64 string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string EncryptBase64String(
            string value, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint)
        {
            return this.EncryptImpl(value, true, storeName, storeLocation, certificateThumbprint);
        }

        /// <summary>
        /// The encrypt entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void EncryptEntity(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                throw new Exception("The model must implement the IEncryptable interface to support encryption.");
            }

            // Get all the properties that are encryptable and encrypt them
            var encryptedProperties =
                entity.GetType()
                    .GetProperties()
                    .Where(
                        p =>
                        p.GetCustomAttributes(typeof(EncryptAttribute), true).Any(a => p.PropertyType == typeof(string)));

            var setThumbprint = false;
            foreach (var property in encryptedProperties)
            {
                var value = property.GetValue(entity) as string;
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var isBase64 = this.GetAttributeProperty<bool>(property, "IsBase64String");
                var encryptedValue = isBase64 ? this.EncryptBase64String(value) : this.Encrypt(value);
                property.SetValue(entity, encryptedValue);
                setThumbprint = true;
            }

            if (!setThumbprint)
            {
                return;
            }

            entity.CertificateThumbprint = this.options.CertificateThumbprint;
            entity.StoreLocation = this.options.StoreLocation;
            entity.StoreName = this.options.StoreName;
        }

        /// <summary>
        /// The process entity.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void ProcessEntity(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                return;
            }

            this.EncryptEntity(entry);
        }

        /// <summary>
        /// The process entity async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task ProcessEntityAsync(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                return Task.FromResult<object>(null);
            }

            this.EncryptEntity(entry);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// The process entity post save.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void ProcessEntityPostSave(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                return;
            }

            this.DecryptEntity(entry);
        }

        /// <summary>
        /// The process entity post save async.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task ProcessEntityPostSaveAsync(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                return Task.FromResult<object>(null);
            }

            this.DecryptEntity(entry);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// The process model upon creation.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        public void ProcessModelUponCreation(DbEntityEntry entry)
        {
            var entity = entry.Entity as IEncryptable;

            if (entity == null)
            {
                return;
            }

            this.DecryptEntity(entry);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The decrypt implementation.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isBase64String">
        /// The is base 64 string.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate Thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DecryptImpl(
            string value, 
            bool isBase64String, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint)
        {
            if (value == null)
            {
                return null;
            }

            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var collection = store.Certificates;
            var cert = collection.Find(X509FindType.FindByThumbprint, certificateThumbprint, false)[0];
            if (cert == null)
            {
                throw new Exception("The certificate with thumbprint: " + certificateThumbprint + " cannot be found.");
            }

            var privateKey = (RSACryptoServiceProvider)cert.PrivateKey;
            var data = privateKey.Decrypt(Convert.FromBase64String(value), true);
            return isBase64String ? Convert.ToBase64String(data) : Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// The encrypt implementation.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isBase64String">
        /// The is base 64 string.
        /// </param>
        /// <param name="storeName">
        /// The store Name.
        /// </param>
        /// <param name="storeLocation">
        /// The store Location.
        /// </param>
        /// <param name="certificateThumbprint">
        /// The certificate Thumbprint.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EncryptImpl(
            string value, 
            bool isBase64String, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint)
        {
            if (value == null)
            {
                return null;
            }

            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var collection = store.Certificates;
            var cert = collection.Find(X509FindType.FindByThumbprint, certificateThumbprint, false)[0];
            if (cert == null)
            {
                throw new Exception("The certificate with thumbprint: " + certificateThumbprint + " cannot be found.");
            }

            var publicKey = (RSACryptoServiceProvider)cert.PublicKey.Key;
            byte[] decodedData;
            try
            {
                decodedData = isBase64String ? Convert.FromBase64String(value) : Encoding.UTF8.GetBytes(value);
            }
            catch (Exception)
            {
                throw;
            }

            return Convert.ToBase64String(publicKey.Encrypt(decodedData, true));
        }

        /// <summary>
        /// The get attribute property.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">
        /// The type of property to return.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T GetAttributeProperty<T>(PropertyInfo attribute, string propertyName)
        {
            var attr = attribute.GetCustomAttributesData();
            if (attr == null)
            {
                return default(T);
            }

            var selectedAttr = attr.FirstOrDefault(a => a.AttributeType == typeof(EncryptAttribute));
            if (selectedAttr == null)
            {
                return default(T);
            }

            var args = selectedAttr.NamedArguments;

            if (args == null)
            {
                return default(T);
            }

            return (T)args.FirstOrDefault(a => a.MemberName == propertyName).TypedValue.Value;
        }

        #endregion
    }
}