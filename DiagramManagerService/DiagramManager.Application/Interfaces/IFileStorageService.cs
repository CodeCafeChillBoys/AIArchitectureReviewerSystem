using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DiagramManager.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder);
        Task<byte[]> ReadFileAsync(string relativeUrl);
    }
}
