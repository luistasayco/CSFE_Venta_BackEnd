using Net.Business.Entities;
using Net.Connection;
using System;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IComprobanteRepository : IRepositoryBase<BE_Comprobante>
    {
        Task<ResultadoTransaccion<BE_Comprobante>> GetListaComprobantesPorFiltro(string codcomprobante, DateTime fecinicio, DateTime fecfin);
    }
}