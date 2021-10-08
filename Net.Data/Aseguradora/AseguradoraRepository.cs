using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.Connection.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class AseguradoraRepository : RepositoryBase<BE_Aseguradora>, IAseguradoraRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public AseguradoraRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            //_cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_AseguradoraPorFiltroGet";

        public async Task<ResultadoTransaccion<BE_Aseguradora>> GetAseguradora(string buscar, string estado, string orden)
        {
            ResultadoTransaccion<BE_Aseguradora> vResultadoTransaccion = new ResultadoTransaccion<BE_Aseguradora>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@estado", estado));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        var response = new List<BE_Aseguradora>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Aseguradora>)context.ConvertTo<BE_Aseguradora>(reader);
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
