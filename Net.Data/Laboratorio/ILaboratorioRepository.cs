using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ILaboratorioRepository : IRepositoryBase<BE_Laboratorio>
    {
        Task<ResultadoTransaccion<BE_Laboratorio>> GetLaboratorio(string buscar, string orden);
    }
}
