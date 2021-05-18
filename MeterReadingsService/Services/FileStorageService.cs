using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeterReadingsService.Services
{
    public class FileStorageService : IFileStorageService
    {
        public Task<string> Store(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("a file must be provided");
            }
            throw new NotImplementedException();
        }

        public Task Delete(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
