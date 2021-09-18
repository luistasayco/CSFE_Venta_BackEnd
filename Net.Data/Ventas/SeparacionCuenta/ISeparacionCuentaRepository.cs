using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ISeparacionCuentaRepository : IRepositoryBase<BE_VentasGenerado>
    {
        Task<ResultadoTransaccion<BE_SeperacionCuenta>> RegistrarSeparacionCuenta(BE_SeparacionCuentaXml value);
    }
}