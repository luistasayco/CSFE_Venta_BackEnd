using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IUbigeoRepository : IRepositoryBase<BE_Pais>
    {
        Task<ResultadoTransaccion<BE_Pais>> GetListPais();
        Task<ResultadoTransaccion<BE_Departamento>> GetListDepartamento();
        Task<ResultadoTransaccion<BE_Provincia>> GetListProvinciaPorFiltro(string coddepartamento);
        Task<ResultadoTransaccion<BE_Distrito>> GetListDistritoPorFiltro(string coddepartamento, string codprovincia);
    }
}
