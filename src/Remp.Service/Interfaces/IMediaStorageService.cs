namespace Remp.Service.Interfaces
{
    public interface IMediaStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string mediaType);
    }
}