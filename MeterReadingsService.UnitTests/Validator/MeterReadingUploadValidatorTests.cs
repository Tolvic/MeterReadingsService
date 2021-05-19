using System;
using System.Dynamic;
using MeterReadingsService.Validator;
using NUnit.Framework;
using FluentAssertions;
using MeterReadingsService.Repositories;
using Moq;

namespace MeterReadingsService.UnitTests.Validator
{
    class MeterReadingUploadValidatorTests
    {
        private MeterReadingUploadsValidator _validator;
        private string _date;
        private Mock<IAccountsRepository> _mockAccountsRepository;

        [SetUp]
        public void Setup()
        {
            _date = new DateTime(2021, 05, 19, 16, 42, 00).ToString();
            _mockAccountsRepository = new Mock<IAccountsRepository>();
            _mockAccountsRepository.Setup(x => x.Exists(It.IsAny<int>())).Returns(true);
            _validator = new MeterReadingUploadsValidator(_mockAccountsRepository.Object);
        }

        [Test]
        public void IsValid_GivenCsvRowWithMoreThanThreeProperties_ShouldReturnFalse()
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "1";
            csvRow.MeterReadingDateTime = _date;
            csvRow.MeterReadValue = "12345";
            csvRow.AnotherProperty = "asdasdsad";

            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_GivenCsvRowWithLessThanThreeProperties_ShouldReturnFalse()
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "1";
            csvRow.MeterReadingDateTime = _date;

            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_GivenCsvRowWithIncorrectProperties_ShouldReturnFalse()
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "1";
            csvRow.MeterReadingDateTime = _date;
            csvRow.AnotherProperty = "asdasdsad";

            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_GivenCsvRowWithInvalidAccountID_ShouldReturnFalse()
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "abcd";
            csvRow.MeterReadingDateTime = _date;
            csvRow.MeterReadValue = "12345";


            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsValid_GivenCsvRowWithInvalidMeterReadingDateTime_ShouldReturnFalse()
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "1";
            csvRow.MeterReadingDateTime = "dsadssads";
            csvRow.MeterReadValue = "12345";


            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase("123456")]
        [TestCase("void")]
        [TestCase("-1234")]
        [TestCase("abcde")]
        public void IsValid_GivenCsvRowWithInvalidMeterReadValue_ShouldReturnFalse(string meterReadingValue)
        {
            // arrange
            dynamic csvRow = new ExpandoObject();
            csvRow.AccountId = "1";
            csvRow.MeterReadingDateTime = _date;
            csvRow.MeterReadValue = meterReadingValue;


            // act
            bool result = _validator.IsValid(csvRow);

            // assert
            result.Should().BeFalse();
        }
    }
}
