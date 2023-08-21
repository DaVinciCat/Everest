namespace Everest.Mime
{
    public interface IMimeTypesProvider
    {
        bool TryGetMimeType(string fileExtension, out MimeType mimeType);
    }
}