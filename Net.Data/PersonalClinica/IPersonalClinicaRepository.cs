using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IPersonalClinicaRepository : IRepositoryBase<BE_PersonalClinica>
    {
        Task<ResultadoTransaccion<BE_PersonalClinica>> GetListPersonalClinicaPorNombre(string nombre);
        Task<ResultadoTransaccion<BE_PersonalClinica>> GetListPersonalClinicaPorCodigo(string codpersonal);
        Task<ResultadoTransaccion<BE_PersonalLimiteConsumo>> GetListLimiteConsumoPorPersonal(string codpersonal);
    }
}
