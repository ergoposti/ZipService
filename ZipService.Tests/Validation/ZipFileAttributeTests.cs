#nullable disable
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using ZipService.Validation;

namespace ZipService.Tests.Validation
{
    [TestClass]
    public class ZipFileAttributeTests
    {
        [TestMethod]
        public void ZipFileAttribute_ReturnsSuccessForValidZipFile()
        {
            // Arrange
            var file = Substitute.For<IFormFile>();
            file.FileName.Returns("file.zip");
            file.Length.Returns(1);
            var attribute = new StructuredZipFileAttribute();
            var context = new ValidationContext(file) { DisplayName = "file.zip" };

            // Act
            var result = attribute.GetValidationResult(file, context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void ZipFileAttribute_ReturnsErrorForInvalidFileExtension()
        {
            // Arrange
            var file = Substitute.For<IFormFile>();
            file.FileName.Returns("file.txt");
            file.Length.Returns(1);
            var attribute = new StructuredZipFileAttribute();
            var context = new ValidationContext(file) { DisplayName = "file.txt" };

            // Act
            var result = attribute.GetValidationResult(file, context);

            // Assert
            Assert.AreEqual("Invalid file extension.", result.ErrorMessage);
        }

        [TestMethod]
        public void ZipFileAttribute_ReturnsErrorForMissingFile()
        {
            // Arrange
            var attribute = new StructuredZipFileAttribute();
            var context = new ValidationContext(new object()) { DisplayName = "file.zip" };

            // Act
            var result = attribute.GetValidationResult(null, context);

            // Assert
            Assert.AreEqual("File not provided.", result.ErrorMessage);
        }
    }
}
