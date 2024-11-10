using System.Security.Principal;
using Everest.Http;

namespace Everest.Authentication
{
    public class BasicIdentity : GenericIdentity
    {
        public string Password { get; }

        public BasicIdentity(string username, string password) :
            base(username, "Basic")
        {
            Password = password;
        }
    }

    public static class EndPointFeatureExtensions
    {
        public static BasicIdentity GetBasicIdentity(this IHttpContext context)
        {
            return context.User.Identity as BasicIdentity;
        }
    }
}
