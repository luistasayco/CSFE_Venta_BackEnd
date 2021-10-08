using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net.Connection;

namespace Net.Data
{
    public class FamiliaRepository : RepositoryBase<BE_Familia>, IFamiliaRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        const string DB_ESQUEMA = "";
        const string SP_GET_FAMILIA_LISTA = DB_ESQUEMA + "VEN_FamiliaGet";

        public FamiliaRepository(IConnectionSQL context,IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }

        public async Task<ResultadoTransaccion<BE_Familia>> GetFamilia(string buscar, string orden)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_Familia>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                string filter = string.Empty;

                if (orden.Equals("NOMBRE"))
                {
                    filter = "&$filter=U_SYP_DESFAMILIA eq '" + buscar + "'";
                }
                else if (orden.Equals("CODIGO"))
                {
                    filter = "&$filter=Code eq '" + buscar + "'";
                }

                var cadena = "U_SYP_CS_FAMILIA";
                var campos = "?$select=Code,U_SYP_DESFAMILIA";

                cadena = cadena + campos + filter;

                List<BE_Familia> data = await _connectServiceLayer.GetAsync<BE_Familia>(cadena);

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

        public async Task<ResultadoTransaccion<BE_Familia>> GetFamiliaLista(string sysFamilia)
        {
            ResultadoTransaccion<BE_Familia> vResultadoTransaccion = new ResultadoTransaccion<BE_Familia>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_FAMILIA_LISTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@sysFamilia", sysFamilia));

                        conn.Open();

                        var response = new List<BE_Familia>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Familia>)context.ConvertTo<BE_Familia>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;

                    }
                }
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
