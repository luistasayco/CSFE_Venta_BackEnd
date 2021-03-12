using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IPersonalClinicaRepository : IRepositoryBase<BE_PersonalClinica>
    {
        Task<IEnumerable<BE_PersonalClinica>> GetListPersonalClinicaPorNombre(string nombre);
    }
}
