using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoConveniosListaPrecioModificar
    {
        public int idconvenio { get; set; }
        public string codalmacen { get; set; }
        public string tipomovimiento { get; set; }
        public string codtipocliente { get; set; }
        public string codcliente { get; set; }
        public string codpaciente { get; set; }
        public string codaseguradora { get; set; }
        public string codcia { get; set; }
        public string moneda { get; set; }
        public int pricelist { get; set; }
        public int regcreateidusuario { get; set; }

        public BE_ConveniosListaPrecio RetornaConveniosListaPrecio()
        {
            return new BE_ConveniosListaPrecio
            {
                idconvenio = this.idconvenio,
                codalmacen = this.codalmacen,
                tipomovimiento = this.tipomovimiento,
                codtipocliente = codtipocliente,
                codcliente = this.codcliente,
                codpaciente = this.codpaciente,
                codaseguradora = this.codaseguradora,
                codcia = this.codcia,
                moneda = this.moneda,
                pricelist = this.pricelist,
                regcreateidusuario = this.regcreateidusuario
            };
        }
    }
}
