using System.Collections.Generic;
using MeterReadingsService.Models;

namespace MeterReadingsService.Builders
{
    public interface IUploadResultBuilder
    {
        public UploadResult Build(List<MeterReading> successfulReading, List<dynamic> csvRows);
    }
}
