using TiendaApi.Models;

namespace TiendaApi.DTOs.Pedidos
{
    public class PedidoReadDto
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente; // 👈 aquí también
        public List<PedidoDetalleReadDto> Detalles { get; set; } = new();
    }
}