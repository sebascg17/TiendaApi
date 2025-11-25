namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        // En caso de que se quieran actualizar los roles
        public List<string>? Roles { get; set; }
    }
}
