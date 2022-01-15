using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConceptoEliminar : EntityBase
    {
        public int codconcepto { get; set; }

        public BE_Concepto RetornaConcepto()
        {
            return new BE_Concepto
            {
                codconcepto = this.codconcepto,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
