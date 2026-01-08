using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        [ForeignKey("Tienda")]
        public int? TiendaId { get; set; } // null = categoría global

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // 🔗 Relaciones
        public Tienda? Tienda { get; set; }
        public ICollection<ProductoCategoria>? ProductoCategorias { get; set; }
    }
}
