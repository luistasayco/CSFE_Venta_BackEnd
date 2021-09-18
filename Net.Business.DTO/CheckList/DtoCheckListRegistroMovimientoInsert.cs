using Net.Business.Entities;
using System;

namespace Net.Business.DTO
{
    public class DtoCheckListRegistroMovimientoInsert
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

        public BE_CheckListRegistroMovimiento RetornaCheckListRegistroMovimientoInsert()
        {
            return new BE_CheckListRegistroMovimiento
            {
                ide_registro_mov = this.ide_registro_mov,
                ide_tarea = this.ide_tarea,
                ide_actividad = this.ide_actividad,
                cod_atencion = this.cod_atencion,
                est_respuesta = this.est_respuesta,
                usr_crea = this.usr_crea,
                fec_registro = this.fec_registro,
                dsc_nombre = this.dsc_nombre,
                flg_estado = this.flg_estado,
                flg_revisado = this.flg_revisado
            };
        }
    }
}
