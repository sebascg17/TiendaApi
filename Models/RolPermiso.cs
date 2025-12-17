using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class RolPermiso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RolId { get; set; }

        [Required]
        public int PermisoId { get; set; }

        // Navegación
        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; }
    }
}
