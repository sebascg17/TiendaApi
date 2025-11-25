using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; } = string.Empty; // Admin, Vendedor, Cliente, Repartidor

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // 🔗 Relaciones
        public ICollection<UsuarioRol>? UsuarioRoles { get; set; }
    }
}
