using Net.Business.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IClienteRepository
    {
        Task<IEnumerable<BE_Cliente>> GetListClientePorFiltro(string opcion, string ruc, string nombre);
    }
}
