using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentaRepository : IRepositoryBase<BE_VentasCabecera>
    {
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin);
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaPorCodVenta(string codventa);
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPendientePorFiltro(DateTime fecha);
        Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaChequea1MesPorFiltro(string codpaciente, int cuantosmesesantes);
    }
}
