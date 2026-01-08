using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.DTOs.Tiendas
{
    public class TipoTiendaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Activo { get; set; }
    }

    public class TipoTiendaCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
