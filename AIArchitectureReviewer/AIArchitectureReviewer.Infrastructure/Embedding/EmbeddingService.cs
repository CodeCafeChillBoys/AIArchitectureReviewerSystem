using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Search;
using Microsoft.Extensions.Configuration;

namespace AIArchitectureReviewer.Infrastructure.Embedding
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly Application.Interfaces.AI.IApiKeyProvider _apiKeyProvider;

        public EmbeddingService(HttpClient httpClient, Application.Interfaces.AI.IApiKeyProvider apiKeyProvider)
        {
            _httpClient = httpClient;
            _apiKeyProvider = apiKeyProvider;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<float>();
            }

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent?key={_apiKeyProvider.GetNextApiKey()}";
            
            var payload = new
            {
                model = "models/gemini-embedding-001",
                content = new
                {
                    parts = new[] { new { text = text } }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini API Error: {response.StatusCode} - {errorBody}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);
            
            // Expected response format: { "embedding": { "values": [0.1, 0.2, ...] } }
            var valuesElement = document.RootElement.GetProperty("embedding").GetProperty("values");
            
            return valuesElement.EnumerateArray().Select(e => e.GetSingle()).Take(768).ToArray();
        }
    }
}
