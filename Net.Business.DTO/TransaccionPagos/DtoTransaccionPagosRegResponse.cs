using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoTransaccionPagosRegResponse
    {

        public string codVenta { get; set; }
        public string nroOperacion { get; set; }
        public string nroReferencia { get; set; }
        public string nroTarjeta { get; set; }
        public string monto { get; set; }
        public string currencyCode { get; set; }
        public string nroTerminal { get; set; }
        public string trama { get; set; }
        public string codtipotransaccion { get; set; }
        public int regcreateusuario { get; set; }

        public BE_TransaccionPagos RetornarDatos()
        {

            var data = new BE_TransaccionPagos()
            {
                //idoperacion = this.nroOperacion,
                codventa = this.codVenta,
                codtipotransaccion = codtipotransaccion,
                codterminal = nroTerminal,
                codreferencial = nroReferencia,
                numeroTarjeta = nroTarjeta,
                dispositivo = "Izipay",
                monto = monto,
                moneda = currencyCode,
                trama = trama,
                regcreateusuario=regcreateusuario
            };

            return data;

        }

    }
}
