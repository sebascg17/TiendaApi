using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using TiendaApi.DTOs.Pedidos;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoDetallesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PedidoDetallesController(AppDbContext db) => _db = db;

        // POST: api/pedidodetalles
        [HttpPost]
        public async Task<IActionResult> AgregarDetalle(PedidoDetalleCreateDto dto)
        {
            var detalle = new PedidoDetalle
            {
                PedidoId = dto.PedidoId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = dto.PrecioUnitario
            };

            _db.PedidoDetalles.Add(detalle);

            var pedido = await _db.Pedidos.Include(p => p.Detalles)
                                          .FirstOrDefaultAsync(p => p.Id == dto.PedidoId);

            if (pedido != null)
                pedido.Total = pedido.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario);

            await _db.SaveChangesAsync();

            return Ok(new { detalle.Id, detalle.PedidoId });
        }

        // PUT: api/pedidodetalles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarDetalle(int id, PedidoDetalleUpdateDto dto)
        {
            var detalle = await _db.PedidoDetalles.FindAsync(id);
            if (detalle == null) return NotFound();

            detalle.Cantidad = dto.Cantidad;
            detalle.PrecioUnitario = dto.PrecioUnitario;

            var pedido = await _db.Pedidos.Include(p => p.Detalles)
                                          .FirstOrDefaultAsync(p => p.Id == detalle.PedidoId);

            if (pedido != null)
                pedido.Total = pedido.Detalles.Sum(x => x.Cantidad * x.PrecioUnitario);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
