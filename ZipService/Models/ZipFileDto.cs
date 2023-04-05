namespace ZipService.Models
{
    public record ZipFileDto(Guid Id, Guid FileId, string Name, FileNodeDto[] ChildNodes) : FileNodeDto(Name, true, ChildNodes);
}
