using Net.Business.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoCabeceraListarResponse
    {
        public IEnumerable<DtoConsolidadoPedidoCabeceraResponse> ListaConsolidadoPedidoCabecera { get; set; }

        public DtoConsolidadoPedidoCabeceraListarResponse RetornarListaConsolidadoPedido(IEnumerable<BE_Consolidado> listaArticulos)
        {
            IEnumerable<DtoConsolidadoPedidoCabeceraResponse> lista = (
                from value in listaArticulos
                select new DtoConsolidadoPedidoCabeceraResponse
                {
                    idconsolidado = value.idconsolidado,
                    fecha = value.fecha,
                    fechahora = value.fechahora,
                    flgestado = value.flgestado,
                    usuario = value.usuario
                }
            );

            return new DtoConsolidadoPedidoCabeceraListarResponse() { ListaConsolidadoPedidoCabecera = lista };
        }
    }
}
