namespace TiendaApi.DTOs.Categorias
{
    public class CategoriaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int? TiendaId { get; set; }
    }
}
