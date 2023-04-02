using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Web.Http.Results;
using ZipService.DAL;
using ZipService.Domain;
using ZipService.Mappers;
using ZipService.Models;
using ZipService.Validation;

namespace ZipService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZipFileController : ControllerBase
    {
        private readonly ILogger<ZipFileController> _logger;
        private readonly IUnitOfWork<FileEntity> _fileEntityUnitOfWork;
        private readonly IBlobService _blobService;
        private readonly IZipFileListDtoMapper zipFileListDtoMapper;

        public ZipFileController(ILogger<ZipFileController> logger, IUnitOfWork<FileEntity> fileEntityUnitOfWork, IBlobService blobService, IZipFileListDtoMapper zipFileListDtoMapper)
        {
            _logger = logger;
            _fileEntityUnitOfWork = fileEntityUnitOfWork;
            _blobService = blobService;
            this.zipFileListDtoMapper = zipFileListDtoMapper;
        }

        [HttpPost(nameof(Upload))]
        [TypeFilter(typeof(StructuredZipFileActionFilterAttribute))]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var filename = Path.GetFileName(file.FileName);

            // TODO Consider using one stream, some minor overhead here with duplicate streams since we create other one in validation
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            // TODO following stuff could be moved out of the controller. Too much stuff that doesnt belong to controller
            var saveFileResult = await _blobService.SaveFile(memoryStream);

            return saveFileResult.Match(savedFileId =>
            {
                var fileEntityStoreResult = _fileEntityUnitOfWork.Add(new FileEntity(savedFileId, file.FileName));

                return fileEntityStoreResult.Match<IActionResult>(fileId => {
                    return Ok(fileId.ToString());
                }, exception =>
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Couldn't store file metadata to database" });
                });
            }, exception =>
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Couldn't save the file" });
            });
        }

        [HttpGet($"{nameof(Download)}/{{fileId}}", Name = nameof(Download))]
        // TODO this shouldn't be part of this service, should be separate. 
        public async Task<IActionResult> Download(Guid fileId)
        {
            var fileResult = await _blobService.LoadFile(fileId);

            return fileResult.Match<IActionResult>(fileStream =>
            {
                return File(fileStream, "application/zip");
            }, exception =>
            {
                // TODO could also be invalid fileId
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Couldn't load the file" });
            });
        }

        [HttpGet(nameof(List))]
        [ProducesResponseType(typeof(ZipFileListDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> List()
        {
            // TODO following stuff could be moved out of the controller. Too much stuff that doesnt belong to controller
            var fileEntitiesResult = _fileEntityUnitOfWork.List();

            var filesAndStreamsResult = await fileEntitiesResult.MapAsync<Result<(FileEntity, Stream)[]>>(async (fileEntities) =>
            {
                // TODO Currently we make all requests to blob storage separately, consider optimizing
                var streams = (await Task.WhenAll(fileEntities.Select(async x => (fileEntity: x, streamResult: await _blobService.LoadFile(x.FileId)))))
                    .ToList();

                var failedStreamDownloads = streams.Where(stream => stream.streamResult.IsFaulted);

                if(failedStreamDownloads.Any())
                {
                    return new Result<(FileEntity, Stream)[]>(new Exception($"Couldn´t download streams for files: {string.Join(", ", failedStreamDownloads.Select(x => x.fileEntity.FileName))}"));
                }

                var clean = streams.Select(tuple => (tuple.fileEntity, tuple.streamResult.IfFail((Exception exception) => throw new Exception("Unhandled failed stream download", exception)))).ToArray();

                return new Result<(FileEntity, Stream)[]>(clean);
            });

            return filesAndStreamsResult.Match(filesWithStreamsResults =>
            {
                return filesWithStreamsResults.Match<IActionResult>(filesWithStreams =>
                {
                    return Ok(zipFileListDtoMapper.Map(filesWithStreams.ToImmutableList()));
                }, exception =>
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = exception.Message });
                });
            }, exception =>
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = "Couldn't fetch files metadata from database" });
            });
        }

        [HttpDelete($"{nameof(Delete)}/{{fileEntityId}}", Name = nameof(Delete))]
        public async Task<IActionResult> Delete(Guid fileEntityId)
        {
            var deleteResult = _fileEntityUnitOfWork.Delete(fileEntityId);

            // TODO delete actual file as well
            return deleteResult.Match<IActionResult>(_ => Ok("File succesfully deleted"), exception => StatusCode((int)HttpStatusCode.InternalServerError, new { Error = exception.Message }));
        }
    }
}