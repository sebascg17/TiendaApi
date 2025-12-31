using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DireccionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DireccionesController(AppDbContext context)
        {
            _context = context;
        }

        // Helper para obtener ID usuario
        private int GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (idClaim != null && int.TryParse(idClaim.Value, out var id)) return id;
            return 0;
        }

        // GET: api/direcciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DireccionUsuario>>> GetMisDirecciones()
        {
            var userId = GetUserId();
            return await _context.DireccionesUsuarios
                .Where(d => d.UsuarioId == userId)
                .ToListAsync();
        }

        // POST: api/direcciones
        [HttpPost]
        public async Task<ActionResult<DireccionUsuario>> CrearDireccion(DireccionUsuario direccion)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            direccion.UsuarioId = userId;
            
            // Si es la primera, hacerla principal
            var count = await _context.DireccionesUsuarios.CountAsync(d => d.UsuarioId == userId);
            if (count == 0) direccion.EsPrincipal = true;

            // Si esta se marca como principal, desmarcar las otras
            if (direccion.EsPrincipal)
            {
                var otras = await _context.DireccionesUsuarios.Where(d => d.UsuarioId == userId).ToListAsync();
                otras.ForEach(d => d.EsPrincipal = false);
            }

            _context.DireccionesUsuarios.Add(direccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMisDirecciones), new { id = direccion.Id }, direccion);
        }

        // DELETE: api/direcciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDireccion(int id)
        {
            var userId = GetUserId();
            var direccion = await _context.DireccionesUsuarios.FindAsync(id);

            if (direccion == null) return NotFound();
            if (direccion.UsuarioId != userId) return Forbid();

            _context.DireccionesUsuarios.Remove(direccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
