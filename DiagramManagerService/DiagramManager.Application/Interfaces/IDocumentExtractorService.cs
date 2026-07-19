using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DiagramManager.Application.DTOs;

namespace DiagramManager.Application.Interfaces
{
    public interface IDocumentExtractorService
    {
        Task<List<ExtractedDiagramDto>> ExtractDiagramsAsync(Stream fileStream, string fileName);
    }
}
