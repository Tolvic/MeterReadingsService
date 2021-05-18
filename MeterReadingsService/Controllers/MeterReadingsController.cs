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

        public MeterReadingsController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
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

