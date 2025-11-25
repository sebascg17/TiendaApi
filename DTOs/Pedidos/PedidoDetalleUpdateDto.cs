using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaApi.DTOs.Pedidos
{
    public class PedidoDetalleUpdateDto
    {
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}