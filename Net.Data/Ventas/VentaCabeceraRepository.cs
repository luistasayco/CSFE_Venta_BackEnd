using Net.Business.DTO;
using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Net.Data
{
    public class VentaCabeceraRepository : RepositoryBase<DtoVentaCabeceraResponse>, IVentaCabeceraRepository
    {
        private readonly string _cnx;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaVentasCabeceraPorFiltrosGet";

        public VentaCabeceraRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }
        public Task<IEnumerable<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa)
        {
            return Task.Run(() => {
                return context.ExecuteSqlViewFindByCondition<BE_VentasCabecera>(SP_GET, new EF_VentaCabeceraConsulta { codcomprobante = codcomprobante, codventa = codventa}, _cnx);
            });
        }
    }
}
