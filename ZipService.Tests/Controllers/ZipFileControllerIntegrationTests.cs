using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NSubstitute;
using System.Net;

namespace ZipService.Tests.Controllers
{
    [TestClass]
    public class ZipFileControllerIntegrationTests
    {
        private readonly HttpClient _httpClient;

        public ZipFileControllerIntegrationTests()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Development");
                    builder.ConfigureServices(AppConfiguration.ConfigureServices);
                });

            using var scope = factory.Services.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<WebApplication>();
            AppConfiguration.Configure(app, Substitute.For<IWebHostEnvironment>());

            app.Run();

            _httpClient = factory.CreateClient();
        }

        [TestMethod]
        public async Task UploadZipFile_EndpointShouldUploadZipFile()
        {
            // Arrange
            string testZipFilePath = "/Assets/CatGame.zip";
            using var fileStream = File.OpenRead(testZipFilePath);
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", Path.GetFileName(testZipFilePath));

            // Act
            var response = await _httpClient.PostAsync("/api/FileUpload/UploadZipFile", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(await response.Content.ReadAsStringAsync() == "Zip file uploaded successfully.");
        }
    }
}
