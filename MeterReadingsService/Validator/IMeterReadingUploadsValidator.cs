using System.Dynamic;

namespace MeterReadingsService.Validator
{
    public interface IMeterReadingUploadsValidator
    {
        public bool IsValid(dynamic csvRow);
    }
}
