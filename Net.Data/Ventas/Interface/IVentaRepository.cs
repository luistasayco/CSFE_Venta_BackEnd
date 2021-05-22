using Net.Business.Entities;
using Net.Connection;
using System;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentaRepository : IRepositoryBase<BE_VentasCabecera>
    {
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin);
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaPorCodVenta(string codventa);
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPendientePorFiltro(DateTime fecha);
        Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaChequea1MesPorFiltro(string codpaciente, int cuantosmesesantes);
        Task<ResultadoTransaccion<BE_VentasCabecera>> ModificarVentaCabeceraEnvioPiso(BE_VentasCabecera value);
        Task<ResultadoTransaccion<BE_VentasCabecera>> RegistrarVentaCabecera(BE_VentasCabecera value);
        Task<ResultadoTransaccion<Boolean>> GetGastoCubiertoPorFiltro(string codaseguradora, string codproducto, int tipoatencion);
    }
}
