using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaApi.Models
{
    public class Referido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioReferenteId { get; set; } // quien refirió
        [ForeignKey(nameof(UsuarioReferenteId))]
        public Usuario UsuarioReferente { get; set; } = null!;

        [Required]
        public int UsuarioReferidoId { get; set; } // el nuevo usuario
        [ForeignKey(nameof(UsuarioReferidoId))]
        public Usuario UsuarioReferido { get; set; } = null!;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Validado, Rechazado

        public ICollection<Comision>? Comisiones { get; set; }
    }
}
