using System.Threading.Tasks;
using MeterReadingsService.Builders;
using MeterReadingsService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsService.Controllers
{
    [ApiController]
    [Route("api")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ICsvParser _csvParser;
        private readonly IMeterReadingsBuilder _meterReadingsBuilder;

        public MeterReadingsController(IFileStorageService fileStorageService, ICsvParser csvParser, IMeterReadingsBuilder meterReadingsBuilder)
        {
            _fileStorageService = fileStorageService;
            _csvParser = csvParser;
            _meterReadingsBuilder = meterReadingsBuilder;
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!IsCsv(file))
            {
                return BadRequest("csv file required");
            }

            var tempFilePath = await _fileStorageService.Store(file);

            try
            {
                var csvRecords = _csvParser.Parse(tempFilePath);
                var meterReadings = _meterReadingsBuilder.Build(csvRecords);
            }
            catch
            {
                return StatusCode(500);
            }
            finally
            {
                _fileStorageService.Delete(tempFilePath);
            }


            return Ok();
        }

        private bool IsCsv(IFormFile file)
        {
            return file != null && file.FileName.EndsWith(".csv");
        }
    }
}

