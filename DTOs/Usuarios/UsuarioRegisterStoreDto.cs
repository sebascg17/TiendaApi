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
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        public string Departamento { get; set; } = string.Empty;

        public string Barrio { get; set; } = string.Empty;
        
        [Required]
        public string Pais { get; set; } = string.Empty;

        public string? NombreTienda { get; set; }

        public string? DireccionTienda { get; set; }
    }
}
