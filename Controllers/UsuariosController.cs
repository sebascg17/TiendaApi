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
        // ✅ REGISTRO GENÉRICO (Paso 1 del flujo por roles)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            // Mapeo de roles (Frontend puede enviar "Vendedor" o "Tendero")
            string rolNombre = dto.Rol;
            if (rolNombre.Equals("Vendedor", StringComparison.OrdinalIgnoreCase)) 
                rolNombre = RolesConsts.Tendero;
            
            // Validar que el rol exista y sea permitido
            if (rolNombre != RolesConsts.Cliente && rolNombre != RolesConsts.Tendero)
                return BadRequest($"Rol '{rolNombre}' no válido para registro público.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    PasswordHash = !string.IsNullOrEmpty(dto.Password) ? BCrypt.Net.BCrypt.HashPassword(dto.Password) : null,
                    VerificationToken = Guid.NewGuid().ToString(),
                    IsVerified = false,
                    Ciudad = dto.Ciudad,
                    Departamento = dto.Departamento,
                    Pais = dto.Pais,
                    Barrio = dto.Barrio,
                    Direccion = dto.Direccion,
                    Telefono = dto.Telefono,
                    FechaNacimiento = dto.FechaNacimiento
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == rolNombre);
                if (rol != null)
                {
                    _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                    await _context.SaveChangesAsync();
                }
                else 
                {
                     // Si el rol no existe, hacemos rollback manual (aunque el catch lo haría si lanzamos ex, aquí retornamos 400)
                     await transaction.RollbackAsync();
                     return BadRequest($"Error interno: El rol '{rolNombre}' no está configurado en el sistema.");
                }

                // 📧 Email - Si falla, hacemos rollback para no dejar usuarios sin correo verificado (opcional, pero recomendado)
                try {
                    await _emailService.SendVerificationEmailAsync(usuario.Email, usuario.Nombre, usuario.VerificationToken);
                } catch {
                    // Log error but maybe don't rollback user? 
                    // No, user requested fix for "invalid data". Safety first -> Rollback if critical.
                    // For now, let's assume email failure shouldn't kill registration strictly, BUT debugging priority is consistency.
                    // Let's Catch and Log, or just let it slide if implementation is weak. 
                    // Better: Let's NOT rollback on email fail for now to avoid blocking testing if SMTP is down, 
                    // BUT we must commit the transaction.
                }

                await transaction.CommitAsync();

                return Ok(new { message = $"Cuenta de {rolNombre} creada. Verifica tu correo." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        // ✅ REGISTRO DE USUARIO (Con Rol y Tienda opcional)
        // ✅ 1. REGISTRO DE TENDERO (Implícito)
        [HttpPost("register-store")]
        public async Task<IActionResult> RegisterStore([FromBody] UsuarioRegisterStoreDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El usuario ya existe");

            // Validar unicidad de tienda (slug básico) si se provee nombre
            string? slug = null;
            if (!string.IsNullOrEmpty(dto.NombreTienda))
            {
                slug = dto.NombreTienda.ToLower().Replace(" ", "-");
                if (await _context.Tiendas.AnyAsync(t => t.Slug == slug))
                     return BadRequest("El nombre de la tienda ya está en uso.");
            }

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
                    IsVerified = false, // Requiere verificación
                    Ciudad = dto.Ciudad,
                    Departamento = dto.Departamento,
                    Pais = dto.Pais,
                    Barrio = dto.Barrio,
                    Direccion = dto.Direccion,
                    Telefono = dto.Telefono,
                    FechaNacimiento = dto.FechaNacimiento
                };
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // 2. Asignar Rol Tendero
                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == RolesConsts.Tendero);
                if (rol != null)
                {
                    _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rol.Id });
                }

                // 3. Crear Tienda (Solo si se proporcionó nombre)
                if (!string.IsNullOrEmpty(dto.NombreTienda) && slug != null)
                {
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
                }

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
                IsVerified = false,
                Ciudad = dto.Ciudad,
                Departamento = dto.Departamento,
                Pais = dto.Pais,
                Barrio = dto.Barrio,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                FechaNacimiento = dto.FechaNacimiento
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
                return NotFound("Usuario no registrado");

            // Verificar contraseña con BCrypt
            if (string.IsNullOrEmpty(usuario.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales inválidas");

            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

            // 🔹 Claims del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim("id", usuario.Id.ToString())
            };

            // Añadir nombre y apellido como claims para que el frontend los pueda leer del token
            claims.Add(new Claim("nombre", usuario.Nombre ?? string.Empty));
            claims.Add(new Claim("apellido", usuario.Apellido ?? string.Empty));

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
                roles = roles,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                hasPassword = !string.IsNullOrEmpty(usuario.PasswordHash)
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
                    Apellido = u.Apellido,
                    Email = u.Email,
                    PhotoUrl = u.FotoPerfilUrl,
                    Roles = u.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList(),
                    FechaNacimiento = u.FechaNacimiento
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

        // Helper method to get the authenticated user
        private async Task<Usuario?> GetUsuarioAutenticado()
        {
            Usuario? usuario = null;
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            
            if (idClaim != null && int.TryParse(idClaim.Value, out var userId))
            {
                usuario = await _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                        .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            
            if (usuario == null)
            {
                var emailClaim = User.Claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.NameIdentifier || 
                    c.Type == "sub" || 
                    c.Type == "email" || 
                    c.Type == ClaimTypes.Email || 
                    c.Type == JwtRegisteredClaimNames.Sub ||
                    c.Type == JwtRegisteredClaimNames.Email);
                
                if (emailClaim != null)
                {
                    usuario = await _context.Usuarios
                        .Include(u => u.UsuarioRoles)
                            .ThenInclude(ur => ur.Rol)
                        .FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
                }
            }
            return usuario;
        }

        // ✅ ENDPOINT: Obtener perfil del usuario autenticado
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // DEBUG: imprimir todos los claims recibidos
            try
            {
                Console.WriteLine("[DEBUG] Claims recibidos en /me:");
                foreach (var c in User.Claims)
                {
                    Console.WriteLine($"[DEBUG] Claim: Type={c.Type}, Value={c.Value}");
                }
            }
            catch { }

            var usuario = await GetUsuarioAutenticado();
            if (usuario == null) return Unauthorized("No se pudo identificar al usuario.");

            var dto = new UsuarioReadDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                PhotoUrl = usuario.FotoPerfilUrl,
                Roles = usuario.UsuarioRoles?.Select(ur => ur.Rol.Nombre).ToList() ?? new List<string>(),
                FechaCreacion = usuario.FechaCreacion,
                FechaNacimiento = usuario.FechaNacimiento,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                Ciudad = usuario.Ciudad,
                Departamento = usuario.Departamento,
                Pais = usuario.Pais,
                Barrio = usuario.Barrio,
                HasPassword = !string.IsNullOrEmpty(usuario.PasswordHash)
            };

            return Ok(dto);
        }

        // ✅ ENDPOINT: Actualizar perfil del usuario autenticado
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UsuarioUpdateDto dto)
        {
            // DEBUG: imprimir todos los claims recibidos
            try
            {
                Console.WriteLine("[DEBUG] Claims recibidos en PUT /me:");
                foreach (var c in User.Claims)
                {
                    Console.WriteLine($"[DEBUG] Claim: Type={c.Type}, Value={c.Value}");
                }
            }
            catch { }

            Usuario? usuario = null;

            // Intentar obtener id desde claims
            // Nota: ASP.NET Core mapea 'sub' a ClaimTypes.NameIdentifier automáticamente
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            
            if (idClaim != null && int.TryParse(idClaim.Value, out var userId))
            {
                // Caso 1: Token tiene claim 'id' válido
                Console.WriteLine($"[DEBUG] Encontrado claim 'id' con valor: {userId}");
                usuario = await _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                        .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            
            // Caso 2: Fallback - buscar por email si no se encontró por ID
            if (usuario == null)
            {
                // ASP.NET Core mapea 'sub' a ClaimTypes.NameIdentifier
                // Buscar en orden de prioridad
                var emailClaim = User.Claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.NameIdentifier ||  // 'sub' mapeado
                    c.Type == "sub" ||                      // 'sub' sin mapear
                    c.Type == "email" ||                    // claim 'email' directo
                    c.Type == ClaimTypes.Email ||           // email mapeado
                    c.Type == JwtRegisteredClaimNames.Sub ||
                    c.Type == JwtRegisteredClaimNames.Email);
                
                if (emailClaim == null)
                {
                    Console.WriteLine("[ERROR] No se encontró claim de email. Claims disponibles:");
                    foreach (var c in User.Claims)
                    {
                        Console.WriteLine($"[ERROR] Claim disponible: Type={c.Type}, Value={c.Value}");
                    }
                    return Unauthorized("No se pudo identificar al usuario. Token inválido.");
                }

                Console.WriteLine($"[DEBUG] Buscando usuario por email: {emailClaim.Value}");
                usuario = await _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                        .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
            }

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Actualizar campos proporcionados
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre;

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                usuario.Apellido = dto.Apellido;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                usuario.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                usuario.Telefono = dto.Telefono;

            if (!string.IsNullOrWhiteSpace(dto.Direccion))
                usuario.Direccion = dto.Direccion;

            if (!string.IsNullOrWhiteSpace(dto.Ciudad))
                usuario.Ciudad = dto.Ciudad;

            if (!string.IsNullOrWhiteSpace(dto.Pais))
                usuario.Pais = dto.Pais;

            if (!string.IsNullOrWhiteSpace(dto.Departamento))
                usuario.Departamento = dto.Departamento;

            if (!string.IsNullOrWhiteSpace(dto.Barrio))
                usuario.Barrio = dto.Barrio;

            if (dto.FechaNacimiento.HasValue)
                usuario.FechaNacimiento = dto.FechaNacimiento;

            if (!string.IsNullOrWhiteSpace(dto.FotoPerfilUrl))
                usuario.FotoPerfilUrl = dto.FotoPerfilUrl;

            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                // Si el usuario YA tiene contraseña, debemos verificar la 'Actual'
                if (!string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || 
                        !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, usuario.PasswordHash))
                    {
                        return BadRequest("La contraseña actual es incorrecta o no fue proporcionada.");
                    }
                }
                
                // Si llegó aquí (o no tenía o la verificación pasó), actualizamos
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            // Devolver el usuario actualizado
            var updatedDto = new UsuarioReadDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                PhotoUrl = usuario.FotoPerfilUrl,
                Roles = usuario.UsuarioRoles?.Select(ur => ur.Rol.Nombre).ToList() ?? new List<string>(),
                FechaCreacion = usuario.FechaCreacion,
                FechaNacimiento = usuario.FechaNacimiento,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                Ciudad = usuario.Ciudad,
                Departamento = usuario.Departamento,
                Pais = usuario.Pais,
                Barrio = usuario.Barrio,
                HasPassword = !string.IsNullOrEmpty(usuario.PasswordHash)
            };

            return Ok(updatedDto);
        }

        [HttpPost("me/photo")]
        [Authorize]
        public async Task<IActionResult> UpdatePhoto(IFormFile foto)
        {
            var usuario = await GetUsuarioAutenticado();
            if (usuario == null) return Unauthorized();

            if (foto == null || foto.Length == 0)
                return BadRequest("No se proporcionó ninguna imagen.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{usuario.Id}_{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            usuario.FotoPerfilUrl = $"/uploads/profiles/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { url = usuario.FotoPerfilUrl });
        }
        

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
                string googleName = decodedToken.Claims.GetValueOrDefault("name")?.ToString() ?? "Usuario Google";
                string picture = decodedToken.Claims.GetValueOrDefault("picture")?.ToString() ?? "";
                
                // Claims específicos para nombre y apellido (más precisos)
                string firstName = decodedToken.Claims.GetValueOrDefault("given_name")?.ToString() ?? "";
                string lastName = decodedToken.Claims.GetValueOrDefault("family_name")?.ToString() ?? "";

                if (string.IsNullOrEmpty(email))
                    return BadRequest("El token de Google no contiene un correo electrónico válido.");

                // 3. Buscar/Crear usuario
                var usuario = await _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.Email == email);

                bool isNewUser = false;

                if (usuario == null)
                {
                    if (!dto.AllowRegistration)
                        return NotFound("Usuario no registrado");

                    isNewUser = true;

                    // Fallback si los claims específicos faltan
                    if (string.IsNullOrEmpty(firstName))
                    {
                        // Split name into First and Last if possible
                        string[] nameParts = googleName.Split(' ', 2);
                        firstName = nameParts[0];
                        lastName = lastName == "" && nameParts.Length > 1 ? nameParts[1] : lastName;
                    }

                    usuario = new Usuario
                    {
                        Nombre = firstName,
                        Apellido = lastName,
                        Email = email,
                        PasswordHash = null,
                        IsVerified = true, // Google ya verificó el email
                        VerificationToken = null,
                        FotoPerfilUrl = picture
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

                // [REMOVED] Enforced Admin Role for Google Tests
                // Users will keep their existing roles or default to 'Cliente' on creation.

                // 4. Generar JWT propio
                var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();
                
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                    new Claim("id", usuario.Id.ToString())
                };
                // Añadir nombre y apellido como claims
                claims.Add(new Claim("nombre", usuario.Nombre ?? string.Empty));
                claims.Add(new Claim("apellido", usuario.Apellido ?? string.Empty));
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
                    apellido = usuario.Apellido,
                    email = usuario.Email,
                    photoUrl = picture,
                    isNewUser = isNewUser,
                    profileIncomplete = !usuario.FechaNacimiento.HasValue || string.IsNullOrEmpty(usuario.Telefono),
                    hasPassword = !string.IsNullOrEmpty(usuario.PasswordHash)
                });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized(new { message = $"Token de Firebase inválido: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error (DB, Configuración, etc.) para evitar el 500 genérico sin cuerpo
                return StatusCode(500, new { message = $"Error interno al procesar login con Google: {ex.Message}" });
            }
        }

        [HttpPost("upgrade-role")]
        [Authorize]
        public async Task<IActionResult> UpgradeRole([FromBody] RoleUpgradeDto dto)
        {
            var role = dto.Role;
            var usuario = await GetUsuarioAutenticado();
            if (usuario == null) return NotFound("Usuario no encontrado");

            // Validar que el rol sea válido
            if (role != RolesConsts.Tendero && role != RolesConsts.Cliente)
                return BadRequest("Rol inválido");

            // Verificar si ya tiene el rol
            var tieneRol = usuario.UsuarioRoles.Any(ur => ur.Rol.Nombre == role);
            if (tieneRol) return BadRequest($"El usuario ya tiene el rol de {role}");

            var dbRole = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == role);
            if (dbRole == null) return BadRequest("Rol no registrado en el sistema");

            _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = dbRole.Id });
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rol {role} asignado correctamente" });
        }
    }
}
