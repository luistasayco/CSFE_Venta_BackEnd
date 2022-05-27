using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IConsolidadoRepository : IRepositoryBase<BE_Consolidado>
    {
        Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoPorFiltro(DateTime fecinicio, DateTime fecfin);
        Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoCerradoPorFiltro(DateTime fecinicio, DateTime fecfin);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPicking(string codpedido, string codproducto);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPickingPorId(int idconsolidado);
        Task<ResultadoTransaccion<BE_ConsolidadoPedido>> GetListDetalleConsolidado(int idconsolidado);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetConsolidadoPedidoPickingPorId(int idconsolidadopicking);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> EliminarConsolidadoPicking(BE_ConsolidadoPedidoPicking value);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> RegistrarConsolidadoPicking(BE_ConsolidadoPedidoPicking value);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ModificarConsolidadoPickingMasivo(List<BE_ConsolidadoPedidoPicking> value);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ModificarConsolidadoPicking(BE_ConsolidadoPedidoPicking value);
        Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ModificarEstadoPedido(BE_ConsolidadoPedidoPicking value);
        Task<ResultadoTransaccion<BE_ConsolidadoSolicitud>> GetListConsolidadoSolicitudGet(DateTime fechaInicio, DateTime fechaFin);
        Task<ResultadoTransaccion<MemoryStream>> GenerarConsolidadoSolicitudPrint(DateTime fechaInicio, DateTime fechaFin);
    }
}
