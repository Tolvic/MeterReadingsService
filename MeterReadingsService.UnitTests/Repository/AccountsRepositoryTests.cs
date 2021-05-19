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
    class AccountsRepositoryTests
    {
        private MeterReadingServiceContext _context;
        private AccountsRepository _accountRepository;

        [SetUp]
        public void Setup()
        {
            SetUpInMemoryDb();
            _accountRepository = new AccountsRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteInMemoryDb();
        }

        [Test]
        public void Exists_WhenGiveAnIdInDatabase_shouldReturnTrue()
        {
            // arrange 
            var test = _context.Accounts.ToList();

            // act
            var result = _accountRepository.Exists(1);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Exists_WhenGiveAnIdNotInDatabase_shouldReturnFalse()
        {
            // arrange 


            // act
            var result = _accountRepository.Exists(2);

            // assert
            result.Should().BeFalse();
        }

        private void SetUpInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<MeterReadingServiceContext>()
                .UseInMemoryDatabase(databaseName: "testDb")
                .Options;

            _context = new MeterReadingServiceContext(options);
            _context.Accounts.AddRange(GetSeedData());
            _context.SaveChanges();
        }

        private void DeleteInMemoryDb()
        {
            _context.Database.EnsureDeleted();
        }

        private List<Account> GetSeedData()
        {
            return new List<Account>
            {
                new Account()
                {
                    AccountId = 1,
                    FirstName = "Test",
                    LastName = "Mctesterson"
                }
            };
        }
    }
}
