using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_Concepto: EntityBase
    {
        public int codconcepto { get; set; }
        public string descripcion { get; set; }
        public int codtipoconcepto { get; set; }
        public string desctipoconcepto { get; set; }
        public bool estado { get; set; }
    }
}
