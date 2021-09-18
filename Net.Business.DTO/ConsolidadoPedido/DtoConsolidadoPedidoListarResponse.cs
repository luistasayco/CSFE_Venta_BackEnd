using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net.Business.Entities;


namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoListarResponse
    {
        public IEnumerable<DtoConsolidadoPedidoResponse> ListaConsolidadoPedido { get; set; }

        public DtoConsolidadoPedidoListarResponse RetornarListaConsolidadoPedido(IEnumerable<BE_ConsolidadoPedido> listaArticulos)
        {
            IEnumerable<DtoConsolidadoPedidoResponse> lista = (
                from value in listaArticulos
                select new DtoConsolidadoPedidoResponse
                {
                    idconsolidado = value.idconsolidado,
                    nomalmacen = value.pedido.nomalmacen,
                    nompaciente = value.pedido.nompaciente,
                    codatencion = value.pedido.codatencion,
                    codpedido = value.codpedido,
                    tipopedido = value.pedido.tipopedido
                }
            );

            return new DtoConsolidadoPedidoListarResponse() { ListaConsolidadoPedido = lista };
        }
    }
}
