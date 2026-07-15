using Microsoft.AspNetCore.Http;

namespace AIArchitectureReviewer.API.DTOs
{
    public class UploadRequestDto
    {
        public IFormFile File { get; set; }
        public string? CustomPrompt { get; set; }
    }
}
