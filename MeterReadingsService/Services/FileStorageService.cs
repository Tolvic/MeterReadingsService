using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeterReadingsService.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGuidGenerator _guidGenerator;

        public FileStorageService(IFileSystem fileSystem, IGuidGenerator guidGenerator)
        {
            _fileSystem = fileSystem;
            _guidGenerator = guidGenerator;

        }

        public async Task<string> Store(IFormFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var extension = _fileSystem.Path.GetExtension(file.FileName);

            var path = _fileSystem.Path.GetTempPath() + _guidGenerator.Generate() + extension;

            await using (var stream = _fileSystem.FileStream.Create(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;
        }

        public void Delete(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (_fileSystem.File.Exists(filePath))
            {
                _fileSystem.File.Delete(filePath);
            }
        }
    }
}
