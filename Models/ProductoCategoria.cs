using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TiendaApi.Models
{
    public class ProductoCategoria
    {
        [Key, Column(Order = 0), ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Key, Column(Order = 1), ForeignKey("Categoria")]
        public int CategoriaId { get; set; }

        // 🔗 Relaciones
        public Producto? Producto { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
