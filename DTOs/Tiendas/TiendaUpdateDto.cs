using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Tiendas
{
    public class TiendaUpdateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? DocumentoIdentidad { get; set; }

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(255)]
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Pais { get; set; }

        [MaxLength(100)]
        public string? Departamento { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        public string? LogoUrl { get; set; }
        public int? PlanId { get; set; }
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
        
        public string? Slug { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }
}
