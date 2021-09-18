using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoCheckListRegistroMovimientoVerificarResponse
    {
        public IEnumerable<DtoCheckListRegistroMovimientoResponse> ListaCheckListRegistroMovimientoVerificar { get; set; }

        public DtoCheckListRegistroMovimientoVerificarResponse RetornarCheckListRegistroMovimientoVerificar(IEnumerable<BE_CheckListRegistroMovimiento> listaCheckListRegistroMovimientoVerificar)
        {
            IEnumerable<DtoCheckListRegistroMovimientoResponse> lista = (
                from value in listaCheckListRegistroMovimientoVerificar
                select new DtoCheckListRegistroMovimientoResponse
                {
                    ide_actividad = value.ide_actividad,
                    cod_atencion = value.cod_atencion,
                    fec_registro = value.fec_registro,
                    dsc_nombre = value.dsc_nombre,
                    est_respuesta = value.est_respuesta,
                    num_orden = value.num_orden,
                    nombre = value.nombre
                }
            );

            return new DtoCheckListRegistroMovimientoVerificarResponse() { ListaCheckListRegistroMovimientoVerificar = lista };
        }
    }
}
