using Moq;
using Azure.Storage.Blobs;
using tzBlobAzureFunctions.Interfaces;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Azure;

namespace tzBlobAzureFunctions.Tests
{
    public class SendEmailTests
    {
        private Mock<BlobServiceClient> _azBlobServiceClientMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<BlobContainerClient> _azBlobContainerClientMock;
        private Mock<BlobClient> _azBlobClientMock;

        public SendEmailTests()
        {
            _azBlobServiceClientMock = new Mock<BlobServiceClient>();
            _emailSenderMock = new Mock<IEmailSender>();
            _azBlobContainerClientMock = new Mock<BlobContainerClient>();
            _azBlobClientMock = new Mock<BlobClient>();
        }

        private void SetupMockObjects()
        {
            _azBlobClientMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), default, default, default, default, default, default, default))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContentInfo(default, default, default, default, default), default!));

            _azBlobClientMock.Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

            var properties = new Dictionary<string, string> { { "Email", "test@test.test" } };
            _azBlobClientMock.Setup(x => x.GetPropertiesAsync(default, default))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobProperties(metadata: properties), default!));

            _azBlobClientMock.Setup(x => x.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(new Uri("http://example.com"));

            _azBlobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_azBlobClientMock.Object);
            _azBlobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(_azBlobContainerClientMock.Object);
        }

        [Fact]
        public async Task Run_SendEmail_WithCorrectMetadata()
        {
            // Arrange
            SetupMockObjects();
            var sendEmail = new SendEmail(_azBlobServiceClientMock.Object, _emailSenderMock.Object);

            // Act
            await sendEmail.Run("example.docx");

            // Assert
            _emailSenderMock.Verify(x => x.SendEmailAsync("test@test.test", "Success", It.IsAny<string>()), Times.Once());
        }
    }
}
