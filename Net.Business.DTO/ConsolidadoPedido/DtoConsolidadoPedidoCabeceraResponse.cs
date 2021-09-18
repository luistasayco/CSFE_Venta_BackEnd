using System;
using System.Collections.Generic;
using System.Text;
using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoCabeceraResponse
    {
        public int idconsolidado { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechahora { get; set; }
        public string usuario { get; set; }
        public bool flgestado { get; set; }
        //public DtoConsolidadoPedidoCabeceraResponse RetornaDtoConsolidadoPedidoCabeceraResponse(BE_Consolidado value)
        //{
        //    return new DtoConsolidadoPedidoCabeceraResponse()
        //    {
        //        idconsolidado = value.idconsolidado,
        //        fecha = value.fecha,
        //        fechahora = fechahora
        //    };
        //}
    }
}
