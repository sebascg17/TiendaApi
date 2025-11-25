using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.DTOs.Productos;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductosController(AppDbContext db) => _db = db;

        // ✅ GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Get()
        {
            var productos = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .ThenInclude(pc => pc.Categoria)
                .ToListAsync();

            return Ok(productos);
        }

        // ✅ GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .ThenInclude(pc => pc.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();
            return Ok(producto);
        }

        // ✅ GET: api/productos/{id}/categorias
        [HttpGet("{id}/categorias")]
        public async Task<IActionResult> GetCategoriasByProducto(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                    .ThenInclude(pc => pc.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            var categorias = producto.ProductoCategorias
                .Select(pc => new 
                {
                    pc.CategoriaId,
                    pc.Categoria!.Nombre
                });

            return Ok(categorias);
        }


        // ✅ POST: api/productos
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Producto>> Create([FromBody] ProductoCreateDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                ProductoCategorias = dto.CategoriasIds.Select(cid => new ProductoCategoria
                {
                    CategoriaId = cid
                }).ToList()
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
        }

        // ✅ PUT: api/productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            // Actualizamos propiedades
            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;

            // Actualizamos relación muchos a muchos
            producto.ProductoCategorias.Clear();
            foreach (var cid in dto.CategoriasIds)
            {
                producto.ProductoCategorias.Add(new ProductoCategoria { CategoriaId = cid });
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: api/productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _db.Remove(producto);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ✅ PATCH: api/productos/{id}/categorias
        [HttpPatch("{id}/categorias")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategorias(int id, [FromBody] ProductoCategoriasDto dto)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            // Limpiamos las categorías actuales
            producto.ProductoCategorias.Clear();

            // Agregamos las nuevas categorías
            foreach (var cid in dto.CategoriasIds)
            {
                producto.ProductoCategorias.Add(new ProductoCategoria { CategoriaId = cid });
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
