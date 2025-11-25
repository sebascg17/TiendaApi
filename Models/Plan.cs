using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class Plan
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; // Ej: "Gratis", "Emprendedor", "Premium"

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecioMensual { get; set; } = 0m;

        [MaxLength(1000)]
        public string? Beneficios { get; set; } // Texto o JSON con detalles

        public int? LimiteProductos { get; set; } // 0 = ilimitado

        public bool PermiteDominioPropio { get; set; } = false;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(20)]
        public string Estado { get; set; } = "activo"; // activo, inactivo

        // 🔗 Navegación
        public ICollection<Tienda>? Tiendas { get; set; }
    }
}
