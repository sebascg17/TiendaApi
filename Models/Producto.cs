using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Tienda")]
        public int TiendaId { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; } = 0;

        [MaxLength(255)]
        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;

        [MaxLength(50)]
        public string? SKU { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // 🔗 Relaciones
        public Tienda? Tienda { get; set; }
        public ICollection<ProductoCategoria>? ProductoCategorias { get; set; }
        public ICollection<PedidoDetalle>? PedidoDetalles { get; set; }
    }
}
