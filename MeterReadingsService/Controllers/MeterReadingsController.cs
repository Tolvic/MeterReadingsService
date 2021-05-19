using System.Threading.Tasks;
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

        public MeterReadingsController(IFileStorageService fileStorageService, ICsvParser csvParser)
        {
            _fileStorageService = fileStorageService;
            _csvParser = csvParser;
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

