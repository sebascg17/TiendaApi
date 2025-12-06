using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using TiendaApi.DTOs.Transacciones;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TransaccionesController(AppDbContext db)
        {
            _db = db;
        }

        // POST: api/transacciones
        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreateDto dto)
        {
            var transaccion = new Transaccion
            {
                UsuarioId = dto.UsuarioId,
                Tipo = dto.Tipo,
                Monto = dto.Monto,
                Referencia = dto.Referencia,
                Estado = "Pendiente", // Por defecto
                FechaCreacion = DateTime.UtcNow
            };

            // Validaciones básicas
            if (dto.Tipo == "Retiro")
            {
                // Verificar saldo (simplificado: suma de todas las transacciones completadas)
                var saldo = await _db.Transacciones
                    .Where(t => t.UsuarioId == dto.UsuarioId && t.Estado == "Completada")
                    .SumAsync(t => t.Tipo == "Recarga" || t.Tipo == "Venta" ? t.Monto : -t.Monto);

                if (saldo < dto.Monto)
                {
                    return BadRequest("Saldo insuficiente para el retiro.");
                }
            }

            _db.Transacciones.Add(transaccion);
            await _db.SaveChangesAsync();

            return Ok(transaccion);
        }

        // GET: api/transacciones/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetPorUsuario(int usuarioId)
        {
            var transacciones = await _db.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return Ok(transacciones);
        }

        // PUT: api/transacciones/{id}/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] string nuevoEstado)
        {
            var transaccion = await _db.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();

            transaccion.Estado = nuevoEstado;
            await _db.SaveChangesAsync();

            return Ok(transaccion);
        }
        
        // GET: api/transacciones/saldo/{usuarioId}
        [HttpGet("saldo/{usuarioId}")]
        public async Task<IActionResult> GetSaldo(int usuarioId)
        {
             var saldo = await _db.Transacciones
                    .Where(t => t.UsuarioId == usuarioId && t.Estado == "Completada")
                    .SumAsync(t => t.Tipo == "Recarga" || t.Tipo == "Venta" ? t.Monto : -t.Monto);
            
            return Ok(new { UsuarioId = usuarioId, Saldo = saldo });
        }
    }
}
