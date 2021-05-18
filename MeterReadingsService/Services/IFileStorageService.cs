using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeterReadingsService.Services
{
    public interface IFileStorageService
    {
        public Task<string> Store(IFormFile file);

        public void Delete(string filePath);
    }
}
