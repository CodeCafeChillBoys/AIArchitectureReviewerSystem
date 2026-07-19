using System.Text;
using System.Text.Json;
using AIArchitectureReviewer.Application.Interfaces.RAG;
using AIArchitectureReviewer.Application.Interfaces.Search;
using Microsoft.Extensions.Configuration;

namespace AIArchitectureReviewer.Infrastructure.RAG
{
    public class RAGService : IRAGService
    {
        private readonly IHybridSearchService _hybridSearchService;
        private readonly HttpClient _httpClient;
        private readonly Application.Interfaces.AI.IApiKeyProvider _apiKeyProvider;
        private readonly string _model;

        public RAGService(IHybridSearchService hybridSearchService, HttpClient httpClient, IConfiguration configuration, Application.Interfaces.AI.IApiKeyProvider apiKeyProvider)
        {
            _hybridSearchService = hybridSearchService;
            _httpClient = httpClient;
            _apiKeyProvider = apiKeyProvider;
            _model = configuration["Gemini:Model"] ?? "gemini-2.5-pro";
        }

        //         public async Task<string> AnswerQuestionAsync(string question, int contextTopK = 5)
        //         {
        //             // 1. Retrieve context using Hybrid Search
        //             var searchResults = await _hybridSearchService.SearchHybridAsync(question, contextTopK);

        //             var contextBuilder = new StringBuilder();
        //             foreach (var result in searchResults)
        //             {
        //                 contextBuilder.AppendLine($"- {result.Content}");
        //             }

        //             var contextString = contextBuilder.ToString();

        //             // 2. Build the System Prompt and request payload
        //             var systemPrompt = @"Bạn là một chuyên gia phân tích kiến trúc phần mềm và trợ lý AI.
        // CHỈ trả lời dựa trên thông tin trong thẻ <context> được cung cấp dưới đây.
        // Tuyệt đối KHÔNG sử dụng kiến thức bên ngoài. Nếu thông tin không có trong <context>, hãy từ chối trả lời và nói rằng câu hỏi nằm ngoài phạm vi dữ liệu.";

        //             var fullPrompt = $"{systemPrompt}\n\n<context>\n{contextString}\n</context>\n\nCâu hỏi: {question}";

        //             var payload = new
        //             {
        //                 contents = new[]
        //                 {
        //                     new
        //                     {
        //                         parts = new[] { new { text = fullPrompt } }
        //                     }
        //                 }
        //             };

        //             var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKeyProvider.GetNextApiKey()}";
        //             var jsonPayload = JsonSerializer.Serialize(payload);
        //             var response = await PostWithRetryAsync(url, jsonPayload);

        //             var responseString = await response.Content.ReadAsStringAsync();
        //             using var document = JsonDocument.Parse(responseString);

        //             // Expected response format: { "candidates": [ { "content": { "parts": [ { "text": "..." } ] } } ] }
        //             var textElement = document.RootElement
        //                 .GetProperty("candidates")[0]
        //                 .GetProperty("content")
        //                 .GetProperty("parts")[0]
        //                 .GetProperty("text");

        //             return textElement.GetString() ?? string.Empty;
        //         }

        public async Task<string> GetRawContextAsync(string question, int contextTopK = 5)
        {
            // HybridSearch se lấy kết quả của kỹ thuật srarch keyWord Search và vercotor chỉ lấy 5 ngữ cảnh gần lấy
            var searchResults = await _hybridSearchService.SearchHybridAsync(question, contextTopK);
            // Khỏi tạo ra một string builder
            // Khi tìm kiếm ra kiến qua tiến thành Add vào stringBuilder
            var contextBuilder = new StringBuilder();
            foreach (var result in searchResults)
            {
                contextBuilder.AppendLine($"- {result.Content}");
            }

            return contextBuilder.ToString();
        }

