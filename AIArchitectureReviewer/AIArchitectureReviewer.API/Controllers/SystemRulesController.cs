using System;
using System.IO;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIArchitectureReviewer.API.Controllers
{
    [ApiController]
    [Route("api/system-rules")]
    public class SystemRulesController : ControllerBase
    {
        private readonly ISystemRuleService _systemRuleService;

        public SystemRulesController(ISystemRuleService systemRuleService)
        {
            _systemRuleService = systemRuleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _systemRuleService.GetAllRulesAsync();
            return Ok(rules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var rule = await _systemRuleService.GetRuleByIdAsync(id);
            if (rule == null) return NotFound();
            return Ok(rule);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSystemRuleDto dto)
        {
            var created = await _systemRuleService.CreateRuleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadRuleFile(IFormFile file, [FromForm] string diagramType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            var dto = new CreateSystemRuleDto
            {
                DiagramType = diagramType ?? "Unknown",
                RuleName = Path.GetFileNameWithoutExtension(file.FileName),
                RegexOrCondition = fileContent,
                IsActive = true
            };

            var created = await _systemRuleService.CreateRuleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("seed-from-directory")]
        public async Task<IActionResult> SeedFromDirectory([FromQuery] bool clearExisting = false)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "RAG_Documents");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                return Ok(new { Message = "Created RAG_Documents folder. Please add files and run again." });
            }

            if (clearExisting)
            {
                await _systemRuleService.ClearAllRulesAsync();
            }

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            int count = 0;
            int skipped = 0;

            var existingRules = await _systemRuleService.GetAllRulesAsync();
            var existingNames = new System.Collections.Generic.HashSet<string>();
            foreach(var r in existingRules) { existingNames.Add(r.RuleName); }

            foreach (var file in files)
            {
                var ruleName = Path.GetFileNameWithoutExtension(file);
                
                if (existingNames.Contains(ruleName))
                {
                    skipped++;
                    continue;
                }

                var content = await System.IO.File.ReadAllTextAsync(file);

                var dto = new CreateSystemRuleDto
                {
                    DiagramType = "Global",
                    RuleName = ruleName,
                    RegexOrCondition = content,
                    IsActive = true
                };

                await _systemRuleService.CreateRuleAsync(dto);
                count++;
            }

            return Ok(new { Message = $"Successfully imported {count} documents as rules. Skipped {skipped} existing documents." });
        }

        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAll()
        {
            try
            {
                await _systemRuleService.ClearAllRulesAsync();
                return Ok(new { Message = "Successfully cleared all existing rules and chunks from DB." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while clearing rules: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateSystemRuleDto dto)
        {
            try
            {
                await _systemRuleService.UpdateRuleAsync(id, dto);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _systemRuleService.DeleteRuleAsync(id);
            return NoContent();
        }
    }
}
