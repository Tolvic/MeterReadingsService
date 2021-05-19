using MeterReadingsService.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsService.Data
{
    public class MeterReadingServiceContext : DbContext
    {
        public MeterReadingServiceContext(DbContextOptions<MeterReadingServiceContext> options) : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
