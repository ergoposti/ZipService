namespace ZipService.Models
{
    public record FileNodeDto(string Name, bool isDirectory, FileNodeDto[] ChildNodes);
}
