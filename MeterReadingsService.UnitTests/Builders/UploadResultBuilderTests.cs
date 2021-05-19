using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MeterReadingsService.Builders;
using MeterReadingsService.Models;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Builders
{
    [ExcludeFromCodeCoverage]
    class UploadResultBuilderTests
    {
        [Test]
        public void Build_ReturnsExpectedResult()
        {
            // arrange
            var meterReadings = new List<MeterReading>
            {
                new MeterReading(),
            };

            var csvRows = new List<dynamic>
            {
                new object(),
                new object(),
                new object()
            };

            var expectedResult = new UploadResult
            {
                NumberOfSuccessfulReading = 1,
                NumberOfUnsuccessfulReading = 2
            };

            var builder = new UploadResultBuilder();

            // act
            var result = builder.Build(meterReadings, csvRows);

            // assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
