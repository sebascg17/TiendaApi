using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Transacciones
{
    public class TransaccionCreateDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string Tipo { get; set; } = "Recarga"; // Recarga, Retiro

        [Required]
        public decimal Monto { get; set; }

        public string? Referencia { get; set; }
    }
}
