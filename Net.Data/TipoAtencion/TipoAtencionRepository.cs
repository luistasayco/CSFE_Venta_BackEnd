using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class TipoAtencionRepository : RepositoryBase<BE_TipoAtencion>, ITipoAtencionRepository
    {
        private readonly string _cnxLogistica;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_TIPOATENCION_POR_FILTRO = DB_ESQUEMA + "VEN_TipoAtencionPorFiltroGet";

        public TipoAtencionRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnxLogistica = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_TipoAtencion>> GetTipoAtencionPorFiltros(string codaseguradora, string codproducto)
        {
            ResultadoTransaccion<BE_TipoAtencion> vResultadoTransaccion = new ResultadoTransaccion<BE_TipoAtencion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxLogistica))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_TIPOATENCION_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                        var response = new List<BE_TipoAtencion>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_TipoAtencion>)context.ConvertTo<BE_TipoAtencion>(reader);
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
