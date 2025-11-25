namespace TiendaApi.DTOs.Pedidos
{
    // 🔹 Creación de pedido
    public class PedidoCreateDto
    {
        public string Cliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PedidoDetalleCreateDto> Detalles { get; set; } = new();
    }
}
