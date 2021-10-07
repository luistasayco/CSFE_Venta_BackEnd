using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Net.Connection;
using System.Linq;

namespace Net.Data
{
    public class BusinessPartnersRepository : RepositoryBase<BusinessPartners>, IBusinessPartnersRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        const string DB_ESQUEMA = "";

        public BusinessPartnersRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IConnectionSQL context)
             : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }

        public async Task<ResultadoTransaccion<SapBaseResponse<BusinessPartners>>> SetCreateBusinessPartners(BusinessPartners value)
        {
            ResultadoTransaccion<SapBaseResponse<BusinessPartners>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<BusinessPartners>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = "BusinessPartners";
                SapBaseResponse<BusinessPartners> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<BusinessPartners>>(cadena, value);

                if (data.CardCode == "")
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<SapBaseResponse<BusinessPartners>>> SetUpdateBusinessPartners(BusinessPartners value)
        {
            ResultadoTransaccion<SapBaseResponse<BusinessPartners>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<BusinessPartners>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = "BusinessPartners";
                SapBaseResponse<BusinessPartners> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<BusinessPartners>>(cadena, value);

                if (data.CardCode == "")
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                vResultadoTransaccion.data = data;
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
