using Net.Business.Entities;
using Net.Connection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IConsolidadoPedidoRepository : IRepositoryBase<BE_ConsolidadoPedidoDetalle>
    {
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>> CreateConsolidadoPedido(BE_ConsolidadoPedidoXml value);

        Task<ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>> UpdateConsolidadoPedido(BE_ConsolidadoPedidoXml value);

        Task<ResultadoTransaccion<BE_Consolidado>> DeleteConsolidado(BE_Consolidado value);

        Task<ResultadoTransaccion<BE_Consolidado>> CloseConsolidado(BE_Consolidado value);

        Task<ResultadoTransaccion<BE_ConsolidadoPedido>> DeleteConsolidadoPedidoPorPedido(BE_ConsolidadoPedido value);

        Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosSinConsolidarPorFiltro(DateTime fechainicio, DateTime fechafin, string codtipopedido, string codpedido);

        Task<ResultadoTransaccion<BE_ConsolidadoPedido>> GetListConsolidadoPedido(int idconsolidado);

        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPedidoPickingPorIdConsolidado(int idconsolidado);

        Task<ResultadoTransaccion<BE_ConsolidadoPedidoProducto>> GetListConsolidadoPedidoProducto(int idconsolidado, string codpedido);

        Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoCabecera(DateTime fechainicio, DateTime fechafin, int idconsolidado);

        Task<ResultadoTransaccion<MemoryStream>> GenerarConsolidadoPedidoPrint(int idconsolidado);
    }
}
