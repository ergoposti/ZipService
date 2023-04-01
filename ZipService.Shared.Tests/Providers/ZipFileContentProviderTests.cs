using ZipService.Shared.Providers;

namespace ZipService.Shared.Tests.Providers
{
    [TestClass]
    public class ZipFileContentProviderTests
    {
        private ZipFileContentProvider _provider;

        public ZipFileContentProviderTests()
        {
            _provider = new ZipFileContentProvider();
        }

        [TestMethod]
        public void GetZipFileTree_ReturnsExpectedTree()
        {
            string testZipFilePath = @"./Assets/CatGame.zip";

            // Arrange
            var expectedTree = new FileNode("CatGame", true)
            {
                Children = new List<FileNode>
                {
                new FileNode("dlls", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("CatGame.dll", false)
                    }
                },
                new FileNode("images", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("0.jpg", false),
                        new FileNode("1.jpg", false),
                        new FileNode("2.jpg", false),
                        new FileNode("3.png", false),
                    }
                },
                new FileNode("languages", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("CatGame_en.xml", false),
                        new FileNode("CatGame_es.xml", false)
                    }
                }
            }
            };

            // Act
            FileNode actualTree;
            using (var fileStream = File.OpenRead(testZipFilePath))
            {
                actualTree = _provider.GetZipFileTree(fileStream, testZipFilePath);
            }

            // Assert
            Assert.IsTrue(expectedTree.Equals(actualTree));
        }
    }
}
