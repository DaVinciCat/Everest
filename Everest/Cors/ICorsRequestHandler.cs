using System.Collections.Generic;
using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		CorsPolicy[] Policies { get; }

		public void AddCorsPolicy(CorsPolicy policy);
		
		public Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
