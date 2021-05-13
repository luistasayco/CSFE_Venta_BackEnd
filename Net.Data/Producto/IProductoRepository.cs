using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IProductoRepository
    {
        Task<ResultadoTransaccion<BE_Producto>> GetListProductoPorFiltro(string codalmacen, string codigo, string nombre, string codaseguradora, string codcia, bool conStock = true);
        Task<ResultadoTransaccion<BE_Producto>> GetListProductoGenericoPorCodigo(string codigo, bool conStock = true);
        Task<ResultadoTransaccion<BE_Producto>> GetListProductoAlternativoPorCodigo(string codproducto, string codaseguradora, string codcia);
        Task<ResultadoTransaccion<BE_Producto>> GetProductoPorCodigo(string codproducto);
    }
}
