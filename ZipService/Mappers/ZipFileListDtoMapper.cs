using System.Collections.Immutable;
using ZipService.Domain;
using ZipService.Models;
using ZipService.Shared;
using ZipService.Shared.Providers;

namespace ZipService.Mappers
{
    public class ZipFileListDtoMapper : IZipFileListDtoMapper
    {
        private readonly IZipFileContentProvider _zipFileContentProvider;

        public ZipFileListDtoMapper(IZipFileContentProvider zipFileContentProvider)
        {
            _zipFileContentProvider = zipFileContentProvider;
        }

        public ZipFileListDto Map(ImmutableList<(FileEntity FileEntity, Stream Stream)> fileEntitiesWithStreams)
        {
            return new ZipFileListDto(fileEntitiesWithStreams.Select(tuple => Map(tuple.FileEntity, tuple.Stream)).ToArray());
        }

        private ZipFileDto Map(FileEntity fileEntity, Stream stream)
        {
            var root = _zipFileContentProvider.GetZipFileTree(stream, fileEntity.FileName);

            return Map(fileEntity.Id, root);
        }

        private static ZipFileDto Map(Guid id, FileNode fileNode) => new ZipFileDto(id, fileNode.Name, fileNode.Children.Select(Map).ToArray());

        private static FileNodeDto Map(FileNode fileNode) => new FileNodeDto(fileNode.Name, fileNode.IsDirectory, fileNode.Children.Select(Map).ToArray());
    }
}
