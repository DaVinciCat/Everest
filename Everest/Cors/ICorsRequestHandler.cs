using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Cors
{
	public interface ICorsRequestHandler
	{
		Task<bool> TryHandleCorsRequestAsync(HttpContext context);
	}
}
