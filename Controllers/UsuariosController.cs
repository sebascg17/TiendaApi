using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaApi.Infrastructure;
using TiendaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TiendaApi.DTOs.Usuarios;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsuariosController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ REGISTRO DE USUARIO
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // ✅ Asignar roles si se enviaron
            if (dto.Roles != null && dto.Roles.Any())
            {
                foreach (var rolNombre in dto.Roles)
                {
                    var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == rolNombre);
                    if (rol != null)
                    {
                        _context.UsuarioRoles.Add(new UsuarioRol
                        {
                            UsuarioId = usuario.Id,
                            RolId = rol.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Usuario registrado con éxito", usuario.Id });
        }

        // ✅ LOGIN DE USUARIO
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas");

            // Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales inválidas");

            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

            // 🔹 Claims del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim("id", usuario.Id.ToString())
            };

            foreach (var rol in roles)
                claims.Add(new Claim(ClaimTypes.Role, rol));

            // 🔹 Generar token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                roles
            });
        }

        // ✅ OBTENER TODOS LOS USUARIOS
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .Select(u => new UsuarioReadDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Roles = u.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList()
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // ✅ ACTUALIZAR USUARIO
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            usuario.Nombre = dto.Nombre ?? usuario.Nombre;
            usuario.Email = dto.Email ?? usuario.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Actualizar roles si se proporcionan
            if (dto.Roles != null)
            {
                usuario.UsuarioRoles.Clear();
                var roles = await _context.Roles
                    .Where(r => dto.Roles.Contains(r.Nombre))
                    .ToListAsync();

                usuario.UsuarioRoles = roles.Select(r => new UsuarioRol
                {
                    RolId = r.Id,
                    UsuarioId = usuario.Id
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ ASIGNAR ROL A UN USUARIO
        [HttpPost("asignar-rol")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AsignarRol(int usuarioId, string rolNombre)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == rolNombre);
            if (rol == null)
                return NotFound("Rol no encontrado");

            if (!usuario.UsuarioRoles.Any(ur => ur.RolId == rol.Id))
            {
                usuario.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                await _context.SaveChangesAsync();
            }

            return Ok($"Rol '{rolNombre}' asignado al usuario {usuario.Email}");
        }

        // ✅ VER CLAIMS DEL TOKEN
        [HttpGet("mis-claims")]
        [Authorize]
        public IActionResult MisClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });

            return Ok(claims);
        }
    }
}
