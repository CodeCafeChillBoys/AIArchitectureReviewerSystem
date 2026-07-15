using DiagramManager.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiagramManager.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            string folderPath = Path.Combine(_env.ContentRootPath, "Storage", subFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"Storage/{subFolder}/{fileName}";
        }

        public async Task<byte[]> ReadFileAsync(string relativeUrl)
        {
            string filePath = Path.Combine(_env.ContentRootPath, relativeUrl);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found at " + filePath);
            }

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
