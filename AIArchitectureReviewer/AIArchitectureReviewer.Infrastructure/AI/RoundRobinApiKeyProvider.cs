using System;
using Microsoft.Extensions.Configuration;

namespace AIArchitectureReviewer.Infrastructure.AI
{
    public class RoundRobinApiKeyProvider : Application.Interfaces.AI.IApiKeyProvider
    {
        private readonly string[] _apiKeys;
        private int _currentIndex = 0;
        private readonly object _lock = new object();

        public RoundRobinApiKeyProvider(IConfiguration configuration)
        {
            _apiKeys = configuration.GetSection("Gemini:ApiKeys").Get<string[]>();
            
            if (_apiKeys == null || _apiKeys.Length == 0)
            {
                throw new Exception("No API keys found in configuration under Gemini:ApiKeys");
            }
        }

        public string GetNextApiKey()
        {
            lock (_lock)
            {
                string key = _apiKeys[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _apiKeys.Length;
                return key;
            }
        }
    }
}
