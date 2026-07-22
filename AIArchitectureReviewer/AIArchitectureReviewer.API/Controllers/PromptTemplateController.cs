using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.DTOs;

namespace AIArchitectureReviewer.API.Controllers
{
    [ApiController]
    [Route("api/prompts")]
    public class PromptTemplateController : ControllerBase
    {
        private readonly IPromptTemplateService _promptService;

        public PromptTemplateController(IPromptTemplateService promptService)
        {
            _promptService = promptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _promptService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var prompt = await _promptService.GetByIdAsync(id);
            if (prompt == null) return NotFound();
            return Ok(prompt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromptTemplateDto dto)
        {
            var result = await _promptService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePromptTemplateDto dto)
        {
            var success = await _promptService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { Message = "Cập nhật prompt thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _promptService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { Message = "Xóa prompt thành công." });
        }
    }
}
