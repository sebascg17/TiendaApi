using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace TiendaApi.Controllers
{
    /// <summary>
    /// Controlador escalable para manejo de uploads
    /// Arquitectura diseñada para soportar tanto almacenamiento local como Azure
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;

        // Configuración escalable (luego se puede mover a appsettings.json)
        private const string UPLOADS_FOLDER = "uploads";
        private const string PROFILES_FOLDER = "profiles";
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5 MB
        private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif" };

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para subir foto de perfil
        /// Escalable: fácil cambiar a Azure Storage en el futuro
        /// </summary>
        [HttpPost("profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file, string userId, string uploadType = "profile")
        {
            try
            {
                // Validaciones
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file provided" });

                if (file.Length > MAX_FILE_SIZE)
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!ALLOWED_EXTENSIONS.Contains(extension))
                    return BadRequest(new { success = false, message = "Invalid file type. Only images allowed" });

                // Crear estructura de carpetas escalable
                var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", UPLOADS_FOLDER, PROFILES_FOLDER, userId);
                Directory.CreateDirectory(uploadPath);

                // Generar nombre único para evitar conflictos
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Ruta relativa para devolver al cliente (escalable)
                var relativePath = $"/uploads/{PROFILES_FOLDER}/{userId}/{fileName}";

                _logger.LogInformation($"Profile photo uploaded successfully: {relativePath}");

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully",
                    filePath = relativePath,
                    fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}" // URL completa
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error uploading file" });
            }
        }

        /// <summary>
        /// Endpoint genérico para subir archivos
        /// Escalable para futuros tipos de uploads
        /// </summary>
        [HttpPost("file")]
        public async Task<IActionResult> UploadFile(IFormFile file, string uploadType, [FromForm] string userId = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file provided" });

                // Determinar carpeta según tipo de upload
                var folder = uploadType switch
                {
                    "profile" => PROFILES_FOLDER,
                    "product" => "products",
                    "document" => "documents",
                    _ => "other"
                };

                var uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", UPLOADS_FOLDER, folder, userId);
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{folder}/{userId}/{fileName}";

                return Ok(new
                {
                    success = true,
                    message = "File uploaded successfully",
                    filePath = relativePath,
                    fileUrl = $"{Request.Scheme}://{Request.Host}{relativePath}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error uploading file" });
            }
        }

        /// <summary>
        /// Eliminar archivo (escalable para ambos proveedores)
        /// </summary>
        [HttpDelete("file")]
        public IActionResult DeleteFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return BadRequest(new { success = false, message = "File path required" });

                // Construir ruta física
                var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", filePath.TrimStart('/'));

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(new { success = false, message = "File not found" });

                System.IO.File.Delete(fullPath);

                _logger.LogInformation($"File deleted: {filePath}");

                return Ok(new { success = true, message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error deleting file" });
            }
        }
    }
}
