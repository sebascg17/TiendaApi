using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComisionesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ComisionesController(AppDbContext db)
        {
            _db = db;
        }

        // POST: api/comisiones/calcular/{pedidoId}
        [HttpPost("calcular/{pedidoId}")]
        public async Task<IActionResult> CalcularComision(int pedidoId)
        {
            var pedido = await _db.Pedidos.Include(p => p.Tienda).FirstOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null) return NotFound("Pedido no encontrado");

            if (pedido.Estado != EstadoPedido.Entregado && pedido.Estado != EstadoPedido.Confirmado) 
            {
                 return BadRequest("El pedido debe estar Confirmado o Entregado para calcular comisiones.");
            }

            // Verificar si ya existe comisión para este pedido
            var existe = await _db.Comisiones.AnyAsync(c => c.PedidoId == pedidoId);
            if (existe) return BadRequest("Ya se han calculado comisiones para este pedido.");

            // Lógica de comisión (Ejemplo: 10% para la plataforma)
            decimal porcentajeComision = 0.10m;
            decimal montoComision = pedido.Total * porcentajeComision;
            decimal montoVendedor = pedido.Total - montoComision;

            // 1. Registrar Comisión Plataforma
            // Asumimos que hay un usuario "Admin" o "Plataforma" (UsuarioId 1 por ejemplo, o null en origen)
            // Aquí simplificamos: Creamos registro de comisión
            var comision = new Comision
            {
                PedidoId = pedidoId,
                UsuarioDestinoId = 1, // ID del Admin/Plataforma (HARDCODED FOR MVP)
                UsuarioOrigenId = pedido.Tienda.UsuarioId, // Dueño de la tienda
                Tipo = "PlatformFee",
                Monto = montoComision,
                FechaCreacion = DateTime.UtcNow
            };
            _db.Comisiones.Add(comision);

            // 2. Registrar Transacción de Venta para el Vendedor (Ingreso)
            var transaccionVenta = new Transaccion
            {
                UsuarioId = pedido.Tienda.UsuarioId,
                PedidoId = pedidoId,
                Tipo = "Venta",
                Monto = pedido.Total, // Ingresa el total
                Estado = "Completada",
                Referencia = $"Venta Pedido #{pedido.Id}",
                FechaCreacion = DateTime.UtcNow
            };
            _db.Transacciones.Add(transaccionVenta);

            // 3. Registrar Transacción de Comisión (Egreso del Vendedor)
            var transaccionComision = new Transaccion
            {
                UsuarioId = pedido.Tienda.UsuarioId,
                PedidoId = pedidoId,
                Tipo = "Comisión",
                Monto = montoComision, // Se descuenta la comisión
                Estado = "Completada",
                Referencia = $"Comisión Pedido #{pedido.Id}",
                FechaCreacion = DateTime.UtcNow
            };
            _db.Transacciones.Add(transaccionComision);

            await _db.SaveChangesAsync();

            return Ok(new { Message = "Comisiones calculadas y transacciones registradas.", MontoComision = montoComision, MontoVendedor = montoVendedor });
        }
    }
}