        public async Task<string> GenerateContentAsync(string systemPrompt, string userPrompt)
        {
            // Lấy systemPromt đc cấu hình cho AI 
            // Lấy userPromt người dùng gõ hoặc nhập
            var fullPrompt = $"{systemPrompt}\n\n{userPrompt}";
            // khỏi tạo payload chứa fullPrompt
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = fullPrompt } }
                    }
                }
            };
            // gọi API key của model gemini 
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKeyProvider.GetNextApiKey()}";
            // parse cái payload sang json
            var jsonPayload = JsonSerializer.Serialize(payload);
            // retry 3 lần nếu ko crash server
            var response = await PostWithRetryAsync(url, jsonPayload);
            // đọc nội dụng 
            var responseString = await response.Content.ReadAsStringAsync();
            // chuyển sang data
            using var document = JsonDocument.Parse(responseString);

            // lấy từng data ra
            var textElement = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text");

            return textElement.GetString() ?? string.Empty;
        }

        public async Task<string> GenerateContentWithImageAsync(string systemPrompt, string userPrompt, byte[] imageBytes, string mimeType)
        {
            var fullPrompt = $"{systemPrompt}\n\n{userPrompt}";
            var base64Image = Convert.ToBase64String(imageBytes);

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = fullPrompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = mimeType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKeyProvider.GetNextApiKey()}";
            var jsonPayload = JsonSerializer.Serialize(payload);
            var response = await PostWithRetryAsync(url, jsonPayload);

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);

            var textElement = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text");

            return textElement.GetString() ?? string.Empty;
        }
        public async Task<string> ChatWithHistoryAsync(string systemPrompt, System.Collections.Generic.IEnumerable<Domain.Entities.ChatMessage> history, string newQuestion)
        {
            // 1. Retrieve context using Hybrid Search
            var searchResults = await _hybridSearchService.SearchHybridAsync(newQuestion, 5);
            var contextBuilder = new StringBuilder();
            foreach (var result in searchResults)
            {
                contextBuilder.AppendLine($"- {result.Content}");
            }

            // lâý lên những nội dung seach đc lấy ra bên trong string Builder
            var contextString = contextBuilder.ToString();

            // 2. Build the message content list
            var contents = new System.Collections.Generic.List<object>();

            // System prompt as the first message
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                contents.Add(new
                {
                    role = "user",
                    parts = new[] { new { text = systemPrompt } }
                });
                contents.Add(new
                {
                    role = "model",
                    parts = new[] { new { text = "Đã rõ, tôi sẽ đóng vai trò như bạn yêu cầu." } }
                });
            }

            // Append history
            foreach (var msg in history)
            {
                contents.Add(new
                {
                    role = msg.Role == "user" ? "user" : "model",
                    parts = new[] { new { text = msg.Content } }
                });
            }

            // Append new question with retrieved context (RAG)
            string augmentedQuestion = newQuestion;
            if (!string.IsNullOrEmpty(contextString))
            {
                augmentedQuestion = $"Hãy sử dụng thông tin trong thẻ <context> dưới đây (nếu có liên quan) để trả lời câu hỏi.\n\n<context>\n{contextString}</context>\n\nCâu hỏi: {newQuestion}";
            }

            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = augmentedQuestion } }
            });

            var payload = new { contents = contents };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKeyProvider.GetNextApiKey()}";
            var jsonPayload = JsonSerializer.Serialize(payload);
            var response = await PostWithRetryAsync(url, jsonPayload);

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);

            var textElement = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text");

            return textElement.GetString() ?? string.Empty;
        }

        private async Task<HttpResponseMessage> PostWithRetryAsync(string url, string jsonPayload, int maxRetries = 5)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                    response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                    response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                    response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    if (i == maxRetries - 1)
                    {
                        var errBody = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Gemini API Error: {response.StatusCode} - {errBody}");
                    }
                    // Exponential backoff: 2s, 4s, 8s, 16s
                    await Task.Delay((int)Math.Pow(2, i + 1) * 1000);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Gemini API Error: {response.StatusCode} - {errBody}");
                }
                return response;
            }
            throw new Exception("Unexpected end of retry loop");
        }
    }
}
