using DiagramManager.Domain.Entities;
using DiagramManager.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DiagramManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkspacesController : ControllerBase
    {
        private readonly WorkspaceDbContext _context;

        public WorkspacesController(WorkspaceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspace([FromBody] Workspace workspace)
        {
            workspace.Id = Guid.NewGuid();
            workspace.CreatedAt = DateTime.UtcNow;

            _context.Workspaces.Add(workspace);
            await _context.SaveChangesAsync();

            return Ok(workspace);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkspaces()
        {
            var workspaces = await _context.Workspaces.Include(w => w.Diagrams).ToListAsync();
            return Ok(workspaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkspace(Guid id)
        {
            var workspace = await _context.Workspaces.Include(w => w.Diagrams).FirstOrDefaultAsync(w => w.Id == id);
            if (workspace == null) return NotFound();
            return Ok(workspace);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkspace(Guid id, [FromBody] Workspace updatedWorkspace)
        {
            var workspace = await _context.Workspaces.FindAsync(id);
            if (workspace == null) return NotFound();

            workspace.Name = updatedWorkspace.Name;
            // Cập nhật các trường khác nếu cần

            await _context.SaveChangesAsync();
            return Ok(workspace);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkspace(Guid id)
        {
            var workspace = await _context.Workspaces.Include(w => w.Diagrams).ThenInclude(d => d.Versions).FirstOrDefaultAsync(w => w.Id == id);
            if (workspace == null) return NotFound();

            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
