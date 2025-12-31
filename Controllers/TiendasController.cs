using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaApi.DTOs.Tiendas;
using TiendaApi.Models;
using TiendaApi.Infrastructure;

namespace TiendaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere estar logueado
    public class TiendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TiendasController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/tiendas (Crear nueva tienda)
        [HttpPost]
        [Authorize(Roles = "Tendero")] // Solo tenderos pueden crear tiendas
        public async Task<IActionResult> CrearTienda([FromBody] TiendaCreateDto dto)
        {
            // Obtener ID del usuario del token
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar al usuario.");

            int usuarioId = int.Parse(userIdClaim);

            var nuevaTienda = new Tienda
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Direccion = dto.Direccion,
                Pais = dto.Pais,
                Departamento = dto.Departamento,
                Ciudad = dto.Ciudad,
                UsuarioId = usuarioId,
                Estado = "inactivo", 
                FechaCreacion = DateTime.UtcNow,
                ColorPrimario = dto.ColorPrimario
            };

            _context.Tiendas.Add(nuevaTienda);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tienda creada exitosamente.", tiendaId = nuevaTienda.Id });
        }

        // GET: api/tiendas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTienda(int id)
        {
            var tienda = await _context.Tiendas
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tienda == null) return NotFound();

            // Seguridad básica: Solo el dueño o un admin puede ver detalles privados? 
            // Para "Configuración", sí. Para vista pública no. 
            // Por ahora permitimos si es el dueño.
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != tienda.UsuarioId)
            {
                if (!User.IsInRole("Admin"))
                    return Forbid();
            }

            return Ok(tienda);
        }

        // PUT: api/tiendas/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Tendero,Admin")]
        public async Task<IActionResult> UpdateTienda(int id, [FromBody] TiendaUpdateDto dto)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null) return NotFound();

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != tienda.UsuarioId)
            {
                if (!User.IsInRole("Admin"))
                    return Forbid();
            }

            tienda.Nombre = dto.Nombre;
            tienda.Descripcion = dto.Descripcion;
            tienda.Direccion = dto.Direccion;
            tienda.Pais = dto.Pais;
            tienda.Departamento = dto.Departamento;
            tienda.Ciudad = dto.Ciudad;
            tienda.ColorPrimario = dto.ColorPrimario;
            
            if (!string.IsNullOrEmpty(dto.LogoUrl))
                tienda.LogoUrl = dto.LogoUrl;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Tienda actualizada correctamente." });
        }

        // DELETE: api/tiendas/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Tendero,Admin")]
        public async Task<IActionResult> DeleteTienda(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda == null) return NotFound();

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != tienda.UsuarioId)
            {
                if (!User.IsInRole("Admin"))
                    return Forbid();
            }

            _context.Tiendas.Remove(tienda);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tienda eliminada correctamente." });
        }

        // GET: api/tiendas/mis-tiendas (Listar tiendas del usuario logueado)
        [HttpGet("mis-tiendas")]
        [Authorize(Roles = "Tendero")]
        public async Task<IActionResult> GetMisTiendas()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            var tiendas = await _context.Tiendas
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => new
                {
                    t.Id,
                    t.Nombre,
                    t.Descripcion,
                    t.Estado,
                    t.LogoUrl
                })
                .ToListAsync();

            return Ok(tiendas);
        }
    }
}
