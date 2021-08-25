using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace Net.Data
{
    public class ComprobanteElectronicoRepository : RepositoryBase<BE_ComprobanteElectronico>, IComprobanteElectronicoRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "Sp_ComprobantesElectronicos_ConsultaVB";

        public ComprobanteElectronicoRepository(IConnectionSQL context, IConfiguration configuration)
           : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlClinica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
        }

        public async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetListComprobanteElectronicoPorFiltro(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
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
                        cmd.Parameters.Add(new SqlParameter("@codempresa", codempresa));
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante_e", codcomprobante_e));
                        cmd.Parameters.Add(new SqlParameter("@codsistema", codsistema));
                        cmd.Parameters.Add(new SqlParameter("@tipocomp_sunat", tipocomp_sunat));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        var response = new List<BE_ComprobanteElectronico>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
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
