namespace ZipService.Shared
{
    public class FileNode
    {
        public string Name { get; }
        public bool IsDirectory { get; }
        public List<FileNode> Children { get; init; } = new List<FileNode>();

        public FileNode(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (FileNode)obj;

            if (Name != other.Name || IsDirectory != other.IsDirectory)
            {
                return false;
            }

            if (Children.Count != other.Children.Count)
            {
                return false;
            }

            for (var i = 0; i < Children.Count; i++)
            {
                if (!Children[i].Equals(other.Children[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(IsDirectory);

            foreach (var child in Children)
            {
                hash.Add(child);
            }

            return hash.ToHashCode();
        }
    }
}