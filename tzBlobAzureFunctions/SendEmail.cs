using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using tzBlobAzureFunctions.Interfaces;

namespace tzBlobAzureFunctions
{
    public class SendEmail
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IEmailSender _emailSender;

        public SendEmail(BlobServiceClient blobServiceClient, IEmailSender emailSender)
        {
            _blobServiceClient = blobServiceClient;
            _emailSender = emailSender;
        }

        [Function(nameof(SendEmail))]
        public async Task Run([BlobTrigger("tz-docx-container/{name}", Connection = "")] string name)
        {
            var blobClient = _blobServiceClient.GetBlobContainerClient("tz-docx-container").GetBlobClient(name);

            if (await blobClient.ExistsAsync())
            {
                var properties = await blobClient.GetPropertiesAsync();
                var metadata = properties.Value.Metadata;

                if (metadata.All(m => m.Key != "Email"))
                    throw new Exception("Incorrect File Metadata!");

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = "tz-docx-container",
                    BlobName = name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasToken = blobClient.GenerateSasUri(sasBuilder);
                string strSasToken = sasToken.ToString();

                await _emailSender.SendEmailAsync(metadata.FirstOrDefault(m => m.Key == "Email").Value, "Success", $"File was added. You can download it using this link: {strSasToken}");
            }
        }
    }
}