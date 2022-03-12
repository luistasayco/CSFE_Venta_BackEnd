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
        public bool flg_revisado { get; set; }
        public string cod_presupuesto { get; set; }
        public int num_orden { get; set; }
        public string nombre { get; set; }
    }

    public class BE_CheckListCorreoDestinatario
    {
        public string destinatario { get; set; }
    }

    public class BE_CheckListReporte
    {
        public string NombrePaciente { get; set; }
        public string cama { get; set; }
        public string codatencion { get; set; }
        public string Descripcion_Diagnostico { get; set; }
        public DateTime? fec_registro { get; set; }
        public int num_orden { get; set; }
        public string grupo { get; set; }
        public string Descripcion_Check { get; set; }
        public string SI { get; set; }
        public string NO { get; set; }
        public string NO_APLICA { get; set; }
        public string Usuario { get; set; }
        public string recepcion { get; set; }
        public DateTime? fecha_recepcion { get; set; }
        public string auditoria_recepcion { get; set; }
        public string elabcuentapac { get; set; }
        public DateTime? fecha_elabcuentapac { get; set; }
        public string auditoria_elabcuentapac { get; set; }
        public string auditoria_reccuentaSAP { get; set; }
        public DateTime? fecha_reccuentaSAP { get; set; }
    }
}
