namespace ZipService.Domain
{
    public class FileEntity : BaseEntity
    {
        public string FileName { get; }
        public Guid FileId { get; }

        public FileEntity(Guid fileId, string fileName)
        {
            FileId = fileId;
            FileName = fileName;
        }
    }
}