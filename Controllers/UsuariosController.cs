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
using FirebaseAdmin.Auth;
using TiendaApi.Services;
using TiendaApi.Constants;

namespace TiendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public UsuariosController(AppDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        // ✅ REGISTRO DE USUARIO
        // ✅ REGISTRO DE USUARIO (Con Rol y Tienda opcional)
        // ✅ 1. REGISTRO DE TENDERO (Implícito)
        [HttpPost("register-store")]
        public async Task<IActionResult> RegisterStore([FromBody] UsuarioRegisterStoreDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            // Validar unicidad de tienda (slug básico)
            var slug = dto.NombreTienda.ToLower().Replace(" ", "-");
            if (await _context.Tiendas.AnyAsync(t => t.Slug == slug))
                 return BadRequest("El nombre de la tienda ya está en uso."); // Simplificación

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear Usuario
                var usuario = new Usuario
                {
                    Nombre = dto.NombreUsuario,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    VerificationToken = Guid.NewGuid().ToString(),
                    IsVerified = false // Requiere verificación
                };
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // 2. Asignar Rol Tendero
                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesConsts.Tendero);
                if (rol != null)
                {
                    _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                }

                // 3. Crear Tienda
                var tienda = new Tienda
                {
                    UsuarioId = usuario.Id,
                    Nombre = dto.NombreTienda,
                    Descripcion = dto.DireccionTienda,
                    Slug = slug,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "activo"
                };
                _context.Tiendas.Add(tienda);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 📧 Email
                await _emailService.SendVerificationEmailAsync(usuario.Email, usuario.Nombre, usuario.VerificationToken);

                return Ok(new { message = "Cuenta de Tendero creada. Verifica tu correo." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ 2. REGISTRO DE CLIENTE (Implícito)
        [HttpPost("register-client")]
        public async Task<IActionResult> RegisterClient([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                VerificationToken = Guid.NewGuid().ToString(),
                IsVerified = false
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Asignar Rol Cliente
            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesConsts.Cliente);
            if (rol != null)
            {
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                await _context.SaveChangesAsync();
            }

            // 📧 Email
            await _emailService.SendVerificationEmailAsync(usuario.Email, usuario.Nombre, usuario.VerificationToken);

            return Ok(new { message = "Cuenta de Cliente creada. Verifica tu correo." });
        }

        // ✅ 3. REGISTRO DE ADMIN (Temporal/Test)
        [HttpPost("register-admin-temp")]
        public async Task<IActionResult> RegisterAdminTemp([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                VerificationToken = Guid.NewGuid().ToString(), // Admin auto-verificado? No, igual verificar.
                IsVerified = true // Auto-verificado para facilitar tests
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Admin");
            if (rol != null)
            {
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Cuenta Admin (Test) creada." });
        }

        // ✅ CONFIRMAR EMAIL
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (usuario == null)
            {
                return BadRequest("Token de verificación inválido o expirado.");
            }

            usuario.IsVerified = true;
            usuario.VerificationToken = null; // Token de un solo uso
            await _context.SaveChangesAsync();

            // Redirigir al Frontend (Login) con mensaje de éxito (query param)
            return Redirect("http://localhost:4200/login?verified=true");
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
        

        // ✅ LOGIN CON REDES SOCIALES (Google, Facebook, Twitter, etc.)
        // ✅ LOGIN CON REDES SOCIALES (Google, Facebook, Twitter, etc.)
        [HttpPost("google-login")]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.IdToken))
                return BadRequest("Se requiere el ID Token de Firebase.");

            try
            {
                // 1. Verificar ID Token
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(dto.IdToken);

                // 2. Obtener claims de forma segura
                string email = decodedToken.Claims.GetValueOrDefault("email")?.ToString() ?? "";
                string nombre = decodedToken.Claims.GetValueOrDefault("name")?.ToString() ?? "Usuario Google";
                string picture = decodedToken.Claims.GetValueOrDefault("picture")?.ToString() ?? "";

                if (string.IsNullOrEmpty(email))
                    return BadRequest("El token de Google no contiene un correo electrónico válido.");

                // 3. Buscar/Crear usuario
                var usuario = await _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null)
                {
                    usuario = new Usuario
                    {
                        Nombre = nombre,
                        Email = email,
                        PasswordHash = null,
                        IsVerified = true, // Google ya verificó el email
                        VerificationToken = null
                    };

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try 
                    {
                        _context.Usuarios.Add(usuario);
                        await _context.SaveChangesAsync();

                        var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesConsts.Cliente);
                        if (rolCliente != null)
                        {
                            _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rolCliente.Id });
                            await _context.SaveChangesAsync();
                        }
                        
                        await transaction.CommitAsync();
                        // Recargar usuario para incluir relaciones
                        await _context.Entry(usuario).Collection(u => u.UsuarioRoles).Query().Include(ur => ur.Rol).LoadAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                // 4. Generar JWT propio
                var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();
                
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                    new Claim("id", usuario.Id.ToString())
                };
                foreach (var rol in roles) claims.Add(new Claim(ClaimTypes.Role, rol));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)); // ! para null check
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
                    roles,
                    nombre = usuario.Nombre,
                    email = usuario.Email,
                    photoUrl = picture
                });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized(new { message = $"Token de Firebase inválido: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}
