using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class CorreoRepository : RepositoryBase<BE_Correo>, ICorreoRepository
    {
        private readonly string _cnxClinica;
        private readonly string _cnxLogistica;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_DESTINATARIO = DB_ESQUEMA + "Sp_CorreoDestinatario_Consulta";
        const string SP_INSERT = DB_ESQUEMA + "VEN_Ut_EnviarCorreov2";

        public CorreoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _cnxLogistica = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<string>> GetCorreoDestinatario(string codLista, string dscTipo)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string destinatario = string.Empty;


            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DESTINATARIO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_lista", codLista));
                        cmd.Parameters.Add(new SqlParameter("@dsc_tipo", dscTipo));

                        string correo = string.Empty;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                correo = ((reader["destinatario"]) is DBNull) ? string.Empty : reader["destinatario"].ToString();
                            }
                        }

                        if (string.IsNullOrEmpty(correo))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "NO HAY CORREO DESTINO CONFIGURADO.";
                            return vResultadoTransaccion;
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = correo;
                    }
                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }



            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<string>> Registrar(BE_Correo value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnxLogistica))
            {
                conn.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@enviara", value.enviara));
                        cmd.Parameters.Add(new SqlParameter("@copiara", value.copiara));
                        cmd.Parameters.Add(new SqlParameter("@copiarh", value.copiarh));
                        cmd.Parameters.Add(new SqlParameter("@asunto", value.asunto));
                        cmd.Parameters.Add(new SqlParameter("@cuerpo", value.cuerpo));
                        cmd.Parameters.Add(new SqlParameter("@file", value.archivo));

                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "CORREO GUARDADO CON EXITO";
                    }
                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return vResultadoTransaccion;
        }
    }
}
