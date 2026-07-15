using System;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Domain.Entities;
using AIArchitectureReviewer.Infrastructure.Data;

namespace AIArchitectureReviewer.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<SystemRule> SystemRules { get; private set; }
        public IGenericRepository<RuleChunk> RuleChunks { get; private set; }
        public IGenericRepository<AnalysisReport> AnalysisReports { get; private set; }
        public IGenericRepository<ChatSession> ChatSessions { get; private set; }
        public IGenericRepository<ChatMessage> ChatMessages { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            SystemRules = new GenericRepository<SystemRule>(_context);
            RuleChunks = new GenericRepository<RuleChunk>(_context);
            AnalysisReports = new GenericRepository<AnalysisReport>(_context);
            ChatSessions = new GenericRepository<ChatSession>(_context);
            ChatMessages = new GenericRepository<ChatMessage>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
