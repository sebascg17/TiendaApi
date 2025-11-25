using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApi.DTOs.Roles;
using TiendaApi.Infrastructure;
using TiendaApi.Models;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo los administradores pueden gestionar roles
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================
        // 🔹 OBTENER TODOS LOS ROLES
        // =====================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new RolDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre
                })
                .ToListAsync();

            return Ok(roles);
        }

        // =====================================
        // 🔹 OBTENER UN ROL POR ID
        // =====================================
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> GetRolById(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound("Rol no encontrado");

            return Ok(new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre
            });
        }

        // =====================================
        // 🔹 CREAR NUEVO ROL
        // =====================================
        [HttpPost]
        public async Task<IActionResult> CreateRol([FromBody] RolDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del rol es obligatorio");

            // Verificar si ya existe
            var existe = await _context.Roles.AnyAsync(r => r.Nombre == dto.Nombre);
            if (existe)
                return Conflict($"El rol '{dto.Nombre}' ya existe.");

            var rol = new Rol
            {
                Nombre = dto.Nombre
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            dto.Id = rol.Id; // devolver el ID generado

            return CreatedAtAction(nameof(GetRolById), new { id = rol.Id }, dto);
        }

        // =====================================
        // 🔹 ACTUALIZAR ROL
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto dto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound("Rol no encontrado");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del rol no puede estar vacío");

            // Verificar duplicado
            var existe = await _context.Roles.AnyAsync(r => r.Nombre == dto.Nombre && r.Id != id);
            if (existe)
                return Conflict($"Ya existe otro rol con el nombre '{dto.Nombre}'.");

            rol.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rol actualizado correctamente" });
        }

        // =====================================
        // 🔹 ELIMINAR ROL
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound("Rol no encontrado");

            // Validar si tiene usuarios asignados
            bool tieneUsuarios = await _context.UsuarioRoles.AnyAsync(ur => ur.RolId == id);
            if (tieneUsuarios)
                return BadRequest("No se puede eliminar el rol porque está asignado a uno o más usuarios.");

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rol '{rol.Nombre}' eliminado correctamente" });
        }
    }
}
