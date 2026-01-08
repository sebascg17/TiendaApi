using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Tiendas
{
    public class TiendaCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? DocumentoIdentidad { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public string? Direccion { get; set; }
        public string? Pais { get; set; }
        public string? Departamento { get; set; }
        public string? Ciudad { get; set; }
        public string? Barrio { get; set; }
        
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public string? ColorPrimario { get; set; }

        // Nuevos Campos Pro
        public string? Bloque { get; set; }
        public string? Apto { get; set; }
        public string? Torre { get; set; }
        public string? Referencia { get; set; }

        public int? HoraApertura { get; set; }
        public int? MinutoApertura { get; set; }
        public int? HoraCierre { get; set; }
        public int? MinutoCierre { get; set; }

        public int? TipoTiendaId { get; set; }

        [Required]
        public string Slug { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        // El usuario se obtiene del token, no del DTO
    }
}
