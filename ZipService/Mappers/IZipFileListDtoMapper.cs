using System.Collections.Immutable;
using ZipService.Domain;
using ZipService.Models;

namespace ZipService.Mappers
{
    public interface IZipFileListDtoMapper
    {
        ZipFileListDto Map(ImmutableList<(FileEntity FileEntity, Stream Stream)> fileEntitiesWithStreams);
    }
}