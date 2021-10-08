using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Net.TCI;
using MSXML2;

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
        const string SP_GET_COMPROBANTEELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosLogXmlGet";
        const string SP_GET_COMPROBANTE_ELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosGet";
        const string SP_GET_NOTAELECTRONICO = DB_ESQUEMA + "VEN_NotaElectronicaLOG_XML";
        const string SP_UPD_COMPROBANTEELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosUpd";
        const string SP_GET_CUADRECAJA_PANTALLA = DB_ESQUEMA + "VEN_Cuadredecaja_Pantalla";
        const string COMPROBANTE_DELETE = DB_ESQUEMA + "VEN_Comprobantes_Delete";
        const string SP_GET_COMPROBANTE = DB_ESQUEMA + "VEN_Comprobantes_Consulta";
        const string COMPROBANTE_UPDATE = DB_ESQUEMA + "VEN_Comprobantes_Update";


        public ComprobanteElectronicoRepository(IConnectionSQL context, IConfiguration configuration)
           : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
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

        public async Task<ResultadoTransaccion<string>> EnviarCorreoError(string codcomprobante, string codventa, string codtipocliente, string nombretipocliente, string anombrede, string nombreusuario, string nombremaquina, string mensaje, SqlConnection conn, SqlTransaction trans)
        {
            return null;
            //ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            //_metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            //vResultadoTransaccion.NombreMetodo = _metodoName;
            //vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            //string destino = string.Empty;
            //string asunto = string.Empty;
            //string body = string.Empty;
            //string secuencia = "\r";

            //try
            //{
            //    CorreoRepository correoRepository = new CorreoRepository(context, _configuration);

            //    vResultadoTransaccion = await correoRepository.GetCorreoDestinatario("EFACT_CORREO_ERRORENREGISTRO", "TO", conn, trans);

            //    destino = vResultadoTransaccion.data;

            //    if (string.IsNullOrEmpty(destino))
            //    {
            //        vResultadoTransaccion.IdRegistro = -1;
            //        vResultadoTransaccion.ResultadoCodigo = -1;
            //        vResultadoTransaccion.ResultadoDescripcion = "NO HAY CORREO DESTINO CONFIGURADO";
            //    }


            //    asunto = "EFact-LOG error en metodo registrar FT-BV : " + codcomprobante + " - " + codventa + " - " + nombretipocliente;// + " - " & RTrim(zFarmacia.LoginSolo)

            //    body = "WebService indica que proceso de registrar comprobantes presentó inconvenientes: " + secuencia + secuencia +
            //           "Comprobante      : " + codcomprobante + secuencia +
            //           "CodVenta         : " + codventa + secuencia +
            //           "Tipo Cliente     : " + codtipocliente + " " + nombretipocliente.Trim() + secuencia +
            //           "Anombre de       : " + anombrede + secuencia +
            //           "Usuario          : " + nombreusuario + secuencia +
            //           "PC               : " + nombremaquina + secuencia +
            //           "Mensaje_Error    : " + mensaje;


            //    //oEmail.EnviarA = wDestino
            //    //oEmail.Asunto = wAsunto
            //    //oEmail.Cuerpo = wBody
            //    //oEmail.EnviarCorreoSQLv2 zFarmacia2.Conexion
            //    //Set oEmail = Nothing
            //    //Set wAdoEmail = Nothing

            //    var correo = new BE_Correo();


            //    vResultadoTransaccion = await correoRepository.Registrar(correo, conn, trans);

            //    if (string.IsNullOrEmpty(destino))
            //    {
            //        vResultadoTransaccion.IdRegistro = -1;
            //        vResultadoTransaccion.ResultadoCodigo = -1;
            //        vResultadoTransaccion.ResultadoDescripcion = "ERROR AL GUARDAR EL CORREO";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    vResultadoTransaccion.IdRegistro = -1;
            //    vResultadoTransaccion.ResultadoCodigo = -1;
            //    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            //}

            //return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetComprobantesElectronicosXml(string codcomprobante, int orden, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTEELECTRONICO, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                    cmd.Parameters.Add(new SqlParameter("@orden", orden));

                    var response = new List<BE_ComprobanteElectronico>();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    vResultadoTransaccion.dataList = response;
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

        public async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetNotaElectronicaXml(string codnota, int orden, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET_NOTAELECTRONICO, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codnota", codnota));
                    cmd.Parameters.Add(new SqlParameter("@orden", orden));

                    var response = new List<BE_ComprobanteElectronico>();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    vResultadoTransaccion.dataList = response;
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

        public async Task<ResultadoTransaccion<string>> GetValirdacionElectronicaNota(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden, SqlConnection conn, SqlTransaction trans)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET, conn, trans))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codempresa", codempresa));
                    cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                    cmd.Parameters.Add(new SqlParameter("@codcomprobante_e", codcomprobante_e));
                    cmd.Parameters.Add(new SqlParameter("@codsistema", codsistema));
                    cmd.Parameters.Add(new SqlParameter("@tipocomp_sunat", tipocomp_sunat));
                    cmd.Parameters.Add(new SqlParameter("@orden", orden));

                    var response = new BE_ComprobanteElectronico();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = context.Convert<BE_ComprobanteElectronico>(reader);
                    }

                    if (response == null)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Verifique la configuración de nota electrónica.";
                        return vResultadoTransaccion;
                    }


                    if (!string.IsNullOrEmpty(response.generarnota) && response.generarnota == "S")
                    {
                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "Genera la nota de crédito";
                        vResultadoTransaccion.data = "S";
                    }
                    else
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "No genera la nota de crédito";
                        vResultadoTransaccion.data = "N";
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

        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadreCaja(string documento)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_PANTALLA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@documento", documento));

                        conn.Open();

                        BE_CuadreCaja response;
                        IList<BE_CuadreCaja> lista = new List<BE_CuadreCaja>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = new BE_CuadreCaja();
                                response.tipopago = ((reader["tipopago"]) is DBNull) ? string.Empty : reader["tipopago"].ToString().Trim();
                                response.nombretipopago = ((reader["nombretipopago"]) is DBNull) ? string.Empty : reader["nombretipopago"].ToString().Trim();
                                response.nombreentidad = ((reader["nombreentidad"]) is DBNull) ? string.Empty : reader["nombreentidad"].ToString().Trim();
                                response.descripcionentidad = ((reader["descripcionentidad"]) is DBNull) ? string.Empty : reader["descripcionentidad"].ToString().Trim();
                                response.numeroentidad = ((reader["numeroentidad"]) is DBNull) ? string.Empty : reader["numeroentidad"].ToString().Trim();
                                response.monto = ((reader["monto"]) is DBNull) ? 0 : Convert.ToDecimal(reader["monto"]);
                                response.montodolares = ((reader["montodolares"]) is DBNull) ? 0 : Convert.ToDecimal(reader["montodolares"]);
                                response.moneda = ((reader["moneda"]) is DBNull) ? string.Empty : reader["moneda"].ToString().Trim();
                                response.codterminal = ((reader["codterminal"]) is DBNull) ? string.Empty : reader["codterminal"].ToString().Trim();
                                response.numeroterminal = ((reader["numeroterminal"]) is DBNull) ? string.Empty : reader["numeroterminal"].ToString().Trim();
                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];
                                response.fechacancelacion = ((reader["fechacancelacion"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechacancelacion"];
                                response.numeroplanilla = ((reader["numeroplanilla"]) is DBNull) ? string.Empty : (string)reader["numeroplanilla"];
                                response.codventa = ((reader["codventa"]) is DBNull) ? string.Empty : (string)reader["codventa"];
                                response.estado = ((reader["estado"]) is DBNull) ? string.Empty : (string)reader["estado"];
                                response.documento = ((reader["documento"]) is DBNull) ? string.Empty : (string)reader["documento"];
                                lista.Add(response);

                            }

                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", lista.Count);
                        vResultadoTransaccion.dataList = lista;

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

        public async Task<ResultadoTransaccion<string>> ModificarComprobanteElectronico_transac(string campo, string nuevoValor, string XML, string codigo, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_UPD_COMPROBANTEELECTRONICO, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@campo", campo));
                    cmd.Parameters.Add(new SqlParameter("@nuevovalor", nuevoValor));
                    cmd.Parameters.Add(new SqlParameter("@xml", XML));
                    cmd.Parameters.Add(new SqlParameter("@codigo", codigo));

                    //await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Comprobante electrónico procesado con éxito.";

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
        public async Task<ResultadoTransaccion<string>> ModificarComprobanteElectronico(string campo, string nuevoValor, string XML, string codigo)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_UPD_COMPROBANTEELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@campo", campo));
                        cmd.Parameters.Add(new SqlParameter("@nuevovalor", nuevoValor));
                        cmd.Parameters.Add(new SqlParameter("@xml", XML));
                        cmd.Parameters.Add(new SqlParameter("@codigo", codigo));

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "Comprobante electrónico procesado con éxito.";
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
