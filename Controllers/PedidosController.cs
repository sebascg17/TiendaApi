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
                TiendaId = dto.TiendaId,
                ClienteNombre = dto.Cliente,
                ClienteEmail = dto.Email,
                Estado = EstadoPedido.Pendiente, // 👈 valor por defecto
                Fecha = DateTime.UtcNow
            };

            foreach (var it in dto.Detalles)
            {
                // 📉 Obtener el producto y restar el stock
                var producto = await _db.Productos.FindAsync(it.ProductoId);
                if (producto == null)
                    return BadRequest(new { message = $"Producto con ID {it.ProductoId} no encontrado" });

                // Validar si hay stock disponible
                if (producto.Stock < it.Cantidad)
                    return BadRequest(new { message = $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}, Solicitado: {it.Cantidad}" });

                // Restar el stock
                producto.Stock -= it.Cantidad;

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

        // GET: api/pedidos/tienda/{tiendaId}
        [HttpGet("tienda/{tiendaId}")]
        public async Task<ActionResult<IEnumerable<PedidoReadDto>>> GetPedidosPorTienda(int tiendaId)
        {
            var pedidos = await _db.Pedidos
                .Where(p => p.TiendaId == tiendaId)
                .Include(p => p.Detalles)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new PedidoReadDto
                {
                    Id = p.Id,
                    Cliente = p.ClienteNombre,
                    Email = p.ClienteEmail,
                    Fecha = p.Fecha,
                    Total = p.Total,
                    Estado = p.Estado,
                    Detalles = p.Detalles.Select(d => new PedidoDetalleReadDto
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                })
                .ToListAsync();

            return Ok(pedidos);
        }

        // PUT: api/pedidos/{id}/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, PedidoUpdateEstadoDto dto)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (pedido == null) return NotFound();

            // 📈 Si se cancela el pedido, restaurar el stock
            if (dto.Estado == EstadoPedido.Cancelado && pedido.Estado != EstadoPedido.Cancelado)
            {
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _db.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        // Sumar el stock de vuelta
                        producto.Stock += detalle.Cantidad;
                    }
                }
            }

            pedido.Estado = dto.Estado;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
