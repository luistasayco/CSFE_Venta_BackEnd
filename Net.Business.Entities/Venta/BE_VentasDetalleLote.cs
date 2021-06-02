using System;

namespace Net.Business.Entities
{
    public class BE_VentasDetalleLote
    {
        public int idlote { get; set; }
        public string coddetalle { get; set; }
        public string lote { get; set; }
        public DateTime fechavencimiento { get; set; }
        public decimal cantidad { get; set; }
    }
}
