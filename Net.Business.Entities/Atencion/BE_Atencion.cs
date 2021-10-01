using System;

namespace Net.Business.Entities
{
    public class BE_Atencion
    {
        public string nombrepaciente { get; set; }
        public string atencion { get; set; }
        public string estadopaciente { get; set; }
        public string finicio { get; set; }
        public string codpaciente { get; set; }
    }

    public class BE_AtencionPaquete
    {
        public int idpaqueteclinico { get; set; }
        public string codatencion { get; set; }
        public DateTime fecasignacion { get; set; }
        public decimal importepaquete { get; set; }
        public int idpaqueteclinico_bk { get; set; }
        public string descripcion { get; set; }
    }
}
