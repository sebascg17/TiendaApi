using System.Text.Json.Serialization;

namespace TiendaApi.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string? Apellido { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("ciudad")]
        public string? Ciudad { get; set; }

        [JsonPropertyName("pais")]
        public string? Pais { get; set; }

        [JsonPropertyName("departamento")]
        public string? Departamento { get; set; }

        [JsonPropertyName("barrio")]
        public string? Barrio { get; set; }

        [JsonPropertyName("fechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [JsonPropertyName("fotoPerfilUrl")]
        public string? FotoPerfilUrl { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("currentPassword")]
        public string? CurrentPassword { get; set; }
        public string? ModoTema { get; set; }

        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }
    }
}
