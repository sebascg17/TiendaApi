using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Tiendas
{
    public class TiendaUpdateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(255)]
        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Pais { get; set; }

        [MaxLength(100)]
        public string? Departamento { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        public string? LogoUrl { get; set; }
        
        [MaxLength(20)]
        public string? ColorPrimario { get; set; }
    }
}
