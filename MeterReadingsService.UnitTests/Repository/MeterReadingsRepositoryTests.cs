using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MeterReadingsService.Data;
using MeterReadingsService.Models;
using MeterReadingsService.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FluentAssertions;

namespace MeterReadingsService.UnitTests.Repository
{
    [ExcludeFromCodeCoverage]
    class MeterReadingsRepositoryTests
    {
        private MeterReadingServiceContext _context;
        private MeterReadingsRepository _meterReadingsRepository;

        [SetUp]
        public void Setup()
        {
            SetUpInMemoryDb();
            _meterReadingsRepository = new MeterReadingsRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteInMemoryDb();
        }

        [Test]
        public void AddRange_AddsMeterReadingsToDatabase()
        {
            // arrange 
            var dateTime = new DateTime(2021, 05, 19, 16, 42, 00);

            var meterReadings = new List<MeterReading>
            {
                new MeterReading
                {
                    AccountId = 1,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                },
                new MeterReading
                {
                    AccountId = 2,
                    MeterReadValue = 12345,
                    MeterReadingDateTime = dateTime
                },
            };

            // act
            _meterReadingsRepository.AddRange(meterReadings);

            // assert
            var dbEntries = _context.MeterReadings.ToList();

            dbEntries.Should().BeEquivalentTo(meterReadings, options => options.Excluding(x => x.Id));
        }

        private void SetUpInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<MeterReadingServiceContext>()
                .UseInMemoryDatabase(databaseName: "testDb")
                .Options;

            _context = new MeterReadingServiceContext(options);
        }

        private void DeleteInMemoryDb()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
