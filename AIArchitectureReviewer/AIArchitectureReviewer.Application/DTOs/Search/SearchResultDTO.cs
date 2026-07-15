namespace AIArchitectureReviewer.Application.DTOs.Search
{
    public class SearchResultDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public double Score { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
