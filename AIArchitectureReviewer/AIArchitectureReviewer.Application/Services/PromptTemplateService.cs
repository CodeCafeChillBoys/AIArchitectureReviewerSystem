using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.Mappings;
using AIArchitectureReviewer.Domain.Diffing;
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
        public async Task<ChangeSetDto?> UpdateAsync(Guid id, UpdatePromptTemplateDto dto)
        {
            var p = await _unitOfWork.PromptTemplates.GetByIdAsync(id);
            if (p == null) return null;

            // Tầng 1: two-way diff giữa entity đang có (A) và dữ liệu gửi lên (B).
            var changeSet = new ChangeSetBuilder()
                .Scalar(nameof(PromptTemplate.Name), p.Name, dto.Name)
                .Text(nameof(PromptTemplate.Content), p.Content, dto.Content)
                .Scalar(nameof(PromptTemplate.DiagramType), p.DiagramType, dto.DiagramType)
                .Build();

            // Không có gì đổi thì không ghi gì cả, kể cả UpdatedAt.
            if (!changeSet.HasChanges)
            {
                return ChangeHistoryMapper.NoChanges(id, changeSet);
            }

            var now = DateTime.UtcNow;

            foreach (var change in changeSet.Changes)
            {
                await _unitOfWork.PromptTemplateHistories.AddAsync(
                    new PromptTemplateHistory { PromptTemplateId = id }
                        .FillFrom(change, changeSet.Id, now));
            }

            // Chỉ gán những field diff xác nhận là đã đổi.
            if (changeSet.Contains(nameof(PromptTemplate.Name))) p.Name = dto.Name!;
            if (changeSet.Contains(nameof(PromptTemplate.Content))) p.Content = dto.Content!;
            if (changeSet.Contains(nameof(PromptTemplate.DiagramType))) p.DiagramType = dto.DiagramType!;
            p.UpdatedAt = now;

            _unitOfWork.PromptTemplates.Update(p);
            await _unitOfWork.CompleteAsync();

            return ChangeHistoryMapper.ToDto(id, changeSet);
        }

        public async Task<IEnumerable<ChangeHistoryEntryDto>?> GetHistoryAsync(Guid id)
        {
            var p = await _unitOfWork.PromptTemplates.GetByIdAsync(id);
            if (p == null) return null;

            var rows = await _unitOfWork.PromptTemplateHistories.FindAsync(h => h.PromptTemplateId == id);
            return ChangeHistoryMapper.ToHistory(rows);
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
