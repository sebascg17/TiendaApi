using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class PedidoDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Pedido")]
        public int PedidoId { get; set; }

        [Required, ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        // Subtotal opcional (puedes calcularlo en la capa de negocio)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }
    }
}
