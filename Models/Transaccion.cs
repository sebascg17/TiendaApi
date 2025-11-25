using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        public int? PedidoId { get; set; }
        [ForeignKey(nameof(PedidoId))]
        public Pedido? Pedido { get; set; }

        public int? ComisionId { get; set; }
        [ForeignKey(nameof(ComisionId))]
        public Comision? Comision { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty; // Recarga, Retiro, Comisión, Venta, Reembolso

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [StringLength(10)]
        public string Moneda { get; set; } = "COP";

        [StringLength(100)]
        public string? Referencia { get; set; } // Id externo o nota

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Completada, Rechazada

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
