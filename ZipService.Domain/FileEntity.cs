namespace ZipService.Domain
{
    public class FileEntity : BaseEntity
    {
        public string Path { get; set; }

        public FileEntity(string path)
        {
            Path = path;
        }
    }
}