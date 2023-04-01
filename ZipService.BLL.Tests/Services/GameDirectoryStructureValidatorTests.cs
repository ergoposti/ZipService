using ZipService.BLL.Services;
using ZipService.Shared;

namespace ZipService.BLL.Tests.Services
{
    // TODO Better to do assertions with collection assert instead of strictly as atm. Test failures will be clearer
    [TestClass]
    public class GameDirectoryStructureValidatorTests
    {
        private readonly IGameDirectoryStructureValidator _validator;

        public GameDirectoryStructureValidatorTests()
        {
            _validator = new GameDirectoryStructureValidator();
        }

        [TestMethod]
        public void Validate_WithValidStructure_ReturnsEmptyArray()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
            {
                new FileNode("dlls", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("MyGame.dll", false)
                    }
                },
                new FileNode("images", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("0.jpg", false),
                        new FileNode("1.jpg", false)
                    }
                },
                new FileNode("languages", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("MyGame_en.xml", false),
                        new FileNode("MyGame_es.xml", false)
                    }
                }
            }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Validate_WithMissingDllsDirectory_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
            {
                new FileNode("images", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("0.jpg", false)
                    }
                },
                new FileNode("languages", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("MyGame_en.xml", false),
                        new FileNode("MyGame_es.xml", false)
                    }
                }
            }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("dlls directory is missing", result[0]);
        }

        [TestMethod]
        public void Validate_WithInvalidFileTypeInDllsFolder_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false),
                            new FileNode("OtherGame.exe", false),
                        }
                    },
                    new FileNode("images", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("0.jpg", false)
                        }
                    },
                    new FileNode("languages", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame_en.xml", false),
                            new FileNode("MyGame_es.xml", false)
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("MyGame/dlls/OtherGame.exe has invalid file type", result[0]);
        }

        [TestMethod]
        public void Validate_WithMissingImagesDirectory_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false)
                        }
                    },
                    new FileNode("languages", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame_en.xml", false),
                            new FileNode("MyGame_es.xml", false)
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("images directory is missing", result[0]);
        }

        [TestMethod]
        public void Validate_WithMissingImages_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false)
                        }
                    },
                    new FileNode("languages", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame_en.xml", false),
                            new FileNode("MyGame_es.xml", false)
                        }
                    },
                    new FileNode("images", true)
                    {
                        Children = new List<FileNode>(),
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("images directory doesn't contain any image files", result[0]);
        }

        [TestMethod]
        public void Validate_WithInvalidFileInImagesFolder_ReturnsEmptyArray()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
            {
                new FileNode("dlls", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("MyGame.dll", false)
                    }
                },
                new FileNode("images", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("0.jpg", false),
                        new FileNode("1.gif", false),
                    }
                },
                new FileNode("languages", true)
                {
                    Children = new List<FileNode>
                    {
                        new FileNode("MyGame_en.xml", false),
                        new FileNode("MyGame_es.xml", false)
                    }
                }
            }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("MyGame/images/1.gif has invalid file type", result[0]);
        }

        [TestMethod]
        public void Validate_WithMissingLanguageDirectory_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false)
                        }
                    },
                    new FileNode("images", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("0.jpg", false)
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("languages directory is missing", result[0]);
        }

        [TestMethod]
        public void Validate_WithInvalidLanguageFormat_ReturnsError()
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false)
                        }
                    },
                    new FileNode("images", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("0.jpg", false),
                            new FileNode("1.jpg", false)
                        }
                    },
                    new FileNode("languages", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame_en.txt", false),
                            new FileNode("MyGame_es.xml", false)
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("MyGame/languages/MyGame_en.txt has invalid file type", result[0]);
        }

        [TestMethod]
        [DataRow("MyGame_ess")]
        [DataRow("WrongName_es")]
        public void Validate_WithInvalidLanguageNaming_ReturnsError(string languageFilename)
        {
            // Arrange
            var rootNode = new FileNode("MyGame", true)
            {
                Children = new List<FileNode>
                {
                    new FileNode("dlls", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame.dll", false)
                        }
                    },
                    new FileNode("images", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("0.jpg", false),
                            new FileNode("1.jpg", false)
                        }
                    },
                    new FileNode("languages", true)
                    {
                        Children = new List<FileNode>
                        {
                            new FileNode("MyGame_en.xml", false),
                            new FileNode($"{languageFilename}.xml", false)
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(rootNode);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual($"MyGame/languages/{languageFilename}.xml has incorrect file name", result[0]);
        }
    }
}
