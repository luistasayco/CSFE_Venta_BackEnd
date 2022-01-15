using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_ProcesarTransaccionResponse
    {
        public string approval_code { get; set; }
        public string read_type { get; set; }
        public string amount { get; set; }
        public string response_code { get; set; }
        public string account_type { get; set; }
        public string ecr_amount { get; set; }
        public string print_data { get; set; }
        public string id_autorizacion { get; set; }
        public string type_transaccion { get; set; }
        public string merchant_id { get; set; }
        public string message { get; set; }
        public string currency_code { get; set; }
        public string card_id { get; set; }
        public string cod_adquiriente { get; set; }

        public string ecr_transaccion { get; set; }
        public string flag_pide_firma { get; set; }
        public string date_time { get; set; }

        public string ecr_aplicacion { get; set; }
        public string ecr_currency_code { get; set; }
        public string client_name { get; set; }
        public string card { get; set; }
        //public string resp_host { get; set; }

    }
}
