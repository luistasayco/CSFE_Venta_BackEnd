using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;

namespace Net.Data
{
    public class ComprobanteRepository : RepositoryBase<BE_Comprobante>, IComprobanteRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_LISTA_COMPROBANTES_POR_FILTRO = DB_ESQUEMA + "VEN_ListaComprobantesPorFiltrosGet";
        const string SP_GET_COMPROBANTE_POR_CODCOMPROBANTE = DB_ESQUEMA + "";

        public ComprobanteRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_Comprobante>> GetListaComprobantesPorFiltro(string codcomprobante, DateTime fecinicio, DateTime fecfin)
        {
            ResultadoTransaccion<BE_Comprobante> vResultadoTransaccion = new ResultadoTransaccion<BE_Comprobante>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fecinicio = Utilidades.GetFechaHoraInicioActual(fecinicio);
            fecfin = Utilidades.GetFechaHoraFinActual(fecfin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                   
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_COMPROBANTES_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@fecinicio", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fecfin", fecfin));

                        conn.Open();

                        var response = new List<BE_Comprobante>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Comprobante>)context.ConvertTo<BE_Comprobante>(reader);
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
