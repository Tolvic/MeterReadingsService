using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using CsvHelper;
using FluentAssertions;
using MeterReadingsService.Builders;
using Moq;
using NUnit.Framework;
using CsvParser = MeterReadingsService.Services.CsvParser;

namespace MeterReadingsService.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    class CsvParserTests
    {
        private CsvParser _csvParser;
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<ICsvReaderBuilder> _mockCsvReaderBuilder;
        private Mock<IReader> _mockCsvReader;
        private StreamReader _streamReader;
        private const string DummyFilePath = "c:\\Temp\\Dummy.csv";

        [SetUp]
        public void Setup()
        {
            _mockCsvReader = new Mock<IReader>();
            _mockFileSystem = new Mock<IFileSystem>();
            _mockCsvReaderBuilder = new Mock<ICsvReaderBuilder>();
            _csvParser = new CsvParser(_mockFileSystem.Object, _mockCsvReaderBuilder.Object);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Parse_GivenAInvalidFilePath_ThrowsAnArgumentNullException(string filePath)
        {
            // arrange 

            // act
            Action result = () => _csvParser.Parse(filePath);

            // assert
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("filePath");
        }

        [Test]
        public void Parse_GivenAValidFilePath_ShouldCallFileSystemFileExists()
        {
            // arrange 
            InitDefaultMocking();

            // act
            _ = _csvParser.Parse(DummyFilePath);

            // assert
            _mockFileSystem.Verify(x => x.File.Exists(DummyFilePath), Times.Once);
        }

        [Test]
        public void Parse_GiveFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // arrange 
            InitDefaultMocking();
            _mockFileSystem.Setup(x => x.File.Exists(DummyFilePath)).Returns(false);

            // act
            Action result = () => _csvParser.Parse(DummyFilePath);

            // assert
            result.Should().Throw<FileNotFoundException>().And.Message.Should().Be($"{DummyFilePath} not found");
        }

        [Test]
        public void Parse_GivenValidFile_CallsFileSystemFileOpenText()
        {
            // arrange 
            InitDefaultMocking();

            // act
            _ = _csvParser.Parse(DummyFilePath);

            // assert
            _mockFileSystem.Verify(x => x.File.OpenText(DummyFilePath), Times.Once);
        }

        [Test]
        public void Parse_GivenValidFile_CallsCsvReaderBuilderBuild()
        {
            // arrange 
            InitDefaultMocking();

            // act
            _ = _csvParser.Parse(DummyFilePath);

            // assert
            _mockCsvReaderBuilder.Verify(x => x.Build(_streamReader), Times.Once);
        }

        [Test]
        public void Parse_GivenValidFile_csvReaderGetRecords()
        {
            // arrange 
            InitDefaultMocking();

            // act
            _ = _csvParser.Parse(DummyFilePath);

            // assert

            _mockCsvReader.Verify(x => x.GetRecords<dynamic>(), Times.Once);
        }

        [Test]
        public void Parse_GivenValidFile_returnsListofRecords()
        {
            // arrange 
            InitDefaultMocking();
            var recordsArray = new dynamic[0];
            var expectedResult = new List<dynamic>();
            _mockCsvReader.Setup(x => x.GetRecords<dynamic>()).Returns(recordsArray);

            // act
            var actualResult = _csvParser.Parse(DummyFilePath);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);

        }

        private void InitDefaultMocking()
        {
            string fakeFileContents = "Hello world";
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(fakeFileContents);

            var fakeMemoryStream = new MemoryStream(fakeFileBytes);

            _streamReader = new StreamReader(fakeMemoryStream);

            _mockFileSystem.Setup(x => x.File.OpenText(DummyFilePath)).Returns(_streamReader);
            _mockCsvReaderBuilder.Setup(x => x.Build(_streamReader)).Returns(_mockCsvReader.Object);
            _mockFileSystem.Setup(x => x.File.Exists(DummyFilePath)).Returns(true);
        }
    }
}
