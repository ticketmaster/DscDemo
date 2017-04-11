using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketmaster.CredentialRepository.Extensions
{
    using System.Security.Claims;
    using System.Security.Principal;

    public static class IdentityProviderExtensions
    {
        public static string GetIdentityProvider(this IPrincipal principal)
        {
            return principal.Identity.GetIdentityProvider();
        }


        public static string GetIdentityProvider(this IIdentity identity)
        {
            var id = identity as ClaimsIdentity;
            if (id == null)
            {
                return null;
            }

            var claim = id.FindFirst("idp");

            if (claim == null) throw new InvalidOperationException("idp claim is missing");
            return claim.Value;
        }
    }
}
