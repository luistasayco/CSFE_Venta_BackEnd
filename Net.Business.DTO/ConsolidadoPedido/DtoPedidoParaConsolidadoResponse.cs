using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoPedidoParaConsolidadoResponse
    {
        public int Id { get; set; }
        public string codigopedido { get; set; }
        public string codigoatencion { get; set; }
        public string cama { get; set; }    
        public string tipopedido { get; set; }
        public string fechagenerada { get; set; }
        public string nombrealmacen { get; set; }
        public string nombrepaciente { get; set; }
        public string nombrecentro{ get; set; }

    }
}
