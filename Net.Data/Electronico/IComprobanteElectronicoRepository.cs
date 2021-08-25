using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IComprobanteElectronicoRepository : IRepositoryBase<BE_ComprobanteElectronico>
    {
        Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetListComprobanteElectronicoPorFiltro(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden);
    }
}