using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Comision
    {
        [Key]
        public int Id { get; set; }

        public int? PedidoId { get; set; }
        [ForeignKey(nameof(PedidoId))]
        public Pedido? Pedido { get; set; }

        public int UsuarioDestinoId { get; set; } // quien recibe la comisión
        [ForeignKey(nameof(UsuarioDestinoId))]
        public Usuario UsuarioDestino { get; set; } = null!;

        public int? UsuarioOrigenId { get; set; } // quien generó la venta o referido
        [ForeignKey(nameof(UsuarioOrigenId))]
        public Usuario? UsuarioOrigen { get; set; }

        [StringLength(50)]
        public string Tipo { get; set; } = "Venta"; // Venta, Referral, PlatformFee, DeliveryFee

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public int? ReferidoId { get; set; }
        [ForeignKey(nameof(ReferidoId))]
        public Referido? Referido { get; set; }

        public int? TransaccionId { get; set; }
        [ForeignKey(nameof(TransaccionId))]
        public Transaccion? Transaccion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
