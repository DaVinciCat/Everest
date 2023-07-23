namespace Everest.Cors
{
	public class CorsPolicy
	{
		public static CorsPolicy Default { get; } = new();

		public string Origin { get; set; } = "*";

		public string[] AllowMethods { get; set; } = { "GET", "POST", "PUT", "DELETE" };

		public string[] AllowHeaders { get; set; } = { "Content-Type", "Accept", "X-Requested-With" };

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
