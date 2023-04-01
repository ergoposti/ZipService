using System.IO.Compression;

namespace ZipService.Shared.Providers
{
    public class ZipFileContentProvider : IZipFileContentProvider
    {
        public FileNode GetZipFileTree(Stream zipFileStream, string archiveName)
        {
            var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
            var root = new FileNode(Path.GetFileNameWithoutExtension(archiveName), true);

            foreach (var entry in zipArchive.Entries)
            {
                var path = entry.FullName.Split('/').ToList();
                var currentNode = root;

                for (var i = 0; i < path.Count; i++)
                {
                    var name = path[i];

                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    var isDirectory = i < path.Count - 1 || entry.Length == 0;

                    var childNode = currentNode.Children.FirstOrDefault(c => c.Name == name && c.IsDirectory == isDirectory);

                    if (childNode == null)
                    {
                        childNode = new FileNode(name, isDirectory);
                        currentNode.Children.Add(childNode);
                    }

                    currentNode = childNode;
                }
            }

            return root;
        }
    }
}
