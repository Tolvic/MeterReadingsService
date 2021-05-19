using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MeterReadingsService.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Services
{
    class FileStorageServiceTests
    {
        private FileStorageService _fileStorageService;
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IFormFile> _mockFile;
        private Mock<IGuidGenerator> _mockGuidGenerator;

        [SetUp]
        public void Setup()
        {
            _mockFile = new Mock<IFormFile>();
            _mockFileSystem = new Mock<IFileSystem>();
            _mockGuidGenerator = new Mock<IGuidGenerator>();
            _fileStorageService = new FileStorageService(_mockFileSystem.Object, _mockGuidGenerator.Object);
        }
        
        [Test]
        public void Store_GivenNullFile_ThrowsNullReferenceException()
        {
            // arrange

            // act 
            Func<Task> result = async () => await _fileStorageService.Store(null);


            // assert
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("file");
        }

        [Test]
        public void Store_GivenAFile_ShouldCallFileSystemPathGetExtension()
        {
            // arrange
            var fileName = "example.csv";

            _mockFile.SetupGet(x => x.FileName).Returns(fileName);

            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName));

            // act
            _ = _fileStorageService.Store(_mockFile.Object);

            // assert
            _mockFileSystem.Verify(x => x.Path.GetExtension(fileName), Times.Once());
        }

        [Test]
        public void Store_GivenAFile_ShouldCallFileSystemPathGetTempPath()
        {
            // arrange
            var fileName = "example.csv";

            _mockFile.SetupGet(x => x.FileName).Returns(fileName);

            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName)).Returns(".csv");

            // act
            _ = _fileStorageService.Store(_mockFile.Object);

            // assert
            _mockFileSystem.Verify(x => x.Path.GetTempPath(), Times.Once());
        }

        [Test]
        public void Store_GivenAFile_ShouldCallGUidGeneratorGenerate()
        {
            // arrange
            var fileName = "example.csv";
            _mockFile.SetupGet(x => x.FileName).Returns(fileName);

            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName)).Returns(".csv");

            // act
            _ = _fileStorageService.Store(_mockFile.Object);

            // assert
            _mockGuidGenerator.Verify(x => x.Generate(), Times.Once());
        }

        [Test]
        public void Store_GivenAFile_ShouldCallFileSystemFileStreamCreate()
        {
            // arrange
            var fileName = "example.csv";
            var guid = new Guid("99999999-9999-9999-9999-999999999999");


            _mockFile.SetupGet(x => x.FileName).Returns(fileName);

            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName)).Returns(".csv");
            _mockFileSystem.Setup(x => x.Path.GetTempPath()).Returns("c:\\temp\\");
            _mockGuidGenerator.Setup(x => x.Generate()).Returns(guid);

            _mockFileSystem.Setup(x => x.FileStream.Create("c:\\temp\\99999999-9999-9999-9999-999999999999.csv", FileMode.Create));


            // act
            _ = _fileStorageService.Store(_mockFile.Object);

            // assert
            _mockFileSystem.Verify(x => x.FileStream.Create("c:\\temp\\99999999-9999-9999-9999-999999999999.csv", FileMode.Create), Times.Once());
        }

        [Test]
        public void Store_GivenAFile_ShouldCallFileCopyToAsync()
        {
            // arrange
            var fileName = "example.csv";
            var stream = (Stream)new MemoryStream();
            
            
            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName)).Returns(".csv");
            _mockFileSystem.Setup(x => x.Path.GetTempPath()).Returns("c:\\temp\\");
            _mockFileSystem.Setup(x => x.FileStream.Create(It.IsAny<string>(), FileMode.Create)).Returns(stream);

            _mockFile.SetupGet(x => x.FileName).Returns(fileName);
            _mockFile.Setup(x => x.CopyToAsync(stream, default));

            // act
            _ = _fileStorageService.Store(_mockFile.Object);

            // assert
            _mockFile.Verify(x => x.CopyToAsync(stream, default), Times.Once());
        }

        [Test]
        public async Task Store_GivenAFile_ShouldReturnFilePathFileHasbeenCopiedTo()
        {
            // arrange
            var fileName = "example.csv";
            var guid = new Guid("99999999-9999-9999-9999-999999999999");


            _mockFile.SetupGet(x => x.FileName).Returns(fileName);

            _mockFileSystem.Setup(x => x.Path.GetExtension(fileName)).Returns(".csv");
            _mockFileSystem.Setup(x => x.Path.GetTempPath()).Returns("c:\\temp\\");
            _mockGuidGenerator.Setup(x => x.Generate()).Returns(guid);

            _mockFileSystem.Setup(x => x.FileStream.Create("c:\\temp\\99999999-9999-9999-9999-999999999999.csv", FileMode.Create));


            // act
            var result = await _fileStorageService.Store(_mockFile.Object);

            // assert
            result.Should().Be("c:\\temp\\99999999-9999-9999-9999-999999999999.csv");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Delete_GivenInvalidFilePath_ThrowsArgumentNullException(string filePath)
        {
            // arrange


            // act
            Action result = () => _fileStorageService.Delete(filePath);

            // assert
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("filePath");
        }

        [Test]
        public void Delete_GivenValidFilePath_CallsFileSystemFileExists()
        {
            // arrange
            var filePath = "c\\temp\filename.csv";
            _mockFileSystem.Setup(x => x.File.Exists(filePath));


            // act
            _fileStorageService.Delete(filePath);

            // assert
            _mockFileSystem.Verify(x => x.File.Exists(filePath), Times.Once());
        }

        [Test]
        public void Delete_GivenFileExists_CallsFileSystemFileDelete()
        {
            // arrange
            var filePath = "c\\temp\filename.csv";
            _mockFileSystem.Setup(x => x.File.Exists(filePath)).Returns(true);


            // act
            _fileStorageService.Delete(filePath);

            // assert
            _mockFileSystem.Verify(x => x.File.Delete(filePath), Times.Once());
        }

        [Test]
        public void Delete_GivenFileDoesNotExist_DoesNotCallsFileSystemFileDelete()
        {
            // arrange
            var filePath = "c\\temp\filename.csv";
            _mockFileSystem.Setup(x => x.File.Exists(filePath)).Returns(false);

            // act
            _fileStorageService.Delete(filePath);

            // assert
            _mockFileSystem.Verify(x => x.File.Delete(filePath), Times.Never);
        }
    }
}
