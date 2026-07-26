using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;

namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface ISystemRuleService
    {
        Task<SystemRuleDto?> GetRuleByIdAsync(Guid id);
        Task<IEnumerable<SystemRuleDto>> GetAllRulesAsync();
        Task<SystemRuleDto> CreateRuleAsync(CreateSystemRuleDto dto);
        Task<ChangeSetDto?> UpdateRuleAsync(Guid id, UpdateSystemRuleDto dto);
        Task<IEnumerable<ChangeHistoryEntryDto>?> GetHistoryAsync(Guid id);
        Task DeleteRuleAsync(Guid id);
        Task ClearAllRulesAsync();
    }
}
