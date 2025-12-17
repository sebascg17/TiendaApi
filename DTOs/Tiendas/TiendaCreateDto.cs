using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Tiendas
{
    public class TiendaCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public string? Direccion { get; set; } // Opcional, o requerido según reglas
        public string? Telefono { get; set; }

        public string? ColorPrimario { get; set; }

        // El usuario se obtiene del token, no del DTO
    }
}
