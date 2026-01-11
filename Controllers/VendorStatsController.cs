using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaApi.Infrastructure;

namespace TiendaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tendero,Admin")]
    public class VendorStatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorStatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/vendorstats
        [HttpGet]
        public async Task<IActionResult> GetVendorStats()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim);

            // Get user's stores
            var tiendas = await _context.Tiendas
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => new { t.Id, t.Nombre, t.Estado, t.LogoUrl })
                .ToListAsync();

            // Get total products across all stores
            var tiendaIds = tiendas.Select(t => t.Id).ToList();
            var totalProducts = await _context.Productos
                .Where(p => tiendaIds.Contains(p.TiendaId))
                .CountAsync();

            // Get sales stats (mock for now - will need Pedidos/Transacciones table)
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            // TODO: Replace with real data when Pedidos table is ready
            var monthlySales = 0;
            var totalSales = 0.0m;
            var totalOrders = 0;

            // Get most sold product (mock - needs real sales data)
            var topProduct = await _context.Productos
                .Where(p => tiendaIds.Contains(p.TiendaId))
                .OrderByDescending(p => p.Stock) // Placeholder: should be by sales count
                .Select(p => new { p.Nombre, p.ImagenUrl, SalesCount = 0 }) // Mock sales count
                .FirstOrDefaultAsync();

            return Ok(new
            {
                tiendas = tiendas.Select(t => new
                {
                    t.Id,
                    t.Nombre,
                    t.Estado,
                    t.LogoUrl,
                    isActive = t.Estado == "activo"
                }),
                totalProducts,
                monthlySales,
                totalSales,
                totalOrders,
                topProduct = topProduct != null ? new
                {
                    topProduct.Nombre,
                    topProduct.ImagenUrl,
                    salesCount = 0 // Placeholder
                } : null
            });
        }
    }
}
