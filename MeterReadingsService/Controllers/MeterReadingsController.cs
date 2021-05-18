using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsService.Controllers
{
    [ApiController]
    [Route("api")]
    public class MeterReadingsController : ControllerBase
    {
        [HttpPost("meter-reading-uploads")]
        public IActionResult Upload()
        {
            return Ok();
        }
    }
}

