using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IAseguradoraxProductoRepository : IRepositoryBase<BE_AseguradoraxProducto>
    {
        Task<ResultadoTransaccion<BE_AseguradoraxProducto>> GetListAseguradoraxProductoPorFiltros(string codaseguradora, string codproducto, int tipoatencion);

        Task<ResultadoTransaccion<int>> AseguradoraxProductoInsert(BE_AseguradoraxProducto value, int orden);
        Task<ResultadoTransaccion<int>> AseguradoraxProductoDelete(BE_AseguradoraxProducto value, int orden);
        Task<ResultadoTransaccion<bool>> GetExisteAseguradoraxProducto(string codaseguradora, string codproducto);
        Task<ResultadoTransaccion<string>> GetExisteConvenioAseguradoraxProducto(string codaseguradora, string codproducto);
    }
}
