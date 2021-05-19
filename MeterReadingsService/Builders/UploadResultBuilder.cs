using System.Collections.Generic;
using MeterReadingsService.Models;

namespace MeterReadingsService.Builders
{
    public class UploadResultBuilder : IUploadResultBuilder
    {
        public UploadResult Build(List<MeterReading> successfulReading, List<dynamic> csvRows)
        {
            return new UploadResult
            {
                NumberOfSuccessfulReading = successfulReading.Count,
                NumberOfUnsuccessfulReading = csvRows.Count - successfulReading.Count
            };
        }
    }
}
