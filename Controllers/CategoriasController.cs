using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.DTOs.Categorias;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/categorias
        [HttpGet]
        public async Task<IActionResult> GetCategorias([FromQuery] int? tiendaId)
        {
            var query = _context.Categorias.AsQueryable();

            if (tiendaId.HasValue)
            {
                query = query.Where(c => c.TiendaId == null || c.TiendaId == tiendaId);
            }
            else
            {
                query = query.Where(c => c.TiendaId == null); // Solo globales por defecto
            }

            var categorias = await query.ToListAsync();
            return Ok(categorias);
        }

        // ✅ GET: api/categorias/mis-categorias
        [HttpGet("mis-categorias")]
        [Authorize(Roles = "Tendero")]
        public async Task<IActionResult> GetMisCategorias()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int usuarioId = int.Parse(userIdClaim);

            // Buscar tiendas del usuario
            var tiendasIds = await _context.Tiendas
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => t.Id)
                .ToListAsync();

            var categorias = await _context.Categorias
                .Where(c => c.TiendaId == null || (c.TiendaId != null && tiendasIds.Contains(c.TiendaId.Value)))
                .ToListAsync();

            return Ok(categorias);
        }

        // ✅ GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            return Ok(categoria);
        }

        // ✅ POST: api/categorias
        [HttpPost]
        [Authorize(Roles = "Admin,Tendero", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaCreateDto dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                TiendaId = dto.TiendaId,
                Slug = dto.Nombre.ToLower().Replace(" ", "-") // Slug básico
            };

            // Si es tendero y no se mandó TiendaId, auto-asignar la suya
            if (User.IsInRole("Tendero") && !User.IsInRole("Admin") && !categoria.TiendaId.HasValue)
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                var tienda = await _context.Tiendas.FirstOrDefaultAsync(t => t.UsuarioId == int.Parse(userIdClaim!));
                if (tienda == null) return BadRequest("Debe crear una tienda primero.");
                categoria.TiendaId = tienda.Id;
            }

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.Id }, categoria);
        }

        // ✅ PUT: api/categorias/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tendero", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaUpdateDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            if (User.IsInRole("Tendero"))
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                var usuarioId = int.Parse(userIdClaim!);
                var tienda = await _context.Tiendas.FindAsync(categoria.TiendaId);
                if (tienda == null || tienda.UsuarioId != usuarioId)
                    return Forbid();
            }

            categoria.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ DELETE: api/categorias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tendero", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            if (User.IsInRole("Tendero"))
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                var usuarioId = int.Parse(userIdClaim!);
                var tienda = await _context.Tiendas.FindAsync(categoria.TiendaId);
                if (tienda == null || tienda.UsuarioId != usuarioId)
                    return Forbid();
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
