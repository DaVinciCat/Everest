﻿using System.Security.Principal;
using Everest.Http;

namespace Everest.Security
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
        public static BasicIdentity GetBasicIdentity(this HttpContext context)
        {
            return context.User.Identity as BasicIdentity;
        }
    }
}
