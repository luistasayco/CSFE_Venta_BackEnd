using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ITipoCambioRepository : IRepositoryBase<BE_TipoCambio>
    {
        Task<ResultadoTransaccion> ObtieneTipoCambio();
    }
}
