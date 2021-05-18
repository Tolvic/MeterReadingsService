using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsService.Controllers
{
    [ApiController]
    [Route("api")]
    public class MeterReadingsController : ControllerBase
    {
        [HttpPost("meter-reading-uploads")]
        public IActionResult Upload(IFormFile file)
        {
            if (!IsCsv(file))
            {
                return BadRequest("csv file required");
            }

            return Ok();
        }

        private bool IsCsv(IFormFile file)
        {
            return file != null && file.FileName.EndsWith(".csv");
        }
    }
}

