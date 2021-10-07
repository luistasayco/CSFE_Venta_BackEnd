using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IBusinessPartnersRepository
    {
        Task<ResultadoTransaccion<SapBaseResponse<BusinessPartners>>> SetCreateBusinessPartners(BusinessPartners value);
    }
}
