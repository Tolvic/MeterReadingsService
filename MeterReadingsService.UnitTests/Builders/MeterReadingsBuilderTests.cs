using System;
using System.Collections.Generic;
using FluentAssertions;
using MeterReadingsService.Builders;
using MeterReadingsService.Models;
using MeterReadingsService.Validator;
using Moq;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Builders
{
    class MeterReadingsBuilderTests
    {
        private Mock<IMeterReadingUploadsValidator> _mockMeterReadingsValidator;
        private MeterReadingsBuilder _meterReadingsBuilder;

        [SetUp]
        public void Setup()
        {
            _mockMeterReadingsValidator = new Mock<IMeterReadingUploadsValidator>();
            _meterReadingsBuilder = new MeterReadingsBuilder(_mockMeterReadingsValidator.Object);
        }

        [Test]
        public void Build_GivenNullCsvRows_ShouldThrowArgumentNullException()
        {
            // arrange 

            // act
            Action result = () => _meterReadingsBuilder.Build(null);

            // assert
            result.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("csvRows");
        }

        [Test]
        public void Build_GivenEmptyCsvRows_ShouldThrowArgumentException()
        {
            // arrange 
            var emptyRows = new List<dynamic>();

            // act
            Action result = () => _meterReadingsBuilder.Build(emptyRows);

            // assert
            result.Should().Throw<ArgumentException>().And.Message.Should().Be("csvRows should contain at least 1 row");
        }

        [Test]
        public void Build_GivenValidCsvRows_ShouldCallIsValid()
        {
            // arrange 
            var csvRows = new List<dynamic>
            {
                new object(),
                new object()
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<object>())).Returns(false);

            // act
            _ = _meterReadingsBuilder.Build(csvRows);

            // assert
            _mockMeterReadingsValidator.Verify(x => x.IsValid(It.IsAny<object>()), Times.Exactly(csvRows.Count));
        }

        [Test]
        public void Build_GivenValidCsvRows_ShouldReturnExpectedResultsWhenAllRowsAreValid()
        {
            // arrange 
            var readingOne = new MeterReading
            {
                AccountId = 1,
                MeterReadValue = 12345,
                MeterReadingDateTime = DateTime.Now
            };
            var readingTwo = new MeterReading
            {
                AccountId = 2,
                MeterReadValue = 12345,
                MeterReadingDateTime = DateTime.Now
            };

            var csvRows = new List<dynamic>
            {
                readingOne,
                readingTwo
            };

            var expectedResult = new List<MeterReading>
            {
                readingOne,
                readingTwo
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<object>())).Returns(true);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Build_GivenValidCsvRows_ShouldReturnExpectedResultsWhenAllRowsAreNotValid()
        {
            // arrange 
            var csvRows = new List<dynamic>
            {
                new object(),
                new object()
            };

            var expectedResult = new List<MeterReading>();

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<object>())).Returns(false);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Build_GivenValidCsvRows_ShouldReturnExpectedResultsWhenThereIsAMixOFValidAndInValidRows()
        {
            // arrange 
            var validReading = new MeterReading
            {
                AccountId = 1,
                MeterReadValue = 12345,
                MeterReadingDateTime = DateTime.Now
            };

            var csvRows = new List<dynamic>
            {
                validReading,
                new object()
            };

            var expectedResult = new List<MeterReading>
            {
                validReading
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<object>())).Returns(false);
            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<MeterReading>())).Returns(true);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Build_GivenDuplicateReadings_ShouldNotIncludedDuplicateResults()
        {
            // arrange 
            var duplicatedReading = new MeterReading
            {
                AccountId = 1,
                MeterReadValue = 12345,
                MeterReadingDateTime = DateTime.Now
            };

            var readingForSameAccountWithDifferentValue = new MeterReading
            {
                AccountId = 1,
                MeterReadValue = 98765,
                MeterReadingDateTime = DateTime.Now
            };

            var readingForDifferentAccount = new MeterReading
            {
                AccountId = 99,
                MeterReadValue = 12345,
                MeterReadingDateTime = DateTime.Now
            };

            var csvRows = new List<dynamic>
            {
                duplicatedReading,
                duplicatedReading,
                readingForSameAccountWithDifferentValue,
                readingForDifferentAccount
            };

            var expectedResult = new List<MeterReading>
            {
                duplicatedReading,
                readingForSameAccountWithDifferentValue,
                readingForDifferentAccount
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<MeterReading>())).Returns(true);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
