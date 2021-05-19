using System;
using System.Diagnostics.CodeAnalysis;
using MeterReadingsService.Services;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    class GuidGeneratorTests
    {
        [Test]
        public void Generate_ReturnsAGuid()
        {
            // arrange
            var guidGenerator = new GuidGenerator();

            // act
            var result = guidGenerator.Generate();

            // Assert
            Assert.IsInstanceOf<Guid>(result);
        }
    }
}
