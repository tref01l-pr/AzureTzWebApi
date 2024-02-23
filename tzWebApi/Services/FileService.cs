using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using tzWepApi.Interfaces;

namespace tzWepApi.Services;

public class FileService : IFileService
{
    private string _connectionString =
        @"DefaultEndpointsProtocol=https;AccountName=tzblobstorageaccount;AccountKey=N8O1wiWHpw0x0M7JF2MsOF5uf5XiNFAvW5k2aC+OCpC0Rp9J7ZBhI4jx4m0A6Xr4a9OQelA7wamQ+AStxJGejA==;EndpointSuffix=core.windows.net";
    
    public async Task<string> SaveFile(string containerName, IFormFile file, string email)
    {
        BlobContainerClient blobClientContainer = new BlobContainerClient(_connectionString, containerName);
        BlobClient blobClient = blobClientContainer.GetBlobClient(file.FileName);
        var memoryStream = new MemoryStream();

        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        await blobClient.UploadAsync(memoryStream);

        Dictionary<string, string> metadata = new Dictionary<string, string>()
        {
            { "Email", email }
        };

        await blobClient.SetMetadataAsync(metadata);

        var path = blobClient.Uri.AbsoluteUri;
        return path;
    }

    public async Task<Stream> DownloadBlobToFileAsync(string containerName, string blobName)
    {
        BlobClient blobClient = new BlobClient(_connectionString, containerName, blobName);
        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        memoryStream.Position = 0;
    
        return memoryStream;
    }

    public async Task DeleteFile(string containerName, string blobName)
    {
        BlobClient blobClient = new BlobClient(_connectionString, containerName, blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}