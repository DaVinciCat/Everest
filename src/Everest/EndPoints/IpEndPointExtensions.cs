﻿using System;
using System.Net;

namespace Everest.EndPoints
{
    public static class IpEndPointExtensions
    {
        public static string Description(this IPEndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));

            return $"{endPoint.Address}:{endPoint.Port}";
        }
    }
}
