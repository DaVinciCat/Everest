namespace Everest.Media;

public interface IMimeProvider
{
	bool TryGetMime(string fileExtension, out Mime mime);
}