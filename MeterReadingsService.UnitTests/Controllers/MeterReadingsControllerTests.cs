using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MeterReadingsService.Builders;
using MeterReadingsService.Controllers;
using MeterReadingsService.Models;
using MeterReadingsService.Repositories;
using MeterReadingsService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Controllers
{
    class MeterReadingsControllerTests
    {
        private const string DummyFilePath = "c:\\Temp\\Dummy.csv";
        private readonly List<dynamic> _dummyCsvRows = new List<dynamic>();
        private readonly List<MeterReading> _dummyMeterReadings = new List<MeterReading>();
        private readonly FormFile _dummyCsvFile; 
        private Mock<IFileStorageService> _mockFileStorageService;
        private Mock<ICsvParser> _mockCsvParser;
        private MeterReadingsController _controller;
        private Mock<IMeterReadingsBuilder> _mockMeterReadingsBuilder;
        private Mock<IUploadResultBuilder> _mockUploadResultBuilder;
        private Mock<IMeterReadingsRepository> _mockMeterReadingsRepository;

        public MeterReadingsControllerTests()
        {
            _dummyCsvFile = GetDummyCsvFile();
        }

        [SetUp]
        public void SetUp()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockCsvParser = new Mock<ICsvParser>();
            _mockMeterReadingsBuilder = new Mock<IMeterReadingsBuilder>();
            _mockUploadResultBuilder = new Mock<IUploadResultBuilder>();
            _mockMeterReadingsRepository = new Mock<IMeterReadingsRepository>();
            _controller = new MeterReadingsController(_mockFileStorageService.Object, _mockCsvParser.Object, _mockMeterReadingsBuilder.Object, _mockUploadResultBuilder.Object, _mockMeterReadingsRepository.Object);
        }

        [Test]
        public void ShouldImplementControllerBase()
        {
            typeof(MeterReadingsController).Should().BeAssignableTo<ControllerBase>();
        }

        [Test]
        public void ShouldBeDecoratedWithApiControllerAttribute()
        {
            typeof(MeterReadingsController).Should().BeDecoratedWith<ApiControllerAttribute>();
        }


        [Test]
        public void ShouldBeDecoratedWithRouteAttribute()
        {
            typeof(MeterReadingsController).Should().BeDecoratedWith<RouteAttribute>(attr => attr.Template == "api");
        }

        [Test]
        public void Upload_ShouldBeDecoratedWithHttpPostAttribute()
        {
            var uploadMethod = typeof(MeterReadingsController).Methods().Single(x => x.Name == "Upload");

            uploadMethod.Should().BeDecoratedWith<HttpPostAttribute>(attr => attr.Template == "meter-reading-uploads");
        }


        [Test]
        public async Task Upload_GivenNullFile_ShouldReturnBadRequestObjectResult()
        {
            // arrange

            // act
            var result = await _controller.Upload(null);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task Upload_GivenNullFile_ShouldReturnBadRequestObjectWithErrorMessage()
        {
            // arrange

            // act
            var result = await _controller.Upload(null) as BadRequestObjectResult;


            // assert
            result!.Value.Should().Be("csv file required");
        }

        [TestCase("dummy.txt")]
        [TestCase("dummy.exe")]
        [TestCase("dummy.xlsx")]
        public async Task Upload_GiveANonCsvFile_ShouldReturnBadRequestObjectResult(string fileName)
        {
            // arrange
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", fileName);

            // act
            var result = await _controller.Upload(file);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestCase("dummy.txt")]
        [TestCase("dummy.exe")]
        [TestCase("dummy.xlsx")]
        public async Task Upload_GiveANonCsvFile_ShouldReturnBadRequestObjectWithErrorMessage(string fileName)
        {
            // arrange
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", fileName);

            // act
            var result = await _controller.Upload(file) as BadRequestObjectResult;

            // assert
            result!.Value.Should().Be("csv file required");
        }

        [Test]
        public void Upload_GivenACsvFile_ShouldCallFileStorageServiceStore()
        {
            // arrange

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockFileStorageService.Verify((x => x.Store(_dummyCsvFile)), Times.Once());
        }

       
        
        [Test]
        public void Upload_GivenACsvFile_ShouldCallFileStorageServiceDelete()
        {
            // arrange
            InitDefaultMocking();

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockFileStorageService.Verify((x => x.Delete(DummyFilePath)), Times.Once());
        }

        [Test]
        public void Upload_GivenACsvFile_ShouldCallCsvParserParse()
        {
            // arrange
            InitDefaultMocking();

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockCsvParser.Verify((x => x.Parse(DummyFilePath)), Times.Once());
        }

        [Test]
        public void Upload_GivenCsvParserThrowsException_ShouldCallFileStorageServiceDelete()
        {
            // arrange
            InitDefaultMocking();
            _mockCsvParser.Setup(x => x.Parse(DummyFilePath)).Throws(new Exception());

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockFileStorageService.Verify((x => x.Delete(DummyFilePath)), Times.Once());
        }

        [Test]
        public async Task Upload_GivenCsvParserThrowsException_Results500Status()
        {
            // arrange
            InitDefaultMocking();
            _mockCsvParser.Setup(x => x.Parse(DummyFilePath)).Throws(new Exception());

            // act
            var result = await _controller.Upload(_dummyCsvFile);

            // assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        }

        [Test]
        public void Upload_GivenACsvFile_ShouldCallCsvMeterReadingsBuilderBuild()
        {
            // arrange
            InitDefaultMocking();

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockMeterReadingsBuilder.Verify((x => x.Build(_dummyCsvRows)), Times.Once());
        }

        [Test]
        public void Upload_GivenMeterReadingsBuilderThrowsException_ShouldCallMeterReadingsBuilderBuild()
        {
            // arrange
            InitDefaultMocking();
            _mockMeterReadingsBuilder.Setup(x => x.Build(_dummyCsvRows)).Throws(new Exception());

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockFileStorageService.Verify((x => x.Delete(DummyFilePath)), Times.Once());
        }

        [Test]
        public async Task Upload_GivenMeterReadingsBuilderThrowsException_Results500Status()
        {
            // arrange
            InitDefaultMocking();
            _mockMeterReadingsBuilder.Setup(x => x.Build(_dummyCsvRows)).Throws(new Exception());

            // act
            var result = await _controller.Upload(_dummyCsvFile);

            // assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        }

        [Test]
        public void Upload_GivenACsvFile_ShouldCallMeterReadingsRepositoryAddRange()
        {
            // arrange
            InitDefaultMocking();

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockMeterReadingsRepository.Verify(x => x.AddRange(_dummyMeterReadings));
        }

        [Test]
        public void Upload_GivenMMeterReadingsRepositoryThrowsException_ShouldCallMeterReadingsBuilderBuild()
        {
            // arrange
            InitDefaultMocking();
            _mockMeterReadingsRepository.Setup(x => x.AddRange(_dummyMeterReadings)).Throws(new Exception());

            // act
            _ = _controller.Upload(_dummyCsvFile);

            // assert
            _mockFileStorageService.Verify((x => x.Delete(DummyFilePath)), Times.Once());
        }

        [Test]
        public async Task Upload_GivenMMeterReadingsRepositoryThrowsException_ResultsIn500Status()
        {
            // arrange
            InitDefaultMocking();
            _mockMeterReadingsRepository.Setup(x => x.AddRange(_dummyMeterReadings)).Throws(new Exception());

            // act
            var result = await _controller.Upload(_dummyCsvFile);

            // assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        }

        [Test]
        public async Task Upload_GivenACsvFile_ShouldCallUploadResultBuilder()
        {
            // arrange
            InitDefaultMocking();

            // act
            _ = await _controller.Upload(_dummyCsvFile);

            // assert
            _mockUploadResultBuilder.Verify(x => x.Build(_dummyMeterReadings, _dummyCsvRows));
        }
        
        [Test]
        public async Task Upload_GivenACsvFile_ShouldReturnOkResultWithUploadResultasValue()
        {
            // arrange
            InitDefaultMocking();
            var uploadResult = new UploadResult();
            _mockUploadResultBuilder.Setup(x => x.Build(_dummyMeterReadings, _dummyCsvRows)).Returns(uploadResult);

            // act
            var result = await _controller.Upload(_dummyCsvFile) as OkObjectResult;


            // assert
            result!.Value.Should().BeEquivalentTo(uploadResult);
        }
        
        private FormFile GetDummyCsvFile()
        {
            return new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.csv");
        }

        private void InitDefaultMocking()
        {
            _mockFileStorageService.Setup(x => x.Store(_dummyCsvFile)).ReturnsAsync(DummyFilePath);
            
            _mockCsvParser.Setup(x => x.Parse(DummyFilePath)).Returns(_dummyCsvRows);

            _mockMeterReadingsBuilder.Setup(x => x.Build(_dummyCsvRows)).Returns(_dummyMeterReadings);
        }
    }
}
