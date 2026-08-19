using System;
using System.Threading.Tasks;
using AIArchitectureReviewer.Domain.Entities;

namespace AIArchitectureReviewer.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<SystemRule> SystemRules { get; }
        IGenericRepository<RuleChunk> RuleChunks { get; }
        IGenericRepository<AnalysisReport> AnalysisReports { get; }
        IGenericRepository<ChatSession> ChatSessions { get; }
        IGenericRepository<ChatMessage> ChatMessages { get; }
        IGenericRepository<PromptTemplate> PromptTemplates { get; }
        Task<int> CompleteAsync();
    }
}
