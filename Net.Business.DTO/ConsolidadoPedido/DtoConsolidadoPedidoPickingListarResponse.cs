using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net.Business.Entities;


namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoPickingListarResponse
    {
        public IEnumerable<DtoConsolidadoPedidoPickingResponse> ListaConsolidadoPedidoPicking { get; set; }

        public DtoConsolidadoPedidoPickingListarResponse RetornarListaConsolidadoPedidoPicking(IEnumerable<BE_ConsolidadoPedidoPicking> listaArticulos)
        {
            IEnumerable<DtoConsolidadoPedidoPickingResponse> lista = (
                from value in listaArticulos
                select new DtoConsolidadoPedidoPickingResponse
                {
                    idconsolidadopicking = value.idconsolidadopicking,
                    idconsolidado = value.idconsolidado,
                    codproducto = value.codproducto,
                    codpedido = value.codpedido,
                    cantidad = value.cantidad,
                    cantidadpicking = value.cantidadpicking,
                    codalmacen = value.codalmacen,
                    fechavencimiento = value.fechavencimiento,
                    lote = value.lote,
                    codusuarioapu = value.codusuarioapu,
                    ubicacion = value.ubicacion,
                    estado = value.estado
                }
            );

            return new DtoConsolidadoPedidoPickingListarResponse() { ListaConsolidadoPedidoPicking = lista };
        }
    }
}
