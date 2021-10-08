using Net.Business.Entities;
using System;

namespace Net.Business.DTO
{
    public class DtoAseguradoraxProductoInsert
    {
        public string codaseguradora { get; set; }
        public string codproducto { get; set; }
        public DateTime fec_registro { get; set; }
        public int cod_tipoatencion_mae { get; set; }

        public BE_AseguradoraxProducto RetornaAseguradoraxProductoInsert()
        {
            return new BE_AseguradoraxProducto
            {
                codaseguradora = this.codaseguradora,
                codproducto = this.codproducto,
                fec_registro = this.fec_registro,
                cod_tipoatencion_mae = this.cod_tipoatencion_mae
            };
        }
    }
}
