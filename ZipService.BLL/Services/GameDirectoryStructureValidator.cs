using System.Text.RegularExpressions;
using ZipService.Shared;

namespace ZipService.BLL.Services
{

    public class GameDirectoryStructureValidator : IGameDirectoryStructureValidator
    {
        private static string[] AllowedImageExtensions = new[] { ".jpg", ".png" };

        // TODO refacto into pipeline/chain approach, atm there's a lot of code together in one method, this would make it easier to read and modify
        public string[] Validate(FileNode rootNode)
        {
            var errors = new List<string>();

            if (rootNode.Children == null || rootNode.Children.Count == 0)
            {
                errors.Add("root directory is empty");
                return errors.ToArray();
            }

            var dlls = rootNode.Children.SingleOrDefault(c => c.Name == "dlls" && c.IsDirectory);
            if (dlls == null)
            {
                errors.Add("dlls directory is missing");
            }
            else
            {
                if (!dlls.Children.Any(c => c.Name == $"{rootNode.Name}.dll" && !c.IsDirectory))
                {
                    errors.Add($"{rootNode.Name}.dll is missing from dlls directory");
                }

                foreach (var dll in dlls.Children)
                {
                    if (Path.GetExtension(dll.Name) != ".dll")
                    {
                        errors.Add($"{rootNode.Name}/dlls/{dll.Name} has invalid file type");
                    }
                }
            }

            var images = rootNode.Children.SingleOrDefault(c => c.Name == "images" && c.IsDirectory);
            if (images == null)
            {
                errors.Add("images directory is missing");
            }
            else if (images.Children.Count == 0)
            {
                errors.Add("images directory doesn't contain any image files");
            }
            else
            {
                foreach (var image in images.Children)
                {
                    if (!AllowedImageExtensions.Contains(Path.GetExtension(image.Name)))
                    {
                        errors.Add($"{rootNode.Name}/images/{image.Name} has invalid file type");
                    }
                }
            }

            var languages = rootNode.Children.SingleOrDefault(c => c.Name == "languages" && c.IsDirectory);
            if (languages == null)
            {
                errors.Add("languages directory is missing");
            }
            else
            {
                foreach (var languageFile in languages.Children)
                {
                    if (Path.GetExtension(languageFile.Name) != ".xml")
                    {
                        errors.Add($"{rootNode.Name}/languages/{languageFile.Name} has invalid file type");
                    }
                    else if (!IsValidLanguageFileName(rootNode.Name, languageFile.Name))
                    {
                        errors.Add($"{rootNode.Name}/languages/{languageFile.Name} has incorrect file name");
                    }
                }

                if (!languages.Children.Any(x => IsValidLanguageFileName(rootNode.Name, x.Name)))
                {
                    errors.Add("No language files found in languages directory");
                }
            }

            return errors.ToArray();
        }

        private static bool IsValidLanguageFileName(string rootName, string fileName) => Regex.IsMatch(fileName, $"^{rootName}_[a-zA-Z]{{2}}\\.xml$");
    }
}
