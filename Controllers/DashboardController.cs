using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using TiendaApi.DTOs.Pedidos;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/dashboard/vendedor/{tiendaId}
        [HttpGet("vendedor/{tiendaId}")]
        public async Task<IActionResult> GetVendorDashboard(int tiendaId)
        {
            // 1. Ventas Totales (Suma de pedidos confirmados/entregados)
            var ventasTotales = await _db.Pedidos
                .Where(p => p.TiendaId == tiendaId && (p.Estado == EstadoPedido.Confirmado || p.Estado == EstadoPedido.Entregado))
                .SumAsync(p => p.Total);

            // 2. Productos Activos
            var productosActivos = await _db.Productos
                .CountAsync(p => p.TiendaId == tiendaId && p.Activo);

            // 3. Pedidos Recientes (Últimos 5)
            var pedidosRecientes = await _db.Pedidos
                .Where(p => p.TiendaId == tiendaId)
                .OrderByDescending(p => p.Fecha)
                .Take(5)
                .Select(p => new PedidoReadDto
                {
                    Id = p.Id,
                    Cliente = p.ClienteNombre,
                    Email = p.ClienteEmail,
                    Fecha = p.Fecha,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToListAsync();

            // 4. Saldo Actual (Necesitamos el UsuarioId del dueño de la tienda)
            var tienda = await _db.Tiendas.FindAsync(tiendaId);
            decimal saldo = 0;
            if (tienda != null)
            {
                saldo = await _db.Transacciones
                    .Where(t => t.UsuarioId == tienda.UsuarioId && t.Estado == "Completada")
                    .SumAsync(t => t.Tipo == "Recarga" || t.Tipo == "Venta" ? t.Monto : -t.Monto);
            }

            // 5. Estadísticas de Pedidos (Para gráficos)
            var statsPedidos = await _db.Pedidos
                .Where(p => p.TiendaId == tiendaId)
                .GroupBy(p => p.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                VentasTotales = ventasTotales,
                ProductosActivos = productosActivos,
                PedidosRecientes = pedidosRecientes,
                Saldo = saldo,
                StatsPedidos = statsPedidos
            });
        }

        // GET: api/dashboard/admin
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            // 1. Estadísticas Globales
            var totalUsers = await _db.Usuarios.CountAsync();
            var totalProducts = await _db.Productos.CountAsync();
            var totalOrders = await _db.Pedidos.CountAsync();
            var totalRevenue = await _db.Pedidos
                .Where(p => p.Estado == EstadoPedido.Confirmado || p.Estado == EstadoPedido.Entregado)
                .SumAsync(p => p.Total);

            // Desglose de Usuarios por Rol
            var userBreakdown = await _db.UsuarioRoles
                .Include(ur => ur.Rol)
                .GroupBy(ur => ur.Rol.Nombre)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToListAsync();

            // 2. Usuarios Recientes (Sessions)
            var recentUsers = await _db.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .OrderByDescending(u => u.UltimaSesion ?? u.FechaRegistro)
                .Take(5)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.UltimaSesion,
                    u.FechaRegistro,
                    Roles = u.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList()
                })
                .ToListAsync();

            // 3. Órdenes Recientes
            var recentOrders = await _db.Pedidos
                .OrderByDescending(p => p.Fecha)
                .Take(5)
                .Select(p => new PedidoReadDto
                {
                    Id = p.Id,
                    Cliente = p.ClienteNombre,
                    Email = p.ClienteEmail,
                    Fecha = p.Fecha,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Ok(new
            {
                TotalUsers = totalUsers,
                UserBreakdown = userBreakdown,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                RecentUsers = recentUsers,
                RecentOrders = recentOrders
            });
        }
    }
}
