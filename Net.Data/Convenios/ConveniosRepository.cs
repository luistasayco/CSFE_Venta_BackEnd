using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System;

namespace Net.Data
{
    class ConveniosRepository : RepositoryBase<BE_Convenios>, IConveniosRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ConveniosPorFiltroGet";

        public ConveniosRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_Convenios>> GetConveniosPorFiltros(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto)
        {
            ResultadoTransaccion<BE_Convenios> vResultadoTransaccion = new ResultadoTransaccion<BE_Convenios>();
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
                        cmd.Parameters.Add(new SqlParameter("@codalmacen", codalmacen == null ? string.Empty : codalmacen));
                        cmd.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento == null ? string.Empty : tipomovimiento));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente == null ? string.Empty : codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente == null ? string.Empty : codcliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente == null ? string.Empty : codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora == null ? string.Empty : codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", codcia == null ? string.Empty : codcia));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto == null ? string.Empty : codproducto));

                        var response = new BE_Convenios();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_Convenios>(reader);

                            if (response == null)
                            {
                                response = new BE_Convenios();
                            }
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = response;
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
