using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_ComprobantesBaja
    {
        public string cod_comprobantee { get; set; }
        public string cod_comprobante { get; set; }
        public string cod_sistema { get; set; }
        public string cod_empresa { get; set; }
        public string cod_tipocompsunat { get; set; }
        public DateTime fec_baja { get; set; }
        public DateTime fec_emisioncomp { get; set; }
        public string dsc_motivobaja { get; set; }
        public int ide_compbaja { get; set; }

    }
}
