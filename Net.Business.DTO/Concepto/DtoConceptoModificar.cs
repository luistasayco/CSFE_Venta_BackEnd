using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConceptoModificar : EntityBase
    {
        public int codconcepto { get; set; }
        public string descripcion { get; set; }
        public int codtipoconcepto { get; set; }
        public bool estado { get; set; }

        public BE_Concepto RetornaConcepto()
        {
            return new BE_Concepto
            {
                codconcepto = this.codconcepto,
                descripcion = this.descripcion,
                codtipoconcepto = this.codtipoconcepto,
                estado = this.estado,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
