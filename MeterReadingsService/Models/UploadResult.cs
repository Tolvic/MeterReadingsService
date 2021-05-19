using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsService.Models
{
    [ExcludeFromCodeCoverage]
    public class UploadResult
    {
        public int NumberOfSuccessfulReading { get; set; }
        public int NumberOfUnsuccessfulReading { get; set; }
    }
}
