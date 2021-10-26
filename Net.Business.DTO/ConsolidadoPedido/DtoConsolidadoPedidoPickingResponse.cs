using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoPickingResponse
    {
        public int idconsolidadopicking { get; set; }
        public int idconsolidado { get; set; }
        public string codpedido { get; set; }
        public string codproducto { get; set; }
        public decimal cantidad { get; set; }
        public decimal cantidadpicking { get; set; }
        public string lote { get; set; }
        public DateTime? fechavencimiento { get; set; }
        public string codalmacen { get; set; }
        public int ubicacion { get; set; }
        public string codusuarioapu { get; set; }
        public int estado { get; set; }

    }
}
