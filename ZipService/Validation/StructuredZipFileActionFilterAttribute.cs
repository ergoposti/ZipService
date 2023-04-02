using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ZipService.BLL.Services;
using ZipService.Shared.Providers;

namespace ZipService.Validation
{
    public class StructuredZipFileActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IGameDirectoryStructureValidator _directoryStructureValidator;
        private readonly IZipFileContentProvider _zipFileContentProvider;

        public StructuredZipFileActionFilterAttribute(IGameDirectoryStructureValidator directoryStructureValidator,
            IZipFileContentProvider zipFileContentProvider)
        {
            _directoryStructureValidator = directoryStructureValidator;
            _zipFileContentProvider = zipFileContentProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.HttpContext.Request.Form.Files.FirstOrDefault();

            var validationErrors = new List<string>();

            if (file == null || file.Length == 0)
            {
                validationErrors.Add("File not provided.");
                return;
            }

            if (Path.GetExtension(file.FileName) != ".zip")
            {
                validationErrors.Add("Invalid file extension.");
            }

            if (validationErrors.Any())
            {
                context.Result = new BadRequestObjectResult(string.Join(Environment.NewLine, validationErrors));
                return;
            }

            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            // TODO move to BLL as with Unit of work stuff from controller, more accurate to have it there. web api shouldnt directly deal with such stuff
            var structureValidationResults = _directoryStructureValidator.Validate(_zipFileContentProvider.GetZipFileTree(memoryStream, file.FileName));

            if (structureValidationResults.Any())
            {
                context.Result = new BadRequestObjectResult(string.Join(Environment.NewLine, structureValidationResults));
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
