using System.Globalization;
using System.IO;
using CsvHelper;

namespace MeterReadingsService.Builders
{
    public class CsvReaderBuilder : ICsvReaderBuilder
    {
        public IReader Build(StreamReader streamReader)
        {
            return new CsvReader(streamReader, CultureInfo.InvariantCulture);
        }

    }
}
