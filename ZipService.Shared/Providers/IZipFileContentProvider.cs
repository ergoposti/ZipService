namespace ZipService.Shared.Providers
{
    public interface IZipFileContentProvider
    {
        FileNode GetZipFileTree(Stream zipFileStream, string archiveName);
    }
}