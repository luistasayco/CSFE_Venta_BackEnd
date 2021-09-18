using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_HospitalDocumento
    {
        public int id_documento { get; set; }
        public string nombre { get; set; }
        public DateTime fecha_creacion { get; set; }
        public string descripcion_doc { get; set; }
        public string tipo { get; set; }
        public int estado { get; set; }
        public int flg_estadodip { get; set; }
        public int tipo_doc { get; set; }
        public string dsc_usuario { get; set; }
    }
}
