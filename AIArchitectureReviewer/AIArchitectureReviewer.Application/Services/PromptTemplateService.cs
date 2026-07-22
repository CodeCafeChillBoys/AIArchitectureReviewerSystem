using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIArchitectureReviewer.Application.Services
{
    public class PromptTemplateService : IPromptTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PromptTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PromptTemplateDto>> GetAllAsync()
        {
            var list = await _unitOfWork.PromptTemplates.GetAllAsync();
            return list.Select(p => new PromptTemplateDto
            {
                Id = p.Id,
                Name = p.Name,
                Content = p.Content,
                DiagramType = p.DiagramType,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }
        public async Task<PromptTemplateDto> GetByIdAsync(Guid id)
        {
            var p = await _unitOfWork.PromptTemplates.GetByIdAsync(id);
            if (p == null) return null!;
            return new PromptTemplateDto
            {
                Id = p.Id,
                Name = p.Name,
                Content = p.Content,
                DiagramType = p.DiagramType,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
        public async Task<PromptTemplateDto> CreateAsync(CreatePromptTemplateDto dto)
        {
            var p = new PromptTemplate
            {
                Name = dto.Name,
                Content = dto.Content,
                DiagramType = dto.DiagramType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.PromptTemplates.AddAsync(p);
            await _unitOfWork.CompleteAsync();
            return new PromptTemplateDto
            {
                Id = p.Id,
                Name = p.Name,
                Content = p.Content,
                DiagramType = p.DiagramType,
                CreatedAt = p.CreatedAt
            };
        }
        public async Task<bool> UpdateAsync(Guid id, UpdatePromptTemplateDto dto)
        {
            var p = await _unitOfWork.PromptTemplates.GetByIdAsync(id);
            if (p == null) return false;
            p.Name = dto.Name;
            p.Content = dto.Content;
            p.DiagramType = dto.DiagramType;
            p.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.PromptTemplates.Update(p);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var p = await _unitOfWork.PromptTemplates.GetByIdAsync(id);
            if (p == null) return false;
            _unitOfWork.PromptTemplates.Remove(p);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
