using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConceptoRegistrar: EntityBase
    {
        public string descripcion { get; set; }
        public int codtipoconcepto { get; set; }

        public BE_Concepto RetornaConcepto()
        {
            return new BE_Concepto
            {
                descripcion = this.descripcion,
                codtipoconcepto = this.codtipoconcepto,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
