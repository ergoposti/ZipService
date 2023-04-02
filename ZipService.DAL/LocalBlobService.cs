using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace ZipService.DAL
{
    public class LocalBlobService : IBlobService
    {
        private readonly string _directoryPath;

        public LocalBlobService()
        {
            _directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public async Task<Result<Stream>> LoadFile(Guid fileId)
        {
            try
            {
                var filePath = Path.Combine(_directoryPath, $"{fileId}.zip");

                var stream = File.OpenRead(filePath);
                return new Result<Stream>(stream);
            }
            catch (Exception ex)
            {
                return new Result<Stream>(ex);
            }
        }

        public async Task<Result<Guid>> SaveFile(Stream fileStream)
        {
            try
            {
                Guid fileId = Guid.NewGuid();
                string filePath = Path.Combine(_directoryPath, $"{fileId}.zip");

                fileStream.Position = 0;

                using (FileStream stream = File.Create(filePath))
                {
                    await fileStream.CopyToAsync(stream);
                }

                return new Result<Guid>(fileId);
            }
            catch (Exception ex)
            {
                return new Result<Guid>(ex);
            }
        }
    }
}
