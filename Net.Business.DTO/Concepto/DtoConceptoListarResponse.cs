using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConceptoListarResponse
    {
        public IEnumerable<DtoConceptoResponse> ListaConceptoResponse { get; set; }


        public DtoConceptoListarResponse RetornarConceptoListar(IEnumerable<BE_Concepto> listaConcepto)
        {
            IEnumerable<DtoConceptoResponse> lista = (
                from value in listaConcepto
                select new DtoConceptoResponse
                {
                    codconcepto = value.codconcepto,
                    descripcion = value.descripcion,
                    codtipoconcepto = value.codtipoconcepto,
                    estado = value.estado
                }
            );

            return new DtoConceptoListarResponse() { ListaConceptoResponse = lista };
        }
    }
}
