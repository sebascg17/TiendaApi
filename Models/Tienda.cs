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

        [MaxLength(50)]
        public string? DocumentoIdentidad { get; set; } // NIT / Cédula

        [MaxLength(100)]
        public string? Subdominio { get; set; } // Ej: tienda.sbenix.com

        [MaxLength(100)]
        public string? Slug { get; set; } // Ej: /tiendas/nombre-tienda

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

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

        [MaxLength(100)]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        // Detalles de Dirección Pro
        [MaxLength(50)]
        public string? Bloque { get; set; }
        [MaxLength(50)]
        public string? Apto { get; set; }
        [MaxLength(50)]
        public string? Torre { get; set; }
        [MaxLength(300)]
        public string? Referencia { get; set; }

        // Horarios Desglosados
        public int? HoraApertura { get; set; } // 0-23
        public int? MinutoApertura { get; set; } // 0-59
        public int? HoraCierre { get; set; }
        public int? MinutoCierre { get; set; }

        // Clasificación
        public int? TipoTiendaId { get; set; }
        public TipoTienda? TipoTienda { get; set; }

        // 🔗 Navegación
        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }

        public ICollection<Producto>? Productos { get; set; }
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public ICollection<Categoria>? Categorias { get; set; }
    }
}
