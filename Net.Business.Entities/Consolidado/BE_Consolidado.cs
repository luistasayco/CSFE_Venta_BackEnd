using System;
using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class BE_Consolidado: EntityBase
    {
        public int idconsolidado { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechahora { get; set; }
        public string usuario { get; set; }
        public bool flgestado { get; set; }
        public List<BE_ConsolidadoPedido> ListaConsolidadoPedido { get; set; }
    }
}
