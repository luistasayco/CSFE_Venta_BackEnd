using Net.Business.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IProductoRepository
    {
        Task<ResultadoTransaccion<BE_Producto>> GetListProductoPorFiltro(string codigo, string nombre, bool conStock = true);

        Task<ResultadoTransaccion<BE_Producto>> GetListProductoGenericoPorCodigo(string codigo, bool conStock = true);
    }
}
