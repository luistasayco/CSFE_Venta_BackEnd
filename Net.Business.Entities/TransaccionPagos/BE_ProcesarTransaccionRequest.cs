using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_ProcesarTransaccionPagoRequest
    {
        public string ecr_aplicacion { get; set; }
        public string ecr_transaccion { get; set; }
        public string ecr_amount { get; set; }
        public string ecr_currency_code { get; set; }

    }

    public class BE_ProcesarTransaccionPagoIzipayRequest
    {
        public string ecr_aplicacion { get; set; }
        public string ecr_transaccion { get; set; }
        public string ecr_amount { get; set; }
        public string ecr_currency_code { get; set; }
        public string ecr_data_adicional { get; set; }
        public string id_autorizacion { get; set; }
        public string message { get; set; }
    }


    public class BE_ProcesarTransaccionAnularRequest
    {
        public string ecr_aplicacion { get; set; }
        public string ecr_transaccion { get; set; }
        public string ecr_amount { get; set; }
        public string ecr_currency_code { get; set; }
        public string ecr_data_adicional { get; set; }
        public string id_autorizacion { get; set; }
        public string message { get; set; }
    }

}
