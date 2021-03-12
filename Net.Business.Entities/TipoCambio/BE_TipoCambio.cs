using System;

namespace Net.Business.Entities
{
    public class BE_TipoCambio
    {
        public string codtipodecambio { get; set; }
        public DateTime fecha { get; set; }
        public decimal paralelocompra { get; set; }
        public decimal paraleloventa { get; set; }
        public decimal bancariocompra { get; set; }
        public decimal bancarioventa { get; set; }
    }
}
