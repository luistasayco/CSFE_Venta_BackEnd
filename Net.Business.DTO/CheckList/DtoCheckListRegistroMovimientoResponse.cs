using System;

namespace Net.Business.DTO
{
    public class DtoCheckListRegistroMovimientoResponse
    {
        public int ide_actividad { get; set; }
        public string cod_atencion { get; set; }
        public DateTime fec_registro { get; set; }
        public string dsc_nombre { get; set; }
        public int est_respuesta { get; set; }
        public int num_orden { get; set; }
        public string nombre { get; set; }
    }
}
