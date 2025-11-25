using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaApi.Models;

namespace TiendaApi.DTOs.Pedidos
{
    public class PedidoUpdateEstadoDto
    {
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
    }
}