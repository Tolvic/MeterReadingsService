using System.Collections.Generic;
using MeterReadingsService.Data;
using MeterReadingsService.Models;

namespace MeterReadingsService.Repositories
{
    public class MeterReadingsRepository : IMeterReadingsRepository
    {
        private readonly MeterReadingServiceContext _context;

        public MeterReadingsRepository(MeterReadingServiceContext context)
        {
            _context = context;
        }

        public void AddRange(List<MeterReading> meterReadings)
        {
            _context.MeterReadings.AddRange(meterReadings);

            _context.SaveChanges();
        }
    }
}
