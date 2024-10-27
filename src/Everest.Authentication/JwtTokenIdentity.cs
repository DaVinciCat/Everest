using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;

namespace Everest.Authentication
{
    public class JwtTokenIdentity : TokenIdentity
    {
        public new JwtSecurityToken SecurityToken { get; }

        public JwtTokenIdentity(JwtSecurityToken securityToken, IIdentity identity)
            : base(securityToken, identity)
        {
            SecurityToken = securityToken;
        }
    }
}
