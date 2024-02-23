namespace tzWepApi.Interfaces;

public interface IFileService
{
    Task<string> SaveFile(string containerName, IFormFile file, string email);
    Task<Stream> DownloadBlobToFileAsync(string containerName, string blobName);
    Task DeleteFile(string containerName, string blobName);
}