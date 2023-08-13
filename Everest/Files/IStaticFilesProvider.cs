using Everest.Http;
using System.IO;

namespace Everest.Files;

public interface IStaticFilesProvider
{
	string PhysicalPath { get; set; }

	string[] Files { get; }

	bool HasFile(HttpRequest request);

	bool TryGetFile(HttpRequest request, out FileInfo file);
}