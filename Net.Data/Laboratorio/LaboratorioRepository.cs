using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class LaboratorioRepository : ILaboratorioRepository
    {
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;
        public LaboratorioRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }

        public async Task<ResultadoTransaccion<BE_Laboratorio>> GetLaboratorio(string buscar, string orden)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_Laboratorio>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                string filter = string.Empty;

                if (orden.Equals("NOMBRE"))
                {
                    if (buscar != null) filter = "&$filter=contains (Name,'" + buscar.ToUpper() + "')";

                }
                else if (orden.Equals("CODIGO"))
                {
                    if (buscar != null) filter = filter = "&$filter=Code eq '" + buscar.ToUpper() + "'";
                }
                
                var cadena = "U_SYP_CS_LABORATOR";
                var campos = "?$select=Code,Name";

                cadena = cadena + campos + filter;

                List<BE_Laboratorio> data = await _connectServiceLayer.GetBaseAsync<BE_Laboratorio>(cadena,200);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
    }
}
