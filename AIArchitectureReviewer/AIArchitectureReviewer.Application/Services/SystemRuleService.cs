using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Domain.Entities;

namespace AIArchitectureReviewer.Application.Services
{
    public class SystemRuleService : ISystemRuleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmbeddingService _embeddingService;

        public SystemRuleService(IUnitOfWork unitOfWork, IEmbeddingService embeddingService)
        {
            _unitOfWork = unitOfWork;
            _embeddingService = embeddingService;
        }

        public async Task<IEnumerable<SystemRuleDto>> GetAllRulesAsync()
        {
            var rules = await _unitOfWork.SystemRules.GetAllAsync();
            return rules.Select(r => new SystemRuleDto
            {
                Id = r.Id,
                DiagramType = r.DiagramType,
                RuleName = r.RuleName,
                RegexOrCondition = r.RegexOrCondition,
                IsActive = r.IsActive
            });
        }

        public async Task<SystemRuleDto?> GetRuleByIdAsync(Guid id)
        {
            var rule = await _unitOfWork.SystemRules.GetByIdAsync(id);
            if (rule == null) return null;

            return new SystemRuleDto
            {
                Id = rule.Id,
                DiagramType = rule.DiagramType,
                RuleName = rule.RuleName,
                RegexOrCondition = rule.RegexOrCondition,
                IsActive = rule.IsActive
            };
        }

        public async Task<SystemRuleDto> CreateRuleAsync(CreateSystemRuleDto dto)
        {
            var entity = new SystemRule
            {
                Id = Guid.NewGuid(),
                DiagramType = dto.DiagramType,
                RuleName = dto.RuleName,
                RegexOrCondition = dto.RegexOrCondition,
                IsActive = dto.IsActive
            };

            await _unitOfWork.SystemRules.AddAsync(entity);
            
            // Simple Chunking & Embedding
            var chunkSize = 1000;
            var text = dto.RegexOrCondition;
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                var chunkText = text.Substring(i, Math.Min(chunkSize, text.Length - i));
                var vector = await _embeddingService.GenerateEmbeddingAsync(chunkText);
                
                var chunk = new RuleChunk
                {
                    Id = Guid.NewGuid(),
                    SystemRuleId = entity.Id,
                    Content = chunkText,
                    Embedding = new Pgvector.Vector(vector)
                };
                await _unitOfWork.RuleChunks.AddAsync(chunk);
            }

            await _unitOfWork.CompleteAsync();

            return new SystemRuleDto
            {
                Id = entity.Id,
                DiagramType = entity.DiagramType,
                RuleName = entity.RuleName,
                RegexOrCondition = entity.RegexOrCondition,
                IsActive = entity.IsActive
            };
        }

        public async Task<SystemRuleDto?> UpdateRuleAsync(Guid id, UpdateSystemRuleDto dto)
        {
            var existing = await _unitOfWork.SystemRules.GetByIdAsync(id);
            if (existing == null) return null;

            if (dto.DiagramType != null) existing.DiagramType = dto.DiagramType;
            if (dto.RuleName != null) existing.RuleName = dto.RuleName;
            if (dto.IsActive.HasValue) existing.IsActive = dto.IsActive.Value;

            if (!string.IsNullOrEmpty(dto.RegexOrCondition) && dto.RegexOrCondition != existing.RegexOrCondition)
            {
                existing.RegexOrCondition = dto.RegexOrCondition;

                var oldChunks = await _unitOfWork.RuleChunks.FindAsync(c => c.SystemRuleId == id);
                _unitOfWork.RuleChunks.RemoveRange(oldChunks);

                var chunkSize = 1000;
                var text = existing.RegexOrCondition;
                for (int i = 0; i < text.Length; i += chunkSize)
                {
                    var chunkText = text.Substring(i, Math.Min(chunkSize, text.Length - i));
                    var vector = await _embeddingService.GenerateEmbeddingAsync(chunkText);

                    await _unitOfWork.RuleChunks.AddAsync(new RuleChunk
                    {
                        Id = Guid.NewGuid(),
                        SystemRuleId = id,
                        Content = chunkText,
                        Embedding = new Pgvector.Vector(vector)
                    });
                }
            }

            _unitOfWork.SystemRules.Update(existing);
            await _unitOfWork.CompleteAsync();

            return new SystemRuleDto
            {
                Id = existing.Id,
                DiagramType = existing.DiagramType,
                RuleName = existing.RuleName,
                RegexOrCondition = existing.RegexOrCondition,
                IsActive = existing.IsActive
            };
        }

        public async Task DeleteRuleAsync(Guid id)
        {
            var existing = await _unitOfWork.SystemRules.GetByIdAsync(id);
            if (existing != null)
            {
                _unitOfWork.SystemRules.Remove(existing);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task ClearAllRulesAsync()
        {
            var chunks = await _unitOfWork.RuleChunks.GetAllAsync();
            _unitOfWork.RuleChunks.RemoveRange(chunks);

            var rules = await _unitOfWork.SystemRules.GetAllAsync();
            _unitOfWork.SystemRules.RemoveRange(rules);

            await _unitOfWork.CompleteAsync();
        }
    }
}
