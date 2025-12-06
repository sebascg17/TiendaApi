namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        // Rol seleccionado por el usuario (Cliente, Tendero)
        public string Rol { get; set; } = "Cliente";

        // Campos opcionales para Tendero
        public string? NombreTienda { get; set; }
        public string? DireccionTienda { get; set; }

        // Puede ser un usuario con varios roles (Legacy)
        public List<string> Roles { get; set; } = new List<string>();
    }
}
