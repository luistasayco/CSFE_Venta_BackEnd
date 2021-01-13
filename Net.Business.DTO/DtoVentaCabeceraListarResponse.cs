using Net.Business.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Net.Business.DTO
{
    public class DtoVentaCabeceraListarResponse
    {
        public List<DtoVentaCabeceraResponse> ListaVentaCabecera { get; set; }

        public DtoVentaCabeceraListarResponse RetornarListaVentaCabecera(List<BE_VentaCabecera> listaArticulos)
        {
            List<DtoVentaCabeceraResponse> lista = (
                from value in listaArticulos
                select new DtoVentaCabeceraResponse
                {
                    CodVenta = value.CodVenta,
                    codalmacen = value.codalmacen,
                    tipomovimiento = value.tipomovimiento,
                    codempresa = value.codempresa,
                    codtipocliente = value.codtipocliente,
                    nombretipocliente = value.nombretipocliente,
                    codatencion = value.codatencion,
                    codcomprobante = value.codcomprobante,
                    estado = value.estado,
                    nombre = value.nombre,
                    montopaciente = value.montopaciente,
                    montoaseguradora = value.montoaseguradora,
                    flg_gratuito = value.flg_gratuito,
                    fechagenera = value.fechagenera,
                    fechaemision = value.fechaemision,
                    codpaciente = value.codpaciente,
                    codcliente = value.codcliente,
                    codpedido = value.codpedido
                }
                ).ToList();

            return new DtoVentaCabeceraListarResponse() { ListaVentaCabecera = lista };
        }
    }
}
