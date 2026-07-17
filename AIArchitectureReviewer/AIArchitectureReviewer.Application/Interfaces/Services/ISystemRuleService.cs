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
        Task UpdateRuleAsync(Guid id, UpdateSystemRuleDto dto);
        Task DeleteRuleAsync(Guid id);
        Task ClearAllRulesAsync();
    }
}
