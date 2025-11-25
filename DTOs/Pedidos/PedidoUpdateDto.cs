using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaApi.DTOs.Pedidos
{
    public class PedidoUpdateDto
    {
        public string Cliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
    }
}