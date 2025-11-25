namespace TiendaApi.DTOs.Productos
{
    public class ProductoUpdateDto
    {
        public int Id { get; set; }   // ⚡ Necesario en update
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }

        // Permite re-asignar categorías al producto
        public List<int> CategoriasIds { get; set; } = new();
    }
}
