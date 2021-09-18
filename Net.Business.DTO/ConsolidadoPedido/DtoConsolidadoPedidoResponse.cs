using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoResponse
    {
        public int idconsolidadopedido { get; set; }
        public int idconsolidado { get; set; }
        public string codpedido { get; set; }
        public string codproducto { get; set; }
        public decimal cantidad { get; set; }
        public bool flgpicking { get; set; }
        public decimal cantidadpicking { get; set; }
        public string nomalmacen { get; set; }
        public string nompaciente { get; set; }
        public string codatencion { get; set; }
        public string tipopedido { get; set; }

    }
}
