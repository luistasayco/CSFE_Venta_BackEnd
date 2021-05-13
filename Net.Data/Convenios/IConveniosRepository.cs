using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IConveniosRepository : IRepositoryBase<BE_Convenios>
    {
        Task<ResultadoTransaccion<BE_Convenios>> GetConveniosPorFiltros(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto);
    }
}
