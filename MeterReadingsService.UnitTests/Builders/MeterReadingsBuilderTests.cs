using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using FluentAssertions;
using MeterReadingsService.Builders;
using MeterReadingsService.Models;
using MeterReadingsService.Validator;
using Moq;
using NUnit.Framework;

namespace MeterReadingsService.UnitTests.Builders
{
    [ExcludeFromCodeCoverage]
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
            var dateTime = new DateTime(2021, 05, 19, 16, 42, 00);

            dynamic readingOne = new ExpandoObject();
            readingOne.AccountId = "1";
            readingOne.MeterReadValue = "12345";
            readingOne.MeterReadingDateTime = dateTime.ToString();

            dynamic readingTwo = new ExpandoObject();
            readingTwo.AccountId = "2";
            readingTwo.MeterReadValue = "12345";
            readingTwo.MeterReadingDateTime = dateTime.ToString();

            var csvRows = new List<dynamic>
            {
                readingOne,
                readingTwo
            };

            var expectedResult = new List<MeterReading>
            {
                new MeterReading
                {
                    AccountId = 1,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                },
                new  MeterReading
                {
                    AccountId = 2,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                }
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<ExpandoObject>())).Returns(true);

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
            var dateTime = new DateTime(2021, 05, 19, 16, 42, 00);

            dynamic validReading = new ExpandoObject();
            validReading.AccountId = "1";
            validReading.MeterReadValue = "12345";
            validReading.MeterReadingDateTime = dateTime.ToString();


            var csvRows = new List<dynamic>
            {
                validReading,
                new object()
            };


            var expectedResult = new List<MeterReading>
            {
                new MeterReading
                {
                    AccountId = 1,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                }
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<object>())).Returns(false);
            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<ExpandoObject>())).Returns(true);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Build_GivenDuplicateReadings_ShouldNotIncludedDuplicateResults()
        {
            // arrange 
            var dateTime = new DateTime(2021, 05, 19, 16, 42, 00);

            dynamic duplicatedReading = new ExpandoObject();
            duplicatedReading.AccountId = "1";
            duplicatedReading.MeterReadValue = "12345";
            duplicatedReading.MeterReadingDateTime = dateTime.ToString();

            dynamic readingForSameAccountWithDifferentValue = new ExpandoObject();
            readingForSameAccountWithDifferentValue.AccountId = "1";
            readingForSameAccountWithDifferentValue.MeterReadValue = "98765";
            readingForSameAccountWithDifferentValue.MeterReadingDateTime = dateTime.ToString();

            dynamic readingForDifferentAccount = new ExpandoObject();
            readingForDifferentAccount.AccountId = "2";
            readingForDifferentAccount.MeterReadValue = "12345";
            readingForDifferentAccount.MeterReadingDateTime = dateTime.ToString();

            var csvRows = new List<dynamic>
            {
                duplicatedReading,
                duplicatedReading,
                readingForSameAccountWithDifferentValue,
                readingForDifferentAccount
            };

            var expectedResult = new List<MeterReading>
            {
                new MeterReading
                {
                    AccountId = 1,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                },
                new  MeterReading
                {
                    AccountId = 1,
                    MeterReadValue = 98765,
                    MeterReadingDateTime = dateTime
                },
                new  MeterReading
                {
                    AccountId = 2,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                },
            };

            _mockMeterReadingsValidator.Setup(x => x.IsValid(It.IsAny<ExpandoObject>())).Returns(true);

            // act
            var actualResult = _meterReadingsBuilder.Build(csvRows);

            // assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
