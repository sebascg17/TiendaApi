namespace TiendaApi.DTOs.Productos
{
    public class ProductoReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; }
        public int Stock { get; set; }
        public int TiendaId { get; set; }

        // Categorías asociadas al producto
        public List<CategoriaSimpleDto> Categorias { get; set; } = new();
    }

    public class CategoriaSimpleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
