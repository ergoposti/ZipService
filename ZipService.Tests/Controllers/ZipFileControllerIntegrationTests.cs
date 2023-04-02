using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using ZipService.Models;

namespace ZipService.Tests.Controllers
{
    [TestClass]
    public class ZipFileControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public ZipFileControllerIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient();
        }

        [TestMethod]
        public async Task UploadEndpoint_ShouldUploadZipFile()
        {
            // Arrange
            string testZipFilePath = "./Assets/CatGame.zip";
            using var fileStream = File.OpenRead(testZipFilePath);
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", Path.GetFileName(testZipFilePath));

            // Act
            var uploadResponse = await _httpClient.PostAsync("/ZipFile/Upload", content);

            // Assert
            uploadResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var listResponse = await _httpClient.GetAsync("/ZipFile/List");
            var listResponseString = await listResponse.Content.ReadAsStringAsync();
            var zipFileListDto = JsonConvert.DeserializeObject<ZipFileListDto>(listResponseString);
            zipFileListDto.ZipFiles.ShouldNotBeEmpty();
        }

        [TestMethod]
        public async Task DeleteEndpoint_ShouldUploadZipFile()
        {
            // Arrange
            string testZipFilePath = "./Assets/CatGame.zip";
            using var fileStream = File.OpenRead(testZipFilePath);
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", Path.GetFileName(testZipFilePath));

            // Act
            var uploadResponse = await _httpClient.PostAsync("/ZipFile/Upload", content);
            var id = new Guid(await uploadResponse.Content.ReadAsStringAsync());
            var deleteResponse = await _httpClient.DeleteAsync($"/ZipFile/Delete/{id}");

            // Assert
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var listResponseAfterDelete = await _httpClient.GetAsync("/ZipFile/List");
            var listResponseAfterDeleteString = await listResponseAfterDelete.Content.ReadAsStringAsync();
            var zipFileListDtoAfterDelete = JsonConvert.DeserializeObject<ZipFileListDto>(listResponseAfterDeleteString);
            zipFileListDtoAfterDelete.ZipFiles.ShouldBeEmpty();
        }

        [TestMethod]
        public async Task UploadEndpoint_ShouldNotUpload_WhenFileIsInvalid()
        {
            // Arrange
            string testZipFilePath = "./Assets/InvalidGame.zip";
            using var fileStream = File.OpenRead(testZipFilePath);
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", Path.GetFileName(testZipFilePath));

            // Act
            var uploadResponse = await _httpClient.PostAsync("/ZipFile/Upload", content);

            // Assert
            uploadResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var responseMessage = await uploadResponse.Content.ReadAsStringAsync();
            responseMessage.ShouldContain("root directory is empty");
        }
    }
}
