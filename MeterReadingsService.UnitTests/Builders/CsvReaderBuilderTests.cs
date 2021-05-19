using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using CsvHelper;
using FluentAssertions;
using MeterReadingsService.Builders;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Builders
{
    [ExcludeFromCodeCoverage]
    class CsvReaderBuilderTests
    {
        [Test]
        public void Build_ReturnsACsvReader()
        {
            // arrange
            string fakeFileContents = "Hello world";
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(fakeFileContents);
            var fakeMemoryStream = new MemoryStream(fakeFileBytes);
            var streamReader = new StreamReader(fakeMemoryStream);

            var builder = new CsvReaderBuilder();

            // act
            var result = builder.Build(streamReader);

            // assert
            result.Should().BeOfType<CsvReader>();
        }
    }
}
