using ZipService.Shared;

namespace ZipService.BLL.Services
{
    public interface IGameDirectoryStructureValidator
    {
        string[] Validate(FileNode rootNode);

    }
}