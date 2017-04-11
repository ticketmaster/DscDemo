using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketmaster.Dsc.DscManager.DataAccess
{
    using System.Data.Entity;
    using System.Security.AccessControl;

    using Ticketmaster.CredentialRepository.DataAccess;
    using Ticketmaster.CredentialRepository.Models;

    public class CredentialRepositoryInitializer : CreateDatabaseIfNotExists<CredentialRepositoryContext>
    {
        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context"> The context to seed. </param>
        protected override void Seed(CredentialRepositoryContext context)
        {
            var permissions = new List<Permission>
                                  {
                                      new Permission
                                          {
                                              Access = AccessControlType.Allow,
                                              Action = PermissionActions.All,
                                              EntityGuid = new Guid(),
                                              Identity = "Anonymous",
                                              IdentityProvider = "Anonymous",
                                              Model = null
                                          }
                                  };
            context.Permissions.AddRange(permissions);
            context.SaveChanges();
        }
    }
}
