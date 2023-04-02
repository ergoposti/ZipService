using Shouldly;
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
            result.ShouldBeEmpty();
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("dlls directory is missing");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("MyGame/dlls/OtherGame.exe has invalid file type");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("images directory is missing");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("images directory doesn't contain any image files");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("MyGame/images/1.gif has invalid file type");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("languages directory is missing");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe("MyGame/languages/MyGame_en.txt has invalid file type");
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
            result.Length.ShouldBe(1);
            result[0].ShouldBe($"MyGame/languages/{languageFilename}.xml has incorrect file name");
        }
    }
}
