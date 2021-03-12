using Net.Business.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IPlanesRepository
    {
        Task<IEnumerable<BE_Planes>> GetByFiltros(BE_Planes value);
        Task<BE_Planes> GetbyId(BE_Planes value);
        Task<ResultadoTransaccion> Eliminar(BE_Planes value);
        Task<ResultadoTransaccion> Registrar(BE_Planes value);
        Task<ResultadoTransaccion> Modificar(BE_Planes value);

    }

}