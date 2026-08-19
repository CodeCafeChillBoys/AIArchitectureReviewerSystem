using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Requests;
using AIArchitectureReviewer.Application.DTOs.Responses;
namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface IPromptTemplateService
    {
        Task<IEnumerable<PromptTemplateDto>> GetAllAsync();
        Task<PromptTemplateDto> GetByIdAsync(Guid id);
        Task<PromptTemplateDto> CreateAsync(CreatePromptTemplateDto dto);
        Task<PromptTemplateDto?> UpdateAsync(Guid id, UpdatePromptTemplateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
