using DiagramManager.Domain.Entities;
using DiagramManager.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiagramManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagramSharesController : ControllerBase
    {
        private readonly WorkspaceDbContext _context;

        public DiagramSharesController(WorkspaceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShare([FromBody] DiagramShare shareRequest)
        {
            var diagram = await _context.Diagrams.FindAsync(shareRequest.DiagramId);
            if (diagram == null) return NotFound("Diagram not found");

            shareRequest.Id = Guid.NewGuid();
            // Default permission if missing
            if (string.IsNullOrEmpty(shareRequest.PermissionLevel))
            {
                shareRequest.PermissionLevel = "Read";
            }

            _context.DiagramShares.Add(shareRequest);
            await _context.SaveChangesAsync();

            return Ok(shareRequest);
        }

        [HttpGet("diagram/{diagramId}")]
        public async Task<IActionResult> GetSharesByDiagram(Guid diagramId)
        {
            var shares = await _context.DiagramShares
                                       .Where(s => s.DiagramId == diagramId)
                                       .ToListAsync();
            return Ok(shares);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RevokeShare(Guid id)
        {
            var share = await _context.DiagramShares.FindAsync(id);
            if (share == null) return NotFound("Share not found");

            _context.DiagramShares.Remove(share);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
