using Net.Business.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IClienteRepository
    {
        Task<ResultadoTransaccion<BE_Cliente>> GetListClientePorFiltro(string opcion, string ruc, string nombre);
        Task<ResultadoTransaccion<BE_Cliente>> GetListClienteLogisticaPorFiltro(string ruc, string nombre);
        Task<ResultadoTransaccion<BE_ClienteLogistica>> GetListDataClienteLogisticaPorFiltro(string ruc, string nombre);
        Task<ResultadoTransaccion<BE_ClienteLogistica>> GetListDataClienteLogisticaPorCliente(string codcliente);
        Task<ResultadoTransaccion<BE_Cliente>> GetCodigoClientePorCodigo(string codigoCliente);
        Task<ResultadoTransaccion<BE_ClienteLogistica>> Registrar(BE_ClienteLogistica item);
        Task<ResultadoTransaccion<BE_ClienteLogistica>> Modificar(BE_ClienteLogistica item);
    }
}
