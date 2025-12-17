using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioRegisterStoreDto
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        [Required]
        public string NombreTienda { get; set; } = string.Empty;

        public string? DireccionTienda { get; set; }
    }
}
