using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class Permiso
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } // Ej: "VerUsuarios", "CrearTienda"

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}
