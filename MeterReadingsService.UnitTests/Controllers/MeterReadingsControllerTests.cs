using System.Linq;
using FluentAssertions;
using MeterReadingsService.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Controllers
{
    class MeterReadingsControllerTests
    {
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
        public void Upload_ShouldReturnOkResult()
        {
            // arrange
            var controller = new MeterReadingsController();

            // act
            var result = controller.Upload();

            // assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
