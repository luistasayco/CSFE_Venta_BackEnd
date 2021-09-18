using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoCheckListRegistroMovimientoUpdate
    {
        public int ide_actividad { get; set; }
        public string cod_atencion { get; set; }
        public int ide_tarea { get; set; }

        public BE_CheckListRegistroMovimiento RetornaCheckListRegistroMovimientoUpdate()
        {
            return new BE_CheckListRegistroMovimiento
            {
                ide_actividad = this.ide_actividad,
                cod_atencion = this.cod_atencion,
                ide_tarea = this.ide_tarea
            };
        }
    }
}
