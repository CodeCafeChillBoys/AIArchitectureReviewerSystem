using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Application.Mappings;
using AIArchitectureReviewer.Domain.Diffing;
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

        public async Task<ChangeSetDto?> UpdateRuleAsync(Guid id, UpdateSystemRuleDto dto)
        {
            var existing = await _unitOfWork.SystemRules.GetByIdAsync(id);
            if (existing == null) return null;

            // Tầng 1: two-way diff giữa entity đang có (A) và dữ liệu gửi lên (B).
            var changeSet = new ChangeSetBuilder()
                .Scalar(nameof(SystemRule.DiagramType), existing.DiagramType, dto.DiagramType)
                .Scalar(nameof(SystemRule.RuleName), existing.RuleName, dto.RuleName)
                .Text(nameof(SystemRule.RegexOrCondition), existing.RegexOrCondition, dto.RegexOrCondition)
                .Scalar(nameof(SystemRule.IsActive),
                        existing.IsActive.ToString().ToLowerInvariant(),
                        dto.IsActive?.ToString().ToLowerInvariant())
                .Build();

            if (!changeSet.HasChanges)
            {
                return ChangeHistoryMapper.NoChanges(id, changeSet);
            }

            var now = DateTime.UtcNow;

            foreach (var change in changeSet.Changes)
            {
                await _unitOfWork.SystemRuleHistories.AddAsync(
                    new SystemRuleHistory { SystemRuleId = id }
                        .FillFrom(change, changeSet.Id, now));
            }

            if (changeSet.Contains(nameof(SystemRule.DiagramType))) existing.DiagramType = dto.DiagramType!;
            if (changeSet.Contains(nameof(SystemRule.RuleName))) existing.RuleName = dto.RuleName!;
            if (changeSet.Contains(nameof(SystemRule.IsActive))) existing.IsActive = dto.IsActive!.Value;

            // Chỉ dựng lại chunk + embedding khi nội dung rule thật sự đổi.
            // Đổi mỗi IsActive hay RuleName thì không gọi embedding lần nào.
            if (changeSet.Contains(nameof(SystemRule.RegexOrCondition)))
            {
                existing.RegexOrCondition = dto.RegexOrCondition!;

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

            // Một CompleteAsync duy nhất: embedding lỗi giữa chừng thì không có chunk mồ côi.
            await _unitOfWork.CompleteAsync();

            return ChangeHistoryMapper.ToDto(id, changeSet);
        }

        public async Task<IEnumerable<ChangeHistoryEntryDto>?> GetHistoryAsync(Guid id)
        {
            var existing = await _unitOfWork.SystemRules.GetByIdAsync(id);
            if (existing == null) return null;

            var rows = await _unitOfWork.SystemRuleHistories.FindAsync(h => h.SystemRuleId == id);
            return ChangeHistoryMapper.ToHistory(rows);
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
