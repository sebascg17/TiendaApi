using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using TiendaApi.DTOs.Pedidos;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PedidosController(AppDbContext db) => _db = db;

        // POST: api/pedidos
        [HttpPost]
        public async Task<ActionResult<PedidoReadDto>> Crear(PedidoCreateDto dto)
        {
            var pedido = new Pedido
            {
                ClienteNombre = dto.Cliente,
                ClienteEmail = dto.Email,
                Estado = EstadoPedido.Pendiente, // 👈 valor por defecto
                Fecha = DateTime.UtcNow
            };

            foreach (var it in dto.Detalles)
            {
                pedido.Detalles.Add(new PedidoDetalle
                {
                    ProductoId = it.ProductoId,
                    Cantidad = it.Cantidad,
                    PrecioUnitario = it.PrecioUnitario
                });
            }

            pedido.Total = pedido.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario);

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            return Ok(new PedidoReadDto
            {
                Id = pedido.Id,
                Cliente = pedido.ClienteNombre,
                Email = pedido.ClienteEmail,
                Fecha = pedido.Fecha,
                Total = pedido.Total,
                Estado = pedido.Estado,
                Detalles = pedido.Detalles.Select(d => new PedidoDetalleReadDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            });
        }

        // PUT: api/pedidos/{id}/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, PedidoUpdateEstadoDto dto)
        {
            var pedido = await _db.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = dto.Estado;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
