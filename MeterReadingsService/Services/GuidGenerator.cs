using System;

namespace MeterReadingsService.Services
{
    public class GuidGenerator : IGuidGenerator
    {
        public Guid Generate()
        {
            return Guid.NewGuid();
        }
    }
}
