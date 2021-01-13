using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentaCabeceraRepository : IRepositoryBase<BE_VentaCabecera>
    {
        Task<IEnumerable<BE_VentaCabecera>> GetAll(string buscar, int key,int numeroLineas,int orden, string fecha);
    }
}
