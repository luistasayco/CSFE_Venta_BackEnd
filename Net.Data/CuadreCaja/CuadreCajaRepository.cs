using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.Connection.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class CuadreCajaRepository : RepositoryBase<BE_CuadreCaja>, ICuadreCajaRepository
    {
        private readonly string _cnx;
        private readonly IConfiguration _configuration;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly ConnectionServiceLayer _connectServiceLayer;
        private readonly IHttpClientFactory _clientFactory;

        const string DB_ESQUEMA = "";
        const string SP_GET_CUADRECAJA_POR_FILTRO = DB_ESQUEMA + "VEN_ListaCuadredeCajaPorFiltroGet";
        const string SP_GET_CUADRECAJA_GENERAL_POR_FILTRO = DB_ESQUEMA + "VEN_ListaCuadredeCajaGralPorFiltroGet";
        const string SP_GET_RPCUADRECAJA_POR_FILTRO = DB_ESQUEMA + "VEN_Rp_CuadreCajaPorFiltroGet";
        const string SP_GET_RPCUADRECAJADETALLADO_POR_FILTRO = DB_ESQUEMA + "VEN_Rp_CuadreCajaDetalladoPorFiltroGet";
        const string SP_GET_CUADRECAJA_INSERT = DB_ESQUEMA + "VEN_CuadredeCaja_Ins";
        const string SP_GET_CUADRECAJA_DELETE = DB_ESQUEMA + "VEN_CuadredeCaja_Del";
        const string SP_GET_TABLAS_CODIGO_TIPO_TARJETA = DB_ESQUEMA + "VEN_TablasCodigoTipoTarjetaGet";
        const string SP_GET_TABLAS_CODIGO_BANCO = DB_ESQUEMA + "VEN_TablasCodigoBancoGet";
        const string SP_POST_CUADRECAJA_INFO_SAP_UPDATE = DB_ESQUEMA + "VEN_CuadreCajaInformacionSapUpd";

        public CuadreCajaRepository(IConnectionSQL context, IConfiguration configuration, IHttpClientFactory clientFactory)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = this.GetType().Name;
            _clientFactory = clientFactory;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadredeCajaPorFiltro(string documento)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_CuadreCaja>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@documento", documento));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CuadreCaja>)context.ConvertTo<BE_CuadreCaja>(reader);
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

        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadredeCajaGeneralPorFiltro(string fecha1, string fecha2, string coduser, string codcentro, string orden)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_CuadreCaja>();
                    BE_CuadreCaja responseEnty;

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_GENERAL_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fecha1", fecha1));
                        cmd.Parameters.Add(new SqlParameter("@fecha2", fecha2));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", coduser));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                responseEnty = new BE_CuadreCaja()
                                {
                                    documento = (reader["documento"] is DBNull) ? string.Empty : (string)reader["documento"],
                                    documentoe = (reader["documentoe"] is DBNull) ? string.Empty : (string)reader["documentoe"],
                                    nombres = (reader["nombres"] is DBNull) ? string.Empty : (string)reader["nombres"],
                                    docmonto = (reader["docmonto"] is DBNull) ? 0 : Convert.ToDecimal(reader["docmonto"]),
                                    movimiento = (reader["movimiento"] is DBNull) ? string.Empty : (string)reader["movimiento"],
                                    montoingreso = (reader["montoingreso"] is DBNull) ? 0 : Convert.ToDecimal(reader["montoingreso"])
                                };
                                response.Add(responseEnty);
                                //response = (List<BE_CuadreCaja>)context.ConvertTo<BE_CuadreCaja>(reader);

                            }
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

        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetRpCuadreCajaPorFiltro(string fechainicio, string fechafin, string coduser, string codcentro, string numeroplanilla, decimal dolares)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_CuadreCaja>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RPCUADRECAJA_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));
                        cmd.Parameters.Add(new SqlParameter("@coduser", coduser));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroplanilla));
                        cmd.Parameters.Add(new SqlParameter("@dolares", dolares));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CuadreCaja>)context.ConvertTo<BE_CuadreCaja>(reader);
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
        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetRpCuadreCajaDetalladoPorFiltro(string fechainicio, string fechafin, string coduserp, string codcentro, string numeroplanilla, string orden)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_CuadreCaja>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RPCUADRECAJADETALLADO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));
                        cmd.Parameters.Add(new SqlParameter("@coduserp", coduserp));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroplanilla));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CuadreCaja>)context.ConvertTo<BE_CuadreCaja>(reader);
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

        public async Task<ResultadoTransaccion<string>> CuadreCajaRegistrar(List<BE_CuadreCaja> value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string correlativo = string.Empty;

            try
            {
                // Obtenemos los registros de pago
                ComprobanteRepository comprobanteRepository = new ComprobanteRepository(context, _configuration, _clientFactory);
                ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccionCaja = await comprobanteRepository.GetListaCuadreCaja(value[0].documento.ToString());

                if (vResultadoTransaccionCaja.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = vResultadoTransaccionCaja.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                List<BE_CuadreCaja> listCuadreCajaOld = (List<BE_CuadreCaja>)vResultadoTransaccionCaja.dataList;

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        var documento = value[0].documento.ToString();
                        var idusuario = value[0].regupdateidusuario.ToString();

                        using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_DELETE, conn, transaction))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@documento", documento));
                            cmd.Parameters.Add(new SqlParameter("@idusuario", idusuario));
                            await cmd.ExecuteNonQueryAsync();
                            vResultadoTransaccion.IdRegistro = 0;
                            vResultadoTransaccion.ResultadoCodigo = 0;
                            vResultadoTransaccion.ResultadoDescripcion = "SE ELIMINO EL REGISTRO CORRECTAMENTE";
                            vResultadoTransaccion.data = correlativo;
                        }

                        using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_INSERT, conn, transaction))
                        {

                            foreach (var item in value)
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                SqlParameter oParam = new SqlParameter("@correlativo", SqlDbType.Char, 11)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmd.Parameters.Add(oParam);

                                cmd.Parameters.Add(new SqlParameter("@codcomprobante", item.codcomprobante));
                                cmd.Parameters.Add(new SqlParameter("@monto", item.monto));
                                cmd.Parameters.Add(new SqlParameter("@tipopago", item.tipopago));
                                cmd.Parameters.Add(new SqlParameter("@nombreentidad", item.nombreentidad.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@descripcionentidad", item.descripcionentidad));
                                cmd.Parameters.Add(new SqlParameter("@numeroentidad", item.numeroentidad.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@operacion", "P"));
                                cmd.Parameters.Add(new SqlParameter("@variostipopago", "S"));
                                cmd.Parameters.Add(new SqlParameter("@moneda", item.moneda));
                                cmd.Parameters.Add(new SqlParameter("@fechaCancelacion", item.fechacancelacion));
                                cmd.Parameters.Add(new SqlParameter("@codterminal", item.codterminal));
                                cmd.Parameters.Add(new SqlParameter("@tipodecambio", item.tipodecambio));

                                await cmd.ExecuteNonQueryAsync();

                                correlativo = cmd.Parameters["@correlativo"].Value.ToString();
                                if (correlativo.Equals("")) throw new Exception("Error al insertar comprobante !!");
                                vResultadoTransaccion.IdRegistro = 0;
                                vResultadoTransaccion.ResultadoCodigo = 0;
                                vResultadoTransaccion.ResultadoDescripcion = "SE MODIFICO EL TIPO DE PAGO CORRECTAMENTE";
                                vResultadoTransaccion.data = correlativo;
                                item.correlativo = correlativo;
                            }
                        }

                        #region Eliminar pagos de comprobante
                        // Eliminar pagos de comprobante
                        // Se retira los pagos a pedido de clinica, se acordo el dia viernes 08/04
                        //foreach (BE_CuadreCaja item in vResultadoTransaccionCaja.dataList)
                        //{
                        //    ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoTransaccionCancelPago = await SetCancelIncomingPayments(item.ide_trans);

                        //    if (resultadoTransaccionCancelPago.ResultadoCodigo == -1)
                        //    {
                        //        transaction.Rollback();
                        //        vResultadoTransaccion.IdRegistro = -1;
                        //        vResultadoTransaccion.ResultadoCodigo = -1;
                        //        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCancelPago.ResultadoDescripcion;
                        //        return vResultadoTransaccion;
                        //    }
                        //}

                        // Insertamos pagos y comprobantes
                        // Se retira los pagos a pedido de clinica, se acordo el dia viernes 08/04
                        //ResultadoTransaccion<string> resultadoTransaccionCreatePago = await SetCreateIncomingPayments(documento, listCuadreCajaOld[0].doc_entry, value, conn, transaction);

                        //if (resultadoTransaccionCreatePago.ResultadoCodigo == -1)
                        //{
                        //    transaction.Rollback();
                        //    vResultadoTransaccion.IdRegistro = -1;
                        //    vResultadoTransaccion.ResultadoCodigo = -1;
                        //    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCreatePago.ResultadoDescripcion;
                        //    return vResultadoTransaccion;
                        //}
                        #endregion

                        transaction.Commit();
                        transaction.Dispose();

                    }
                    catch (Exception ex)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex3)
                        {
                            vResultadoTransaccion.ResultadoDescripcion = ex3.Message.ToString();
                        }
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

        public async Task<ResultadoTransaccion<SapBaseResponse<SapDocument>>> SetCancelIncomingPayments(int docentry)
        {
            ResultadoTransaccion<SapBaseResponse<SapDocument>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapDocument>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("IncomingPayments({0})/Cancel", docentry);
                SapBaseResponse<SapDocument> data = await _connectServiceLayer.PostCancelAsyncSBA<SapBaseResponse<SapDocument>>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<string>> SetCreateIncomingPayments(string codcomprobante, long docentry ,List<BE_CuadreCaja> listCuadreCaja, SqlConnection conn, SqlTransaction transaction)
        {

            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ComprobanteElectronicoRepository comprobante = new ComprobanteElectronicoRepository(context, _configuration);
                ResultadoTransaccion<BE_ComprobanteElectronico> resultadoTransaccionComprobanteElectronico = await comprobante.GetComprobantesElectronicosXml(codcomprobante, string.Empty, listCuadreCaja[0].regupdateidusuario, 0, conn, transaction);

                if (resultadoTransaccionComprobanteElectronico.ResultadoCodigo == -1)
                {
                    transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionComprobanteElectronico.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                List<BE_ComprobanteElectronico> response = (List<BE_ComprobanteElectronico>)resultadoTransaccionComprobanteElectronico.dataList;

                #region <<< Pago >>>
                if (response[0].flg_gratuito == "0")
                {
                    SapIncomingPayments IncomingPayments = new SapIncomingPayments();

                    if (vResultadoTransaccion.ResultadoCodigo == 0)
                    {
                        foreach (var item in listCuadreCaja)
                        {
                            if (item.tipopago != "D")
                            {
                                DateTime? transferDate = null;

                                if (item.tipopago == "A") transferDate = response[0].fechaemision;

                                IncomingPayments = new SapIncomingPayments()
                                {
                                    DocType = "rCustomer",
                                    DocDate = response[0].fechaemision,
                                    CardCode = response[0].cardcode.Trim(),
                                    DocCurrency = response[0].c_simbolomoneda.Trim(),
                                    CashSum = (item.tipopago == "E") ? item.monto : 0,
                                    CheckAccount = (item.tipopago == "C") ? "10411004" : "",
                                    TransferAccount = (item.tipopago == "A") ? "10411002" : "",
                                    TransferSum = (item.tipopago == "A") ? item.monto : 0,
                                    TransferDate = transferDate,
                                    TransferReference = (item.tipopago == "A") ? item.numeroentidad : "",
                                    CounterReference = "",
                                    TaxDate = response[0].fechaemision,
                                    DocObjectCode = "bopot_IncomingPayments",
                                    DueDate = response[0].fechaemision,
                                    PaymentChecks = new List<SapPaymentChecks>(),
                                    PaymentInvoices = new List<SapPaymentInvoices>(),
                                    PaymentCreditCards = new List<SapPaymentCreditCards>()
                                };

                                //I-Factura a la cual esta asociado el Pago, se tiene que colocar el Total
                                var paymentInvoices = new SapPaymentInvoices()
                                {
                                    DocEntry = (int)docentry,
                                    SumApplied = item.monto,
                                    InvoiceType = "it_Invoice"
                                };
                                IncomingPayments.PaymentInvoices.Add(paymentInvoices);
                                //F-Factura a la cual esta asociado el Pago, se tiene que colocar el Total

                                //I-Cheque
                                if (item.tipopago == "C")
                                {
                                    object bankCode;

                                    using (SqlCommand cmd = new SqlCommand(SP_GET_TABLAS_CODIGO_BANCO, conn, transaction))
                                    {
                                        cmd.Parameters.Clear();
                                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                        cmd.Parameters.Add(new SqlParameter("@codigo", item.nombreentidad));

                                        bankCode = await cmd.ExecuteScalarAsync();
                                    }

                                    var paymentChecks = new SapPaymentChecks()
                                    {
                                        DueDate = response[0].fechaemision,
                                        CheckNumber = int.Parse(item.numeroentidad),
                                        BankCode = bankCode.ToString(),
                                        CheckSum = item.monto,
                                        Currency = response[0].c_simbolomoneda.Trim(),
                                        CountryCode = "PE"
                                    };
                                    IncomingPayments.PaymentChecks.Add(paymentChecks);
                                }
                                //F-Cheque

                                //I-Tarjeta crédito
                                if (item.tipopago == "T")
                                {
                                    int creditCard = 0;
                                    int paymentMethodCode = 0;

                                    using (SqlCommand cmd = new SqlCommand(SP_GET_TABLAS_CODIGO_TIPO_TARJETA, conn, transaction))
                                    {
                                        cmd.Parameters.Clear();
                                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                        cmd.Parameters.Add(new SqlParameter("@codigo", item.nombreentidad));

                                        using (var reader = await cmd.ExecuteReaderAsync())
                                        {
                                            while (await reader.ReadAsync())
                                            {
                                                creditCard = int.Parse(reader["valor2"].ToString());
                                                paymentMethodCode = int.Parse(reader["valor"].ToString());
                                            }
                                        }
                                    }

                                    var paymentCreditCards = new SapPaymentCreditCards()
                                    {
                                        CreditCard = creditCard,
                                        CreditCardNumber = item.numeroentidad,
                                        CardValidUntil = DateTime.Parse("2024/12/31"), // Revisar como se envia los Pagos a SAP
                                        VoucherNum = "000",
                                        PaymentMethodCode = paymentMethodCode,
                                        NumOfPayments = 1,
                                        FirstPaymentDue = response[0].fechaemision,
                                        FirstPaymentSum = item.monto,
                                        CreditSum = item.monto,
                                        CreditCur = response[0].c_simbolomoneda.Trim(),
                                        NumOfCreditPayments = 1
                                    };
                                    IncomingPayments.PaymentCreditCards.Add(paymentCreditCards);
                                }
                                //F-Tarjeta crédito
                                try
                                {
                                    string cadena = "IncomingPayments";

                                    SapBaseResponse<SapDocument> dataPago = new SapBaseResponse<SapDocument>();

                                    dataPago = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapDocument>>(cadena, IncomingPayments);

                                    if (dataPago.DocEntry != 0)
                                    {
                                        using (SqlCommand cmd = new SqlCommand(SP_POST_CUADRECAJA_INFO_SAP_UPDATE, conn, transaction))
                                        {
                                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                            cmd.Parameters.Add(new SqlParameter("@documento", codcomprobante));
                                            cmd.Parameters.Add(new SqlParameter("@doc_entry", docentry));
                                            cmd.Parameters.Add(new SqlParameter("@ide_trans", dataPago.DocEntry));
                                            cmd.Parameters.Add(new SqlParameter("@fec_enviosap", DateTime.Now));
                                            cmd.Parameters.Add(new SqlParameter("@correlativo", item.correlativo));

                                            await cmd.ExecuteNonQueryAsync();
                                        }
                                    }
                                    else
                                    {
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = "ERROR AL ENVIAR EL PAGO A SAP.";
                                        return vResultadoTransaccion;
                                    }

                                    vResultadoTransaccion.IdRegistro = 0;
                                    vResultadoTransaccion.ResultadoCodigo = 0;
                                    vResultadoTransaccion.ResultadoDescripcion = "PROCESADO CORRECTAMENTE";
                                }
                                catch (Exception ex)
                                {
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                                }
                            }
                        }
                    }
                }
                #endregion
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
