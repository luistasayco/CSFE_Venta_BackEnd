using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoProcesarTransaccionRequest
    {
        public string aplicacion { get; set; }
        public string transaccion { get; set; }
        public string amount { get; set; }
        public string currency_code { get; set; }
        public string data_adicional { get; set; }
        public string codventa { get; set; }
        public int regcreateusuario { get; set; }

        public BE_ProcesarTransaccionPagoRequest RetornarPagosDatos()
        {
            var data = new BE_ProcesarTransaccionPagoRequest()
            {
                ecr_aplicacion = aplicacion,
                ecr_transaccion = transaccion,
                ecr_amount = amount,
                ecr_currency_code = currency_code
            };
            return data;
        }

        public BE_ProcesarTransaccionAnularRequest RetornarAnularDatos()
        {

            var data = new BE_ProcesarTransaccionAnularRequest()
            {
                ecr_aplicacion = aplicacion,
                ecr_transaccion = transaccion,
                ecr_amount = amount,
                ecr_currency_code = currency_code,
                ecr_data_adicional = data_adicional
            };

            return data;
        }


    }
}
