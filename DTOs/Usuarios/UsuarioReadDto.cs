namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public string? Pais { get; set; }
        public string? Barrio { get; set; }
        public bool HasPassword { get; set; }
    }
}
