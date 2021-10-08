using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IAseguradoraRepository : IRepositoryBase<BE_Aseguradora>
    {
        Task<ResultadoTransaccion<BE_Aseguradora>> GetAseguradora(string buscar, string estado, string orden);
    }
}
