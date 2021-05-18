using System;

namespace MeterReadingsService.Services
{
    public interface IGuidGenerator
    {
        public Guid Generate();
    }
}