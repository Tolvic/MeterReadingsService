using System.Collections.Generic;
using MeterReadingsService.Models;

namespace MeterReadingsService.Repositories
{
    public interface IMeterReadingsRepository
    {
        public void AddRange(List<MeterReading> meterReadings);
    }
}
