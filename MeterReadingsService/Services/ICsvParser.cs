using System.Collections.Generic;

namespace MeterReadingsService.Services
{
    public interface ICsvParser
    {
        public List<dynamic> Parse(string filePath);
    }
}