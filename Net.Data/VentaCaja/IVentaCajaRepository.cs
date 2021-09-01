using Net.Business.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentaCajaRepository
    {
        Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPorCodVenta(string codVenta);
        Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaDetallePorCodVenta(string codVenta);
        Task<ResultadoTransaccion<AsegContPaci>> GetRucConsultav2PorFiltro(string codPaciente, string codAseguradora, string codCia);
        Task<ResultadoTransaccion<B_MdsynPagos>> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden);
        Task<ResultadoTransaccion<object>> GetLimiteConsumoPersonalPorCodPersonal(string codPersonal);
        Task<ResultadoTransaccion<string>> GetWebServicesPorCodTabla(string codTabla);
        Task<ResultadoTransaccion<object>> GetCorrelativoConsulta();
        Task<ResultadoTransaccion<string>> ComprobantesRegistrar(BE_Comprobante value, string tipocomprobante, string correo, string codTipoafectacionigv, bool wFlg_electronico,string maquina);
        Task<ResultadoTransaccion<string>> GetDatoCardCodeConsulta(string tipoCliente, string codCliente);
        
    }
}
