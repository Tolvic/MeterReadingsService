using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using MeterReadingsService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Controllers
{
    class MeterReadingsControllerTests
    {
        private readonly MeterReadingsController _controller;

        public MeterReadingsControllerTests()
        {
            _controller = new MeterReadingsController();
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
        public void Upload_GivenACsvFile_ShouldReturnOkResult()
        {
            // arrange
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.csv");

            // act
            var result = _controller.Upload(file);

            // assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public void Upload_GivenNullFile_ShouldReturnBadRequestObjectResult()
        {
            // arrange

            // act
            var result = _controller.Upload(null);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void Upload_GivenNullFile_ShouldReturnBadRequestObjectWithErrorMessage()
        {
            // arrange

            // act
            var result = _controller.Upload(null) as BadRequestObjectResult;


            // assert
            result.Value.Should().Be("csv file required");
        }

        [TestCase("dummy.txt")]
        [TestCase("dummy.exe")]
        [TestCase("dummy.xlsx")]
        public void Upload_GiveANonCsvFile_ShouldReturnBadRequestObjectResult(string fileName)
        {
            // arrange
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", fileName);

            // act
            var result = _controller.Upload(file);

            // assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestCase("dummy.txt")]
        [TestCase("dummy.exe")]
        [TestCase("dummy.xlsx")]
        public void Upload_GiveANonCsvFile_ShouldReturnBadRequestObjectWithErrorMessage(string fileName)
        {
            // arrange
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", fileName);

            // act
            var result = _controller.Upload(null) as BadRequestObjectResult;

            // assert
            result.Value.Should().Be("csv file required");
        }
    }
}
