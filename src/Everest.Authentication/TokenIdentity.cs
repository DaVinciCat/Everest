using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace Everest.Authentication
{
    public class TokenIdentity : ClaimsIdentity
    {
        public SecurityToken SecurityToken { get; }

        public TokenIdentity(SecurityToken securityToken, IIdentity identity)
            : base(identity)
        {
            SecurityToken = securityToken;
        }
    }
}