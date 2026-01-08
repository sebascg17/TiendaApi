using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class TipoTienda
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        public string? Icono { get; set; } // Ej: "pi-shopping-bag", "pi-briefcase"
        
        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;

        // 🔗 Relación con Tiendas
        public ICollection<Tienda>? Tiendas { get; set; }
    }
}
