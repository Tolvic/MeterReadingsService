using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using MeterReadingsService.Models;

namespace MeterReadingsService.Data
{
    [ExcludeFromCodeCoverage]
    public class DataSeeder
    {
        private readonly MeterReadingServiceContext _context;
        private const string TestAccountsFileName = "\\Data\\Test_Accounts.csv";

        public DataSeeder(MeterReadingServiceContext context)
        {
            _context = context;
        }

        //TODO Given more time this would be tested and calls to System.IO objects, etc. would be abstracted away similar to the FileStorageService

        public void Seed()
        {
            if (_context.Accounts.Any())
            {
                return;
            }

            _context.Database.EnsureCreated();

            var currentDirectory = Directory.GetCurrentDirectory();

            var filePath = $"{currentDirectory}{TestAccountsFileName}";

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(nameof(filePath));
            }

            var streamReader = new StreamReader(filePath);

            var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            List<Account> records;

            using (csvReader)
            {
                records = csvReader.GetRecords<Account>().ToList();
            }

            _context.Accounts.AddRange(records);

            _context.SaveChanges();
        }
    }
}
