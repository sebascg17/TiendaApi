using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Tienda
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Usuario")]
        public int UsuarioId { get; set; } // Propietario

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Subdominio { get; set; } // Ej: tienda.sbenix.com

        [MaxLength(100)]
        public string? Slug { get; set; } // Ej: /tiendas/nombre-tienda

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        public string? LogoUrl { get; set; }

        [MaxLength(20)]
        public string? ColorPrimario { get; set; } // formato HEX (#00FF00)

        [ForeignKey("Plan")]
        public int? PlanId { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(20)]
        public string Estado { get; set; } = "inactivo"; // activo, inactivo, pendiente

        [MaxLength(100)]
        public string? Pais { get; set; }

        [MaxLength(100)]
        public string? Departamento { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        [MaxLength(255)]
        public string? Direccion { get; set; }

        // 🔗 Navegación
        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }

        public ICollection<Producto>? Productos { get; set; }
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public ICollection<Categoria>? Categorias { get; set; }
    }
}
