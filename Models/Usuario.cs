using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Phone]
        public string? Telefono { get; set; }

        [MaxLength(200)]
        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        [MaxLength(100)]
        public string? Pais { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [Required]
        public string Estado { get; set; } = "Activo"; // Activo, Inactivo, Suspendido

        public string? VerificationToken { get; set; }
        public bool IsVerified { get; set; } = false;

        [Range(0, double.MaxValue)]
        public decimal Saldo { get; set; } = 0;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // 🔗 Relaciones
        public ICollection<UsuarioRol>? UsuarioRoles { get; set; }
        public ICollection<Tienda>? Tiendas { get; set; }
        public ICollection<Transaccion>? Transacciones { get; set; }
        public ICollection<Referido>? Referidos { get; set; }
        public ICollection<Comision>? Comisiones { get; set; }
        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<Contacto>? Contactos { get; set; }
    }
}
