using Microsoft.Data.SqlClient;
using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IComprobanteElectronicoRepository : IRepositoryBase<BE_ComprobanteElectronico>
    {
        Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetListComprobanteElectronicoPorFiltro(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden);
        Task<ResultadoTransaccion<string>> EnviarCorreoError(string codcomprobante, string codventa, string codtipocliente, string nombretipocliente, string anombrede, string nombreusuario, string nombremaquina, string mensaje, SqlConnection conn, SqlTransaction trans);
        Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetComprobantesElectronicosXml(string codcomprobante,string maquina, int idusuario, int orden, SqlConnection conn, SqlTransaction transaction);
        Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetNotaElectronicaXml(string codnota, int orden, SqlConnection conn, SqlTransaction transaction);
        Task<ResultadoTransaccion<string>> ModificarComprobanteElectronico(string campo, string nuevoValor, string XML, string codigo);
        Task<ResultadoTransaccion<string>> GetValirdacionElectronicaNota(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden, SqlConnection conn, SqlTransaction trans);
        Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadreCaja(string documento);
        Task<ResultadoTransaccion<string>> ModificarComprobanteElectronico_transac(string campo, string nuevoValor, string XML, string codigo, SqlConnection conn, SqlTransaction transaction);


    }
}