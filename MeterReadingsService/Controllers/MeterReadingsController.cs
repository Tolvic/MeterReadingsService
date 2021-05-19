using System.Collections.Generic;
using System.Threading.Tasks;
using MeterReadingsService.Builders;
using MeterReadingsService.Models;
using MeterReadingsService.Repositories;
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
        private readonly IUploadResultBuilder _uploadResultBuilder;
        private readonly IMeterReadingsRepository _meterReadingsRepository;

        public MeterReadingsController(IFileStorageService fileStorageService, ICsvParser csvParser, IMeterReadingsBuilder meterReadingsBuilder, IUploadResultBuilder uploadResultBuilder, IMeterReadingsRepository meterReadingsRepository)
        {
            _fileStorageService = fileStorageService;
            _csvParser = csvParser;
            _meterReadingsBuilder = meterReadingsBuilder;
            _uploadResultBuilder = uploadResultBuilder;
            _meterReadingsRepository = meterReadingsRepository;
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!IsCsv(file))
            {
                return BadRequest("csv file required");
            }

            var tempFilePath = await _fileStorageService.Store(file);

            List<dynamic> csvRecords;
            List<MeterReading> meterReadings;

            try
            {
                csvRecords = _csvParser.Parse(tempFilePath);
                meterReadings = _meterReadingsBuilder.Build(csvRecords);
                _meterReadingsRepository.AddRange(meterReadings);
            }
            catch
            {
                return StatusCode(500);
            }
            finally
            {
                _fileStorageService.Delete(tempFilePath);
            }

            var uploadResult = _uploadResultBuilder.Build(meterReadings, csvRecords);

            return Ok(uploadResult);
        }

        private bool IsCsv(IFormFile file)
        {
            return file != null && file.FileName.EndsWith(".csv");
        }
    }
}

