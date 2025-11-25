using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class UsuarioRol
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [Required, ForeignKey("Rol")]
        public int RolId { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        // 🔗 Navegación
        public Usuario? Usuario { get; set; }
        public Rol? Rol { get; set; }
    }
}
