using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ISapDocumentsRepository
    {
        Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCreateDocument(BE_VentasCabecera valueVenta);
    }
}
