using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Files;

public interface IStaticFileRequestHandler
{
	Task<bool> TryServeStaticFileAsync(HttpContext context);
}