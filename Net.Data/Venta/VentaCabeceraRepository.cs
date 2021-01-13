using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public class VentaCabeceraRepository : RepositoryBase<BE_VentaCabecera>, IVentaCabeceraRepository
    {
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "Sp_VentasCabecera_Consulta";

        public VentaCabeceraRepository(IConnectionSQL context)
            : base(context)
        {
        }
        public Task<IEnumerable<BE_VentaCabecera>> GetAll(string buscar, int key, int numeroLineas, int orden, string fecha)
        {
            return Task.Run(() => {
                buscar = buscar == null ? "" : buscar;
                fecha = fecha == null ? "" : fecha;
                return context.ExecuteSqlViewFindByCondition<BE_VentaCabecera>(SP_GET, new EF_VentaCabeceraConsulta { buscar = buscar, key = key, numerolineas = numeroLineas, orden = orden, fecha = fecha });
            });
        }
    }
}
