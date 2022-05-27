using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using Net.CrossCotting;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Net.Connection.ServiceLayer;
//using System.Net.Http;
//using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace Net.Data
{
    public class TransaccionPagosRepository : RepositoryBase<BE_TransaccionPagos>, ITransaccionPagosRepository
    {


        private readonly string _IzipayUsuario;
        private readonly string _IzipayPassword;
        private readonly string _IzipayUrlLogin;
        private readonly string _IzipayUrlTransaccion;

        private readonly string _urlLogo;

        private readonly string _cnx;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly ConnectionServiceLayer _connectServiceLayer;

        const string DB_ESQUEMA = "";
        const string SP_INSERT_TRANSACCIONPAGOS_INSERT = DB_ESQUEMA + "VEN_TransaccionPagosIns";
        const string SP_INSERT_TRANSACCIONPAGOS_UPDATE = DB_ESQUEMA + "VEN_TransaccionPagosUpd";

        private BE_ProcesarTransaccionResponse resultado;

        public TransaccionPagosRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _urlLogo = configuration["ArchivoImg:UrlLogo"];
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
            _IzipayUsuario = configuration["IzipayTransaccion:Usuario"];
            _IzipayPassword = configuration["IzipayTransaccion:Password"];
            _IzipayUrlLogin = configuration["IzipayTransaccion:UrlLogin"];
            _IzipayUrlTransaccion = configuration["IzipayTransaccion:UrlTransaccion"];

        }

        //public async Task<ResultadoTransaccion<long>> RegistrarTransaccionPagos(BE_TransaccionPagos value)
        //{

        //    ResultadoTransaccion<long> vResultadoTransaccion = new ResultadoTransaccion<long>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_cnx))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(SP_INSERT_TRANSACCIONPAGOS, conn))
        //            {
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@codoperacion", value.idoperacion));
        //                cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
        //                cmd.Parameters.Add(new SqlParameter("@codtipotransaccion", value.codtipotransaccion));
        //                cmd.Parameters.Add(new SqlParameter("@codterminal", value.codterminal));
        //                cmd.Parameters.Add(new SqlParameter("@codreferencial", value.codreferencial));
        //                cmd.Parameters.Add(new SqlParameter("@numeroTarjeta", value.numeroTarjeta));
        //                cmd.Parameters.Add(new SqlParameter("@dispositivo", value.dispositivo));
        //                cmd.Parameters.Add(new SqlParameter("@trama", value.trama));
        //                cmd.Parameters.Add(new SqlParameter("@regcreateusuario", value.regcreateusuario));

        //                SqlParameter oParamPago = new SqlParameter("@IdtransaccionPago", SqlDbType.BigInt, 1);
        //                oParamPago.Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add(oParamPago);

        //                SqlParameter oParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 1);
        //                oParam.SqlDbType = SqlDbType.Int;
        //                oParam.Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add(oParam);

        //                SqlParameter oParamMs = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 100);
        //                oParamMs.Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add(oParamMs);

        //                await conn.OpenAsync();
        //                await cmd.ExecuteNonQueryAsync();

        //                vResultadoTransaccion.IdRegistro = (int)oParam.Value;
        //                vResultadoTransaccion.ResultadoCodigo = (int)oParam.Value;
        //                vResultadoTransaccion.ResultadoDescripcion = (string)oParamMs.Value;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = "Error al actualizar el archivo en la Base de Datos. Posibles errores: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        public async Task<ResultadoTransaccion<object>> RegistrarPagoTransaccion(BE_TransaccionPagos value, SqlConnection conn, SqlTransaction transaction)
        {

            ResultadoTransaccion<object> vResultadoTransaccion = new ResultadoTransaccion<object>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_INSERT_TRANSACCIONPAGOS_INSERT, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                    cmd.Parameters.Add(new SqlParameter("@codtipotransaccion", value.codtipotransaccion));
                    cmd.Parameters.Add(new SqlParameter("@dispositivo", value.dispositivo));
                    cmd.Parameters.Add(new SqlParameter("@monto", value.monto));
                    cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
                    cmd.Parameters.Add(new SqlParameter("@regcreateusuario", value.regcreateusuario));

                    SqlParameter oParamId = new SqlParameter("@idtransaccionpagos", SqlDbType.Int);
                    oParamId.SqlDbType = SqlDbType.Int;
                    oParamId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oParamId);

                    SqlParameter oParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 1);
                    oParam.SqlDbType = SqlDbType.Int;
                    oParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oParam);

                    SqlParameter oParamMs = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 100);
                    oParamMs.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oParamMs);

                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = (int)cmd.Parameters["@idtransaccionpagos"].Value;
                    vResultadoTransaccion.ResultadoCodigo = (int)cmd.Parameters["@IdTransaccion"].Value;
                    vResultadoTransaccion.ResultadoDescripcion = (string)cmd.Parameters["@MsjTransaccion"].Value;

                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Error al registrar pago: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<object>> ActualizarPagoTransaccion(BE_TransaccionPagos value, SqlConnection conn, SqlTransaction transaction)
        {

            ResultadoTransaccion<object> vResultadoTransaccion = new ResultadoTransaccion<object>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_INSERT_TRANSACCIONPAGOS_UPDATE, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idtransaccionpagos", value.idtransaccionpagos));
                    cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                    cmd.Parameters.Add(new SqlParameter("@idtransaccion", value.idtransaccion));
                    cmd.Parameters.Add(new SqlParameter("@codterminal", value.codterminal));
                    cmd.Parameters.Add(new SqlParameter("@codreferencial", value.codreferencial));
                    cmd.Parameters.Add(new SqlParameter("@tipotarjeta", value.tipotarjeta));
                    cmd.Parameters.Add(new SqlParameter("@tarjetanumero", value.numeroTarjeta));
                    cmd.Parameters.Add(new SqlParameter("@trama", value.trama));
                    cmd.Parameters.Add(new SqlParameter("@regupdateusuario", value.regupdateusuario));

                    SqlParameter oParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 1);
                    oParam.SqlDbType = SqlDbType.Int;
                    oParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oParam);

                    SqlParameter oParamMs = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 100);
                    oParamMs.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oParamMs);

                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = (int)cmd.Parameters["@IdTransaccion"].Value;
                    vResultadoTransaccion.ResultadoCodigo = (int)cmd.Parameters["@IdTransaccion"].Value;
                    vResultadoTransaccion.ResultadoDescripcion = (string)cmd.Parameters["@MsjTransaccion"].Value;

                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Error al actualizar el archivo en la Base de Datos. Posibles errores: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }


        public async Task<ResultadoTransaccion<BE_ProcesarTransaccionLoginResponse>> LoginTransaccion()
        {

            var vResultadoTransaccion = new ResultadoTransaccion<BE_ProcesarTransaccionLoginResponse>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string url = _IzipayUrlLogin; //"http://localhost:9090/API_PPAD/login";
            var objLoginRq = new BE_ProcesarTransaccionLoginRequest(_IzipayUsuario, _IzipayPassword);
            BE_ProcesarTransaccionLoginResponse resultado;

            try
            {

                var client = _clientFactory.CreateClient();
                string json = JsonConvert.SerializeObject(objLoginRq, Formatting.Indented);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, httpContent);
                if (!response.IsSuccessStatusCode)
                {
                    resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionLoginResponse>(response.Content.ReadAsStringAsync().Result);
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionLoginResponse>(response.Content.ReadAsStringAsync().Result);
                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = (resultado.resultado == "00") ? 0 : 1;
                    vResultadoTransaccion.ResultadoDescripcion = resultado.mensaje;
                    vResultadoTransaccion.data = resultado;

                }

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Error al actualizar el archivo en la Base de Datos. Posibles errores: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<object>> ProcesarTransaccion(BE_ProcesarTransaccionPagoIzipayRequest value, string codventa, int regcreateusuario)
        {

            var vResultadoTransaccion = new ResultadoTransaccion<object>();

            //string strToken = string.Empty;
            //var objLogin = this.LoginTransaccion();
            //if (objLogin.Result.data.resultado != "00")
            //{
            //    vResultadoTransaccion.IdRegistro = 0;
            //    vResultadoTransaccion.ResultadoCodigo = regcreateusuario;
            //    vResultadoTransaccion.ResultadoDescripcion = "Izipay. " + objLogin.Result.data.mensaje;
            //    return vResultadoTransaccion;
            //}

            //strToken = objLogin.Result.data.token;

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string url = _IzipayUrlTransaccion;
            BE_ProcesarTransaccionResponse resultado;

            try
            {

                var objTransaccionPago = new BE_TransaccionPagos()
                {
                    codventa = codventa,
                    codtipotransaccion = value.ecr_transaccion,
                    regcreateusuario = regcreateusuario,
                    monto = value.ecr_amount,
                    moneda = value.ecr_currency_code,
                    dispositivo = "IZIPAY"
                };

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    //RegistrarPagoTransaccionUpd
                    ResultadoTransaccion<object> InsertTransac = await RegistrarPagoTransaccion(objTransaccionPago, conn, transaction);
                    if (InsertTransac.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion = InsertTransac;
                        transaction.Rollback();
                        return InsertTransac;
                    }

                    objTransaccionPago.idtransaccionpagos = InsertTransac.IdRegistro;

                    //var client = _clientFactory.CreateClient();
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strToken);

                    //string json = JsonConvert.SerializeObject(value, Formatting.Indented);

                    //StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    //var response = await client.PostAsync(url, httpContent);
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionResponse>(response.Content.ReadAsStringAsync().Result);
                    //    vResultadoTransaccion.IdRegistro = -1;
                    //    vResultadoTransaccion.ResultadoCodigo = -1;
                    //    vResultadoTransaccion.ResultadoDescripcion = response.Content.ReadAsStringAsync().Result;
                    //}
                    //else
                    //{
                        //resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionResponse>(response.Content.ReadAsStringAsync().Result);
                        //vResultadoTransaccion.IdRegistro = 0;
                        //vResultadoTransaccion.ResultadoCodigo = 0;

                    if (value.id_autorizacion != null)
                    {
                        vResultadoTransaccion.ResultadoDescripcion = "<strong>PAGO IZIPAY EXITOSO.</strong><br>" + value.ecr_data_adicional;

                        string text = value.ecr_data_adicional;

                        var resultTER = text.IndexOf("ATERM:");
                        var numTER = text.Substring(resultTER + 6, 8);

                        var resultREF = text.IndexOf("REF:");
                        var numRef = text.Substring(resultREF + 4, 4);

                        var resultOpe = text.IndexOf("ID:");
                        var numOpe = text.Substring(resultOpe + 3, 16);

                        var resulTAR = text.IndexOf("ATARJ:");
                        var numTarjeta = text.Substring(resulTAR + 11, 4);

                        var resulTIPO = text.IndexOf("TIPO:");
                        var tipotarjeta = text.Substring(resulTIPO + 5, 4);

                        var objResp = new { aterm = numTER, numref = numRef, numOpe= numOpe, tipoTarjeta = tipotarjeta, numTarjeta = numTarjeta, message = value.message };
                        vResultadoTransaccion.data = objResp;

                        try
                        {
                            var objTransaccionPagoUpd = new BE_TransaccionPagos()
                            {
                                idtransaccionpagos = objTransaccionPago.idtransaccionpagos,
                                codventa = codventa,
                                idtransaccion = value.id_autorizacion,
                                codterminal = numTER,
                                codreferencial = numRef,
                                tipotarjeta = tipotarjeta,
                                numeroTarjeta = numTarjeta,
                                trama = text,
                                regupdateusuario = regcreateusuario,
                            };

                            ResultadoTransaccion<object> InsertTransacUpd = await ActualizarPagoTransaccion(objTransaccionPagoUpd, conn, transaction);
                            if (InsertTransacUpd.ResultadoCodigo == -1)
                            {
                                // ignoramos, porque ya realizo el pago el izipay
                                ////vResultadoTransaccion = InsertTransacUpd;
                                ////transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            // ignoramos, porque ya realizo el pago el izipay
                        }

                        transaction.Commit();

                        //}
                        //else
                        //{

                        //    //validacion
                        //    vResultadoTransaccion.ResultadoCodigo = 1;
                        //    vResultadoTransaccion.ResultadoDescripcion = "<strong>IZIPAY.</strong><br>" + resultado.message;
                        //    var objResp = new { aterm = (string)null, numref = (string)null, numTarjeta = (string)null, message = (string)null };
                        //    vResultadoTransaccion.data = objResp;
                        //    transaction.Rollback();

                        //}
                    }

                }//using 

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Error al actualizar el archivo en la Base de Datos. Posibles errores: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<object>> AnularTransaccion(BE_ProcesarTransaccionAnularRequest value, string codventa, int regcreateusuario)
        {

            var vResultadoTransaccion = new ResultadoTransaccion<object>();

            //string strToken = string.Empty;
            //var objLogin = this.LoginTransaccion();
            //if (objLogin.Result.data.resultado != "00")
            //{
            //    vResultadoTransaccion.IdRegistro = 0;
            //    vResultadoTransaccion.ResultadoCodigo = 1;
            //    vResultadoTransaccion.ResultadoDescripcion = "Izipay. " + objLogin.Result.data.mensaje;
            //    return vResultadoTransaccion;
            //}

            //strToken = objLogin.Result.data.token;

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            //string url = _IzipayUrlTransaccion;
            //BE_ProcesarTransaccionResponse resultado;

            try
            {
                var objTransaccionPago = new BE_TransaccionPagos()
                {
                    codventa = codventa,
                    codtipotransaccion = value.ecr_transaccion,
                    regcreateusuario = regcreateusuario,
                    monto = value.ecr_amount,
                    moneda = value.ecr_currency_code,
                    dispositivo = "IZIPAY"
                };

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    ResultadoTransaccion<object> InsertTransac = await RegistrarPagoTransaccion(objTransaccionPago, conn, transaction);
                    if (InsertTransac.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion = InsertTransac;
                        transaction.Rollback();
                        return InsertTransac;
                    }

                    objTransaccionPago.idtransaccionpagos = InsertTransac.IdRegistro;

                    //var client = _clientFactory.CreateClient();
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strToken);

                    //string json = JsonConvert.SerializeObject(value, Formatting.Indented);

                    //StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    //var response = await client.PostAsync(url, httpContent);
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionResponse>(response.Content.ReadAsStringAsync().Result);
                    //    vResultadoTransaccion.IdRegistro = -1;
                    //    vResultadoTransaccion.ResultadoCodigo = -1;
                    //    vResultadoTransaccion.ResultadoDescripcion = response.ReasonPhrase;
                    //    transaction.Rollback();
                    //}
                    //else
                    //{
                    //    resultado = JsonConvert.DeserializeObject<BE_ProcesarTransaccionResponse>(response.Content.ReadAsStringAsync().Result);
                    //    vResultadoTransaccion.IdRegistro = 0;
                    //    vResultadoTransaccion.ResultadoCodigo = 0;

                    if (value.id_autorizacion != null)
                    {

                        string text = value.ecr_data_adicional;

                        var resultTER = text.IndexOf("ATERM:");
                        var numTER = text.Substring(resultTER + 6, 9);

                        var resultREF = text.IndexOf("REF:");
                        var numRef = text.Substring(resultREF + 4, 4);

                        var resulTAR = text.IndexOf("ATARJ:");
                        var numTarjeta = text.Substring(resulTAR + 18, 4);

                        var resulTIPO = text.IndexOf("TIPO:");
                        var tipotarjeta = text.Substring(resulTIPO + 5, 4);

                        var objResp = new { aterm = numTER, numref = numRef, numTarjeta = string.Empty, message = string.Empty };
                        vResultadoTransaccion.data = objResp;

                        vResultadoTransaccion.ResultadoDescripcion = $"<strong>IZIPAY ANULACION DE PAGO REF:{value.message}  EXITOSO.</strong><br>" + value.ecr_data_adicional;

                        try
                        {
                            var objTransaccionPagoUpd = new BE_TransaccionPagos()
                            {
                                idtransaccionpagos = objTransaccionPago.idtransaccionpagos,
                                codventa = codventa,
                                idtransaccion = value.id_autorizacion,
                                codterminal = numTER,
                                codreferencial = numRef,
                                tipotarjeta = tipotarjeta,
                                numeroTarjeta = numTarjeta,
                                trama = text,
                                regupdateusuario = regcreateusuario,
                            };

                            ResultadoTransaccion<object> InsertTransacUpd = await ActualizarPagoTransaccion(objTransaccionPagoUpd, conn, transaction);
                            if (InsertTransacUpd.ResultadoCodigo == -1)
                            {
                                // ignoramos, porque ya realizo el pago el izipay
                                ////vResultadoTransaccion = InsertTransacUpd;
                                ////transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            // ignoramos, porque ya realizo el pago el izipay
                        }

                        transaction.Commit();

                        //}
                        //else
                        //{
                        //    //validacion
                        //    vResultadoTransaccion.ResultadoCodigo = 1;
                        //    vResultadoTransaccion.ResultadoDescripcion = "<strong>IZIPAY.</strong><br>" + resultado.message;
                        //    var objResp = new { aterm = (string)null, numref = (string)null, numTarjeta = (string)null, message = (string)null };
                        //    vResultadoTransaccion.data = objResp;
                        //    transaction.Rollback();
                        //}
                    }
                }//using

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Error al actualizar el archivo en la Base de Datos. Posibles errores: 1.- Ya sea porque existe el archivo en la BD. 2.- No se encuentra registrado el comprobante en la BD por eso no puede actualizar. otros:" + ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }


    }
}
