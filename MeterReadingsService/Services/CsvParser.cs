using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using MeterReadingsService.Builders;

namespace MeterReadingsService.Services
{
    public class CsvParser : ICsvParser
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICsvReaderBuilder _csvReaderBuilder;

        public CsvParser(IFileSystem filesystem, ICsvReaderBuilder csvReaderBuilder)
        {
            _fileSystem = filesystem;
            _csvReaderBuilder = csvReaderBuilder;
        }

        public List<dynamic> Parse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!_fileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} not found");
            }

            var streamReader = _fileSystem.File.OpenText(filePath);

            var csvReader = _csvReaderBuilder.Build(streamReader);

            List<dynamic> records;

            using (csvReader)
            {
                records = csvReader.GetRecords<dynamic>().ToList();
            }

            return records;
        }
    }
}
