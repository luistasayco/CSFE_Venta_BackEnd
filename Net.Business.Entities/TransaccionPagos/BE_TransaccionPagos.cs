using System;

namespace Net.Business.Entities
{
    public class BE_TransaccionPagos
    {

        
        public int idtransaccionpagos { get; set; }
        public string codventa { get; set; }
        public string idtransaccion { get; set; }
        public string codtipotransaccion { get; set; }
        public string codterminal { get; set; }
        public string codreferencial { get; set; }
        public string tipotarjeta { get; set; }
        public string numeroTarjeta { get; set; }
        public string dispositivo { get; set; }
        public string monto { get; set; }
        public string moneda { get; set; }
        public string trama { get; set; }
        public DateTime regcreate { get; set; }
        public DateTime regupdate { get; set; }
        public int regcreateusuario { get; set; }
        public int regupdateusuario { get; set; }

    }
}
