using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;
namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface IPromptTemplateService
    {
        Task<IEnumerable<PromptTemplateDto>> GetAllAsync();
        Task<PromptTemplateDto> GetByIdAsync(Guid id);
        Task<PromptTemplateDto> CreateAsync(CreatePromptTemplateDto dto);
        Task<ChangeSetDto?> UpdateAsync(Guid id, UpdatePromptTemplateDto dto);
        Task<IEnumerable<ChangeHistoryEntryDto>?> GetHistoryAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
