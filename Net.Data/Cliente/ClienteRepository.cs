using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;

namespace Net.Data
{
    public class ClienteRepository: IClienteRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public ClienteRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<IEnumerable<BE_Cliente>> GetListClientePorFiltro(string opcion, string ruc, string nombre)
        {
            var cadena = "BusinessPartners?$filter = CardType eq 'C'";
            var filter = "";
            var campos = "&$select= CardCode, CardName, FederalTaxID, MailAddress, Phone1, U_SYP_BPTP, U_SYP_BPAP, U_SYP_BPAM, U_SYP_BPNO, U_SYP_BPN2, U_SYP_BPTD";

            if (opcion == "RUC")
            {
                filter = "&$filter = FederalTaxID eq '" + ruc + "'";
            }

            if (opcion == "NOMBRE")
            {
                filter = "&$filter=contains (CardName , '" + nombre + "' )";
            }

            cadena = cadena + filter+ campos;

            List<BE_Cliente> data = await _connectServiceLayer.GetAsync<BE_Cliente>(cadena);

            return data;
        }
    }
}
