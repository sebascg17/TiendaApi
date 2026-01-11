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
        public async Task<ActionResult<IEnumerable<ProductoReadDto>>> Get()
        {
            var productos = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .ThenInclude(pc => pc.Categoria)
                .ToListAsync();

            var productosDto = productos.Select(p => new ProductoReadDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                ImagenUrl = p.ImagenUrl,
                Activo = p.Activo,
                Stock = p.Stock,
                TiendaId = p.TiendaId,
                Categorias = p.ProductoCategorias
                    ?.Select(pc => new CategoriaSimpleDto
                    {
                        Id = pc.Categoria.Id,
                        Nombre = pc.Categoria.Nombre
                    })
                    .ToList() ?? new()
            }).ToList();

            return Ok(productosDto);
        }

        // ✅ GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoReadDto>> GetById(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .ThenInclude(pc => pc.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            var productoDto = new ProductoReadDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                ImagenUrl = producto.ImagenUrl,
                Activo = producto.Activo,
                Stock = producto.Stock,
                TiendaId = producto.TiendaId,
                Categorias = producto.ProductoCategorias
                    ?.Select(pc => new CategoriaSimpleDto
                    {
                        Id = pc.Categoria.Id,
                        Nombre = pc.Categoria.Nombre
                    })
                    .ToList() ?? new()
            };

            return Ok(productoDto);
        }

        // ✅ GET: api/productos/destacados
        [HttpGet("destacados")]
        public async Task<ActionResult<IEnumerable<ProductoReadDto>>> GetDestacados()
        {
            var productos = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .ThenInclude(pc => pc.Categoria)
                .Where(p => p.Activo)
                .Take(8)
                .ToListAsync();

            var productosDto = productos.Select(p => new ProductoReadDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                ImagenUrl = p.ImagenUrl,
                Activo = p.Activo,
                Stock = p.Stock,
                TiendaId = p.TiendaId,
                Categorias = p.ProductoCategorias
                    ?.Select(pc => new CategoriaSimpleDto
                    {
                        Id = pc.Categoria.Id,
                        Nombre = pc.Categoria.Nombre
                    })
                    .ToList() ?? new()
            }).ToList();

            return Ok(productosDto);
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
        [Authorize(Roles = "Admin,Tendero")]
        public async Task<ActionResult<ProductoReadDto>> Create([FromBody] ProductoCreateDto dto)
        {
            // Validar que TiendaId sea válido
            var tiendaExists = await _db.Tiendas.AnyAsync(t => t.Id == dto.TiendaId);
            if (!tiendaExists)
                return BadRequest(new { message = "La tienda especificada no existe" });

            var producto = new Producto
            {
                TiendaId = dto.TiendaId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock,
                ImagenUrl = dto.ImagenUrl,
                ProductoCategorias = dto.CategoriasIds.Select(cid => new ProductoCategoria
                {
                    CategoriaId = cid
                }).ToList()
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            // Recargar para obtener las categorías
            await _db.Entry(producto)
                .Collection(p => p.ProductoCategorias)
                .LoadAsync();
            await _db.Entry(producto)
                .Reference(p => p.ProductoCategorias.FirstOrDefault()!.Categoria)
                .LoadAsync();

            var productoDto = new ProductoReadDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                ImagenUrl = producto.ImagenUrl,
                Activo = producto.Activo,
                Stock = producto.Stock,
                TiendaId = producto.TiendaId,
                Categorias = producto.ProductoCategorias
                    ?.Select(pc => new CategoriaSimpleDto
                    {
                        Id = pc.Categoria.Id,
                        Nombre = pc.Categoria.Nombre
                    })
                    .ToList() ?? new()
            };

            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, productoDto);
        }

        // ✅ PUT: api/productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tendero")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            var producto = await _db.Productos
                .Include(p => p.ProductoCategorias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound();

            // Actualizamos propiedades
            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;

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
        [Authorize(Roles = "Admin,Tendero")]
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
        [Authorize(Roles = "Admin,Tendero")]
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
