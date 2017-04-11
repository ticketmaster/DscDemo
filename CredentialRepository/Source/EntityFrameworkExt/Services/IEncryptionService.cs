// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEncryptionService.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.Dsc.EntityFrameworkExt.Services
{
    using System.Data.Entity.Infrastructure;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     The EncryptionService interface.
    /// </summary>
    public interface IEncryptionService : ISaveAction, IPostSaveAction, IModelCreationAction
    {
        #region Public Methods and Operators

        /// <summary>
        /// The decrypt base 64 string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Decrypt(string value);

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
        string Decrypt(string value, string storeName, StoreLocation storeLocation, string certificateThumbprint);

        /// <summary>
        /// The decrypt base 64 string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string DecryptBase64String(string value);

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
        string DecryptBase64String(
            string value, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint);

        /// <summary>
        /// The decrypt entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void DecryptEntity(DbEntityEntry entity);

        /// <summary>
        /// The encrypt key.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Encrypt(string value);

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
        string Encrypt(string value, string storeName, StoreLocation storeLocation, string certificateThumbprint);

        /// <summary>
        /// The encrypt key.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string EncryptBase64String(string value);

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
        string EncryptBase64String(
            string value, 
            string storeName, 
            StoreLocation storeLocation, 
            string certificateThumbprint);

        /// <summary>
        /// The encrypt entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void EncryptEntity(DbEntityEntry entity);

        #endregion
    }
}