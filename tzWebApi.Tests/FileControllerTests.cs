using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using tzWepApi.Controllers;
using tzWepApi.Interfaces;
using tzWepApi.Models;

namespace tzWebApi.Tests
{
    public class FileControllerTests
    {
        [Fact]
        public async Task UploadFile_ReturnsOkResponse()
        {
            // Arrange
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>())).Returns(Task.FromResult(string.Empty));
            var controller = new FileController(fileServiceMock.Object);
            var cancellationToken = new CancellationToken();
            var data = new TzModel()
            {
                Email = "test@example.com",
                File = CreateTestFile()
            };
        
            // Act
            var result = await controller.SaveFile(data, cancellationToken) as OkObjectResult;
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.Value);
        }
        
        [Fact]
        public async Task DownloadFile_ReturnsFile()
        {
            // Arrange
            var fileServiceMock = new Mock<IFileService>();
            var blobName = "testBlobName";
            var memoryStream = new MemoryStream();
            fileServiceMock.Setup(x => x.DownloadBlobToFileAsync(It.IsAny<string>(), blobName)).ReturnsAsync(memoryStream);
            var controller = new FileController(fileServiceMock.Object);

            // Act
            var result = await controller.DownloadFile(blobName) as FileStreamResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("application/octet-stream", result.ContentType);
            Assert.Equal(blobName, result.FileDownloadName);
            Assert.Equal(memoryStream, result.FileStream);
        }

        [Fact]
        public async Task DeleteFile_ReturnsSuccess()
        {
            // Arrange
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(x => x.DeleteFile(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            var controller = new FileController(fileServiceMock.Object);

            // Act
            var result = await controller.DeleteFile("testBlobName") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.Value);
        }
    
        private static IFormFile CreateTestFile()
        {
            var content = "test content";
            var fileName = "test.docx";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            return new FormFile(ms, 0, ms.Length, "file", fileName);
        }
    }
}