using Net.Business.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IPlanillaRepository
    {
        Task<ResultadoTransaccion<string>> RegistrarPorUsuario(BE_Planilla value);
        Task<ResultadoTransaccion<BE_Planilla>> Modificar(BE_Planilla values);
        Task<ResultadoTransaccion<BE_Planilla>> Elminar(string numeroplanilla);
        Task<ResultadoTransaccion<BE_Planilla>> GetPlanillasPorFiltro(string buscar, int key, int numerolineas, int orden, string serie);
        Task<ResultadoTransaccion<BE_Planilla>> GetListaPlanillasPorFiltro(string buscar, int key, int numerolineas, int orden, string serie, string codcentro, string idusuario, string numeroPlanilla, string fechaInicio, string fechaFin);
        Task<ResultadoTransaccion<BE_ReportePlanilla>> GetReportePlanillaPorNumero(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares);
        Task<ResultadoTransaccion<BE_ReportePlanillaDetalle>> GetReporteDetallePlanillaPorNumero(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden);
        Task<ResultadoTransaccion<MemoryStream>> GenerarReportePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares);
        Task<ResultadoTransaccion<MemoryStream>> GenerarExcelPlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares);
        Task<ResultadoTransaccion<MemoryStream>> GenerarReporteDetallePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden);
        Task<ResultadoTransaccion<MemoryStream>> GenerarExcelDetallePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden);
    }
}
