using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using TiendaApi.DTOs.Tiendas;
using Microsoft.AspNetCore.Authorization;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoTiendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TipoTiendasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoTiendaDto>>> GetTipos()
        {
            return await _context.TipoTiendas
                .Where(t => t.Activo)
                .Select(t => new TipoTiendaDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Icono = t.Icono,
                    ImagenUrl = t.ImagenUrl,
                    Activo = t.Activo
                })
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TipoTiendaDto>> Create([FromBody] TipoTiendaCreateDto dto)
        {
            var tipo = new TipoTienda
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Icono = dto.Icono,
                ImagenUrl = dto.ImagenUrl,
                Activo = true
            };

            _context.TipoTiendas.Add(tipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTipos), new { id = tipo.Id }, new TipoTiendaDto
            {
                Id = tipo.Id,
                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion,
                Icono = tipo.Icono,
                ImagenUrl = tipo.ImagenUrl,
                Activo = tipo.Activo
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoTiendaCreateDto dto)
        {
            var tipo = await _context.TipoTiendas.FindAsync(id);
            if (tipo == null) return NotFound();

            tipo.Nombre = dto.Nombre;
            tipo.Descripcion = dto.Descripcion;
            tipo.Icono = dto.Icono;
            tipo.ImagenUrl = dto.ImagenUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _context.TipoTiendas.FindAsync(id);
            if (tipo == null) return NotFound();

            tipo.Activo = false; // Soft delete
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
