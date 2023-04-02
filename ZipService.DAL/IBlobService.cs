using LanguageExt.Common;

namespace ZipService.DAL
{
    public interface IBlobService
    {
        Task<Result<Guid>> SaveFile(Stream fileStream);
        Task<Result<Stream>> LoadFile(Guid fileId);
    }
}