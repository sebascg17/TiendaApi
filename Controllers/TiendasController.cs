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
                // Mapear dirección personal a descripción? O usar campos específicos si extendemos Tienda.
                // Por ahora mapeamos 'Direccion' del DTO a la Descripción o creamos un campo futuro.
                // Requisito del usuario: "Dirección del Local Físico". Tienda.cs no tiene 'Direccion', tiene 'Descripcion'.
                // Vamos a concatenar la dirección a la descripción temporalmente o asumir que el Usuario quiere el campo.
                // Mejor opción: Agregar 'Direccion' a la entidad Tienda más adelante. Por ahora lo guardo en Descripción.
                // UPDATE: Usuario pidió 'Direccion del Local Físico'.
                // Voy a agregar la nota en Descripción: "Direccion: [valor]"
                
                UsuarioId = usuarioId,
                Estado = "inactivo", // Por defecto
                FechaCreacion = DateTime.UtcNow,
                ColorPrimario = dto.ColorPrimario
            };

            // Concatenar dirección si existe
            if (!string.IsNullOrEmpty(dto.Direccion))
            {
                nuevaTienda.Descripcion = (nuevaTienda.Descripcion ?? "") + " | Dirección: " + dto.Direccion;
            }

            _context.Tiendas.Add(nuevaTienda);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tienda creada exitosamente. Estado: Inactivo (Pendiente de aprobación).", tiendaId = nuevaTienda.Id });
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
