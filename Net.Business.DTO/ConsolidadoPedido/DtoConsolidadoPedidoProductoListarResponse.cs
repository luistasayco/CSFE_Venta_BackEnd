using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net.Business.Entities;


namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoProductoListarResponse
    {
        public IEnumerable<DtoConsolidadoPedidoProductoResponse> ListaConsolidadoPedidoProducto { get; set; }

        public DtoConsolidadoPedidoProductoListarResponse RetornarListaConsolidadoPedido(IEnumerable<BE_ConsolidadoPedidoProducto> listaArticulos)
        {
            IEnumerable<DtoConsolidadoPedidoProductoResponse> lista = (
                from value in listaArticulos
                select new DtoConsolidadoPedidoProductoResponse
                {
                    codproducto = value.codproducto,
                    despro = value.despro
                }
            );

            return new DtoConsolidadoPedidoProductoListarResponse() { ListaConsolidadoPedidoProducto = lista };
        }
    }
}
