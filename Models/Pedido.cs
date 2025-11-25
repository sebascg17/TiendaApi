using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public enum EstadoPedido
    {
        Pendiente,
        Confirmado,
        Enviado,
        Entregado,
        Cancelado
    }

    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tienda")]
        public int TiendaId { get; set; }

        [ForeignKey("Cliente")]
        public int? ClienteId { get; set; } // null si compra como invitado

        [MaxLength(150)]
        public string ClienteNombre { get; set; } = string.Empty;

        [MaxLength(150)]
        public string ClienteEmail { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? ClienteTelefono { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

        [MaxLength(50)]
        public string MetodoPago { get; set; } = "Efectivo";

        [MaxLength(300)]
        public string? DireccionEntrega { get; set; }

        [ForeignKey("Repartidor")]
        public int? RepartidorId { get; set; } // si aplica

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // 🔗 Relaciones
        public Tienda? Tienda { get; set; }
        public Usuario? Cliente { get; set; }
        public Usuario? Repartidor { get; set; }
        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
