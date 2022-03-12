using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoCheckListRegistroMovimientoUpdate
    {
        public string data { get; set; }
        public string comentario { get; set; }

        //public BE_CheckListRegistroMovimiento RetornaCheckListRegistroMovimientoUpdate()
        //{
        //    return new BE_CheckListRegistroMovimiento
        //    {
        //        ide_actividad = this.ide_actividad,
        //        cod_atencion = this.cod_atencion,
        //        ide_tarea = this.ide_tarea
        //    };
        //}
    }
}
