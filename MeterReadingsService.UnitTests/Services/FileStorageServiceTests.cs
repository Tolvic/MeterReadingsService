using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeterReadingsService.Services;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Services
{
    class FileStorageServiceTests
    {
        [Test]
        public async Task Store_GivenNullFile_ThrowsNullReferenceException()
        {
            // arrange
            var fileStorageService = new FileStorageService();


            // act 
            Action result = () => fileStorageService.Store(null);


            // assert
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a file must be provided");

        }
    }
}
