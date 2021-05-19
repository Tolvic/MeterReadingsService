using System.Collections.Generic;
using MeterReadingsService.Models;

namespace MeterReadingsService.Builders
{
    public interface IMeterReadingsBuilder
    {
        public List<MeterReading> Build(List<dynamic> csvRows);
    }
}
