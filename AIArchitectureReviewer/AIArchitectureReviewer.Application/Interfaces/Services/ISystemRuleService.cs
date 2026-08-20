using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Requests;
using AIArchitectureReviewer.Application.DTOs.Responses;

namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface ISystemRuleService
    {
        Task<SystemRuleDto?> GetRuleByIdAsync(Guid id);
        Task<IEnumerable<SystemRuleDto>> GetAllRulesAsync();
        Task<SystemRuleDto> CreateRuleAsync(CreateSystemRuleDto dto);
        Task<SystemRuleDto?> UpdateRuleAsync(Guid id, UpdateSystemRuleDto dto);
        Task DeleteRuleAsync(Guid id);
        Task ClearAllRulesAsync();
    }
}
