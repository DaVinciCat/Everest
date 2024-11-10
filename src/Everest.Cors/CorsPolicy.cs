using Everest.Http;

namespace Everest.Cors
{
    public class CorsPolicy
	{
		public static CorsPolicy Default { get; } = new CorsPolicy();

		public string Origin { get; set; } = "*";

		public string[] AllowMethods { get; set; } = { HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete };

		public string[] AllowHeaders { get; set; } = { HttpHeaders.ContentType, HttpHeaders.Accept, HttpHeaders.XRequestedWith };

		public int MaxAge { get; set; } = 1728000;

		public CorsPolicy(string origin, string[] allowMethods, string[] allowHeaders, int maxAge)
		{
			Origin = origin;
			AllowMethods = allowMethods;
			AllowHeaders = allowHeaders;
			MaxAge = maxAge;
		}

		private CorsPolicy()
		{

		}
	}
}
