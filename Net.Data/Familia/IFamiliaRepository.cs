using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IFamiliaRepository : IRepositoryBase<BE_Familia>
    {
        Task<ResultadoTransaccion<BE_Familia>> GetFamilia(string buscar, string orden);
        Task<ResultadoTransaccion<BE_Familia>> GetFamiliaLista(string sysFamilia);

    }
}
