namespace ZipService.Models
{
    public record ZipFileDto(Guid Id, string Name, FileNodeDto[] ChildNodes) : FileNodeDto(Name, true, ChildNodes);
}
