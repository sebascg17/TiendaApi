namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Puede ser un usuario con varios roles
        public List<string> Roles { get; set; } = new List<string>();
    }
}
