using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ITipoAtencionRepository : IRepositoryBase<BE_TipoAtencion>
    {
        Task<ResultadoTransaccion<BE_TipoAtencion>> GetTipoAtencionPorFiltros(string codaseguradora, string codproducto);
    }
}
