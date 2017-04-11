// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialsController.cs" company="">
//   
// </copyright>
// <summary>
//   The credentials controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ticketmaster.CredentialRepository.Controllers
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Query;

    using Newtonsoft.Json;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Http;
    using Ticketmaster.CredentialRepository.Models;
    using Ticketmaster.Dsc.EntityFrameworkExt.Services;

    /// <summary>
    ///     The credentials controller.
    /// </summary>
    [Authorize]
    [Route]
    public class CredentialsController : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialsController"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CredentialsController(IAuthorizedRepository<Credential> repository)
        {
            this.Repository = repository;
        }

        /// <summary>
        ///     Gets or sets the repository.
        /// </summary>
        protected IAuthorizedRepository<Credential> Repository { get; set; }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{id:int}")]
        public virtual async Task<IHttpActionResult> Delete(int id)
        {
            var entity = this.Repository.Get(id);
            if (entity == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.Repository.Delete(entity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }

            return this.Ok(entity);
        }

        /// <summary>
        /// The delete permission.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{id:int}/permissions/{permissionId:int}")]
        [Route("permissions/{permissionId:int}")]
        public virtual async Task<IHttpActionResult> DeletePermission(int? id, int permissionId)
        {
            var permission = this.Repository.GetPermission(permissionId);

            if (permission == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.Repository.DeletePermission(permission);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }

            return this.Ok(permission);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{id:int}")]
        public virtual IHttpActionResult Get(int id)
        {
            var entity = this.Repository.Get(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            return this.Ok(entity);
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="resultSize">
        /// The result size.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route]
        [ActionName("Get")]
        public virtual IHttpActionResult GetAll(ODataQueryOptions<Credential> options, int resultSize = 100)
        {
            var settings = new ODataQuerySettings { EnableConstantParameterization = true };
            if (resultSize > 0)
            {
                settings.PageSize = resultSize;
            }

            var results = (IQueryable<Credential>)options.ApplyTo(this.Repository.GetAll(), settings);
            return this.Ok(new PagedResult<Credential>(results, options, resultSize));
        }

        /// <summary>
        ///     The get all permissions.
        /// </summary>
        /// <returns>
        ///     The <see cref="IHttpActionResult" />.
        /// </returns>
        [EnableQuery]
        [Route("permissions")]
        public virtual IHttpActionResult GetAllPermissions()
        {
            var result = this.Repository.GetAllPermissions();
            if (result == null)
            {
                return this.Ok();
            }

            return this.Ok(result);
        }

        /// <summary>
        /// The get permissions.
        /// </summary>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{id:int}/permissions/{permissionId:int}")]
        [Route("permissions/{permissionId:int}")]
        [ActionName("GetPermissions")]
        public virtual IHttpActionResult GetPermissions(int permissionId, int? id = null)
        {
            var result = this.Repository.GetPermission(permissionId);
            if (result == null)
            {
                return this.Ok();
            }

            return this.Ok(result);
        }

        /// <summary>
        /// The get permissions.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [EnableQuery]
        [Route("{id:int}/permissions")]
        public virtual IHttpActionResult GetPermissionsForRecord(int id)
        {
            var entity = this.Repository.Get(id);
            if (entity == null)
            {
                return this.NotFound();
            }

            var result = this.Repository.GetAllPermissionsForRecord(entity);
            if (result == null)
            {
                return this.Ok();
            }

            return this.Ok(result);
        }

        /// <summary>
        /// The post.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route]
        public async Task<IHttpActionResult> Post(Credential entity)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                await this.Repository.Insert(entity);
                return this.Created(this.Request.RequestUri + "/" + entity.Id, entity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }
        }

        /// <summary>
        /// The post permission.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="permissionEntity">
        /// The permission entity.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("{id:int}/permissions")]
        public virtual async Task<IHttpActionResult> PostPermission(int id, Permission permissionEntity)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var entity = this.Repository.Get(id);
            if (entity == null)
            {
                return this.NotFound();
            }

            try
            {
                permissionEntity.EntityGuid = entity.EntityGuid;
                permissionEntity.Model = entity.GetType().Name;
                await this.Repository.InsertPermission(permissionEntity);
                return this.Created(this.Request.RequestUri + "/" + entity.Id, entity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }
        }

        /// <summary>
        /// The post permission.
        /// </summary>
        /// <param name="permissionEntity">
        /// The permission entity.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [Route("permissions")]
        public virtual async Task<IHttpActionResult> PostPermission(Permission permissionEntity)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                await this.Repository.InsertPermission(permissionEntity);
                return this.Created(this.Request.RequestUri + "/" + permissionEntity.Id, permissionEntity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }
        }

        /// <summary>
        /// The put.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(void))]
        [Route("{id:int}")]
        public virtual async Task<IHttpActionResult> Put(int id, Credential entity)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != entity.Id)
            {
                return this.BadRequest();
            }

            try
            {
                var currenCredential = this.Repository.Get(id);
                var serializedData = JsonConvert.SerializeObject(entity);
                var mergedEntity = JsonConvert.DeserializeObject<Credential>(
                    serializedData, 
                    new JsonEntityConverter<Credential>(currenCredential));
                await this.Repository.Update(mergedEntity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (this.Repository.Get(id) == null)
                {
                    return this.NotFound();
                }

                throw;
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// The put permission.
        /// </summary>
        /// <param name="permissionId">
        /// The permission id.
        /// </param>
        /// <param name="permissionEntity">
        /// The permission entity.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [ResponseType(typeof(void))]
        [Route("{id:int}/permissions/{permissionId:int}")]
        [Route("permissions/{permissionId:int}")]
        public virtual async Task<IHttpActionResult> PutPermission(
            int permissionId, 
            Permission permissionEntity, 
            int? id = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                await this.Repository.UpdatePermission(permissionEntity);
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Forbidden(e.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (this.Repository.Get(permissionId) == null)
                {
                    return this.NotFound();
                }

                throw;
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Repository.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// The forbidden.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        protected IHttpActionResult Forbidden(string reason)
        {
            return new ForbiddenActionResult(this.Request, reason);
        }

        /// <summary>
        ///     The forbidden.
        /// </summary>
        /// <returns>
        ///     The <see cref="IHttpActionResult" />.
        /// </returns>
        protected IHttpActionResult Forbidden()
        {
            return new ForbiddenActionResult(this.Request, "You do not have permission to perform this action.");
        }
    }
}