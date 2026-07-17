using System.IO;
using System.Threading.Tasks;

namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface ICodeExtractorService
    {
        Task<string> ExtractCodeAsync(Stream stream, string fileName);
    }
}
