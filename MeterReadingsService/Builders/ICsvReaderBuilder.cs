using System.IO;
using CsvHelper;

namespace MeterReadingsService.Builders
{
    public interface ICsvReaderBuilder
    {
        public IReader Build(StreamReader streamReader);
    }
}