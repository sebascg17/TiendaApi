namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
    }
}
