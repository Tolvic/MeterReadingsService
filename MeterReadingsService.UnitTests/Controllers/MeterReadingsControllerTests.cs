using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MeterReadingsService.Controllers;
using MeterReadingsService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Controllers
{
    class MeterReadingsControllerTests
    {
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly MeterReadingsController _controller;

        public MeterReadingsControllerTests()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _controller = new MeterReadingsController(_mockFileStorageService.Object);
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
        public async Task Upload_GivenACsvFile_ShouldReturnOkResult()
        {
            // arrange
            var file = GetDummyCsvFile();

            // act
            var result = await _controller.Upload(file);

            // assert
            result.Should().BeOfType<OkResult>();
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
            var file = GetDummyCsvFile();

            // act
            _ = _controller.Upload(file);

            // assert
            _mockFileStorageService.Verify((x => x.Store(file)), Times.Once());
        }

        [Test]
        public void Upload_GivenACsvFile_ShouldCallFileStorageServiceDelete()
        {
            // arrange
            var tempFilePath = "\\temp\\tempFile.csv";
            var file = GetDummyCsvFile();

            _mockFileStorageService.Setup(x => x.Store(file)).ReturnsAsync(tempFilePath);

            // act
            _ = _controller.Upload(file);

            // assert
            _mockFileStorageService.Verify((x => x.Delete(tempFilePath)), Times.Once());
        }

        private FormFile GetDummyCsvFile()
        {
            return new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.csv");
        }
    }
}
