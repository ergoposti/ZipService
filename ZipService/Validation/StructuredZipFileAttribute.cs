using System.ComponentModel.DataAnnotations;
using ZipService.BLL.Services;
using ZipService.Shared.Providers;

namespace ZipService.Validation;

public class StructuredZipFileAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;

        var validationErrors = new List<string>();

        if (file == null || file.Length == 0)
        {
            validationErrors.Add("File not provided.");
        }

        if (Path.GetExtension(validationContext.DisplayName) != ".zip")
        {
            validationErrors.Add("Invalid file extension.");
        }

        if (validationErrors.Any())
        {
            return new ValidationResult(string.Join(Environment.NewLine, validationErrors));
        }

        var directoryStructureValidator = validationContext.GetRequiredService<IGameDirectoryStructureValidator>();
        var zipFileContentProvider = validationContext.GetRequiredService<IZipFileContentProvider>();

        using var memoryStream = new MemoryStream();
        // ! is introduced because it actually cannot be null here.
        file!.CopyTo(memoryStream);

        var structureValidationResults = directoryStructureValidator.Validate(zipFileContentProvider.GetZipFileTree(memoryStream, file.Name));

        if (structureValidationResults.Any())
        {
            return new ValidationResult(string.Join(Environment.NewLine, structureValidationResults));
        }

        return ValidationResult.Success;
    }
}
