using Net.Business.DTO;
using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IVentaCabeceraRepository : IRepositoryBase<DtoVentaCabeceraResponse>
    {
        Task<IEnumerable<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa);
    }
}
