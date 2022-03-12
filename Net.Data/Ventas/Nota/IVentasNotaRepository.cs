using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentasNotaRepository
    {
        Task<ResultadoTransaccion<BE_VentasNota>> GetNotaPorFiltro(string buscar, int key, int numerolineas, int orden, string tipo);
        Task<ResultadoTransaccion<BE_VentasNota>> GetNotaCabeceraPorFiltro(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin, string codnota, string anombredequien);
        Task<ResultadoTransaccion<BE_VentasNota>> GetNotaCabeceraPorCodNota(string codnota);
        //Task<ResultadoTransaccion<BE_VentasNota>> CancelarCredito(string codnota, string usuario);
        Task<ResultadoTransaccion<BE_VentasNota>> EliminarNotaCredito(string codnota, string usuario, int idusuario);
        //Task<ResultadoTransaccion<BE_VentasNota>> DarBajaCredito(string codnota, string usuario, string observacion, int idusuario);
    }
}
