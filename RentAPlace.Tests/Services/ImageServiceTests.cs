using Xunit;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RentAPlace.Api.Services;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class ImageServiceTests
{
    [Fact]
    public async Task SavePropertyImageAsync_Should_SaveFile_AndReturnRelativePath()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.WebRootPath).Returns(tempDir);

        var service = new ImageService(envMock.Object);

        // Create fake file
        var fileMock = new Mock<IFormFile>();
        var content = "Fake image content";
        var fileName = "test.jpg";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
            .Returns<Stream, System.Threading.CancellationToken>((stream, token) => ms.CopyToAsync(stream));

        // Act
        var relativePath = await service.SavePropertyImageAsync(fileMock.Object);

        // Assert
        var savedFile = Path.Combine(tempDir, "uploads", "properties", Path.GetFileName(relativePath));
        Assert.True(File.Exists(savedFile));
        Assert.Equal($"/uploads/properties/{Path.GetFileName(relativePath)}", relativePath);

        // Cleanup
        Directory.Delete(tempDir, true);
    }
}
