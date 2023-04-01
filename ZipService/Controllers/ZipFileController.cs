#nullable enable
using Microsoft.AspNetCore.Mvc;
using ZipService.Validation;

namespace ZipService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZipFileController : ControllerBase
    {
        private readonly ILogger<ZipFileController> _logger;

        public ZipFileController(ILogger<ZipFileController> logger)
        {
            _logger = logger;
        }

        [HttpPost("upload")]
        [StructuredZipFile]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file to upload.");
            }

            var filename = Path.GetFileName(file.FileName);

            // TODO Consider using one stream, some minor overhead here with duplicate streams since we create other one in validation
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            return Ok($"File {filename} has been uploaded successfully.");
        }
    }
}