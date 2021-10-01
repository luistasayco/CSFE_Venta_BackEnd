using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ISapReserveStockRepository
    {
        Task<ResultadoTransaccion<SapBaseResponse<SapReserveStock>>> SetCreateReserve(SapReserveStockNew value);
        Task<ResultadoTransaccion<SapBaseResponse<SapReserveStock>>> SetDeleteReserve(int value);
    }
}
