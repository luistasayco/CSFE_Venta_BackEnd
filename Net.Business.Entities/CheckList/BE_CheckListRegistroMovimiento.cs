using System;

namespace Net.Business.Entities
{
    public class BE_CheckListRegistroMovimiento
    {
        public int ide_registro_mov { get; set; }
        public int ide_tarea { get; set; }
        public int ide_actividad { get; set; }
        public string cod_atencion { get; set; }
        public int est_respuesta { get; set; }
        public int usr_crea { get; set; }
        public DateTime fec_registro { get; set; }
        public string dsc_nombre { get; set; }
        public string flg_estado { get; set; }
        public int flg_revisado { get; set; }
        public string cod_presupuesto { get; set; }
        public int num_orden { get; set; }
        public string nombre { get; set; }
    }
}
