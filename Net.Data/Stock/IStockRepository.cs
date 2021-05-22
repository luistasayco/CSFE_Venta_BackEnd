using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IStockRepository
    {
        Task<ResultadoTransaccion<BE_Stock>> GetListStockPorFiltro(string codalmacen, string nombre, string codproducto, bool constock);
        Task<ResultadoTransaccion<BE_Stock>> GetListStockPorProductoAlmacen(string codalmacen, string codproducto);
        Task<ResultadoTransaccion<BE_StockLote>> GetListStockLotePorFiltro(string codalmacen, string codproducto, bool constock);
        Task<ResultadoTransaccion<BE_Stock>> GetListStockPorProducto(string codproducto, bool constock);
    }
}
