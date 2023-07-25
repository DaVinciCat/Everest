using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		CorsPolicy[] Policies { get; }

		void AddCorsPolicy(CorsPolicy policy);

		void RemoveCorsPolicy(CorsPolicy policy);

		void RemoveCorsPolicy(string origin);

		void ClearCorsPolicies();
		
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
