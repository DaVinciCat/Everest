using Everest.Http;
using System.IO;

namespace Everest.Files;

public interface IStaticFilesProvider
{
	bool TryGetFile(HttpRequest request, out FileInfo file);
}