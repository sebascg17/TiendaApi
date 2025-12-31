using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TiendaApi.Models
{
    public class DireccionUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = "Casa"; // Ej: "Casa", "Trabajo"

        [Required, MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Referencia { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }
        
        [MaxLength(100)]
        public string? Departamento { get; set; }
        
        [MaxLength(100)]
        public string? Pais { get; set; }
        
        [MaxLength(100)]
        public string? Barrio { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public bool EsPrincipal { get; set; } = false;

        [ForeignKey("UsuarioId")]
        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
