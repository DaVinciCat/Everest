using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Everest.Utils
{
    public static class ClaimsExtensions
    {
        public static IEnumerable<string> GetRoles(this IEnumerable<Claim> claims) 
	        => claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

        public static string NameIdentifier(this IEnumerable<Claim> claims)
	        => claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public static string Email(this IEnumerable<Claim> claims)
	        => claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        public static string Name(this IEnumerable<Claim> claims)
	        => claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        public static bool IsAuthenticated(this ClaimsPrincipal principal) => principal.Identity != null && principal.Identity.IsAuthenticated;

    }
}
