namespace TiendaApi.DTOs.Productos
{
    public class ProductoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int TiendaId { get; set; }

        // Permite asignar varias categorías al producto
        public List<int> CategoriasIds { get; set; } = new();
    }
}
