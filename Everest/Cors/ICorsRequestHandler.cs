using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		CorsPolicy[] Policies { get; }

		void AddCorsPolicy(CorsPolicy policy);
		
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
