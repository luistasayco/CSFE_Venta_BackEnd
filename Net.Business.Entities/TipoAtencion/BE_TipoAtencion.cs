using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_TipoAtencion
    {
        public string codtabla { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        //public string valor { get; set; }
        public bool bvalor { get; set; }
        public DateTime? fec_registro { get; set; }

    }
}
