using System.Threading.Tasks;
using Everest.Http;

namespace Everest.Files;

public interface IStaticFileRequestHandler
{
	IStaticFilesProvider FilesProvider { get; }

	Task<bool> TryServeStaticFileAsync(HttpContext context);
}