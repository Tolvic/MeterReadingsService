using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterReadingsService.Validator;
using NUnit.Framework;
using FluentAssertions;

namespace MeterReadingsService.UnitTests.Validator
{
    class MeterReadingUploadValidatorTests
    {
        private MeterReadingUploadsValidator _validator;
        private string _date;

        [SetUp]
        public void Setup()
        {
            _date = new DateTime(2021, 05, 19, 16, 42, 00).ToString();
            _validator = new MeterReadingUploadsValidator();
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
