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
namespace Net.Data
{
    public class VentaCajaRepository : RepositoryBase<BE_VentasCabecera>, IVentaCajaRepository
    {
        private readonly string _cnx;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_VENTA_CABECERA_BY_CODVENTA = DB_ESQUEMA + "VEN_VentasCabecera_Consulta";
        const string SP_GET_VENTA_DETALLE_BY_CODVENTA = DB_ESQUEMA + "VEN_VentasDetallePorCodventaGet";
        const string SP_GET_RUC_CONSULTAV2_BY_FILTRO = DB_ESQUEMA + "Fa_Ruc_Consultav2";
        const string SP_GET_MDSYN_PAGOS_CONSULTA_BY_FILTRO = DB_ESQUEMA + "Sp_MdsynPagos_Consulta";
        //const string SP_GET_LIMITE_CONSUMO_PERSONAL_BY_CODPERSONAL = DB_ESQUEMA + "Sp_LimiteConsumoPersonal";
        const string SP_GET_LIMITE_CONSUMO_PERSONAL_BY_CODPERSONAL = DB_ESQUEMA + "VEN_LimiteConsumoPersonal";
        const string SP_GET_WEBSERVICES_CONSULTA_BY_CODTABLA = DB_ESQUEMA + "Sp_TCI_WebService_Consulta";
        const string SP_GET_CORRELATIVO_CONSULTA = DB_ESQUEMA + "Sp_Correlativo_Consulta";
        const string SP_GET_COMPROBANTE_INSERT = DB_ESQUEMA + "VEN_Comprobantes_Ins";
        const string SP_GET_DATOCARDCODE_CONSULTAR = DB_ESQUEMA + "Sp_DatoCardCode_Consulta";
        //const string SP_GET_COMPROBANTE_UPDATE_PERSO = DB_ESQUEMA + "Sp_Comprobantes_Update";
        const string SP_POST_COMPROBANTE_VAL_UPDATE = DB_ESQUEMA + "VEN_ComprobantesValUpd";

        
        public VentaCajaRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPorCodVenta(string codVenta)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VENTA_CABECERA_BY_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codigo", codVenta)); //codventa

                        BE_VentasCabecera response = new BE_VentasCabecera();

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.codatencion = ((reader["codatencion"]) is DBNull) ? string.Empty : reader["codatencion"].ToString().Trim();
                                response.fechagenera = ((reader["fechagenera"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechagenera"];
                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];
                                response.codcomprobante = ((reader["codcomprobante"]) is DBNull) ? string.Empty : reader["codcomprobante"].ToString().Trim();
                                response.planpoliza = ((reader["planpoliza"]) is DBNull) ? string.Empty : reader["planpoliza"].ToString().Trim();
                                response.codaseguradora = ((reader["codaseguradora"]) is DBNull) ? string.Empty : reader["codaseguradora"].ToString().Trim();
                                response.codcia = ((reader["codcia"]) is DBNull) ? string.Empty : reader["codcia"].ToString().Trim();
                                response.codventa = ((reader["codventa"]) is DBNull) ? string.Empty : reader["codventa"].ToString().Trim();
                                response.estado = ((reader["estado"]) is DBNull) ? string.Empty : reader["estado"].ToString().Trim();
                                response.cardcode = ((reader["cardcode"]) is DBNull) ? string.Empty : reader["cardcode"].ToString().Trim();
                                response.codcliente = ((reader["codcliente"]) is DBNull) ? string.Empty : reader["codcliente"].ToString().Trim();
                                response.codpaciente = ((reader["codpaciente"]) is DBNull) ? string.Empty : reader["codpaciente"].ToString().Trim();
                                response.nombre = ((reader["nombre"]) is DBNull) ? string.Empty : reader["nombre"].ToString().Trim();
                                response.codplan = ((reader["codplan"]) is DBNull) ? string.Empty : reader["codplan"].ToString().Trim();
                                response.porcentajedctoplan = ((reader["porcentajedctoplan"]) is DBNull) ? 0 : (decimal)reader["porcentajedctoplan"];
                                response.codtipocliente = ((reader["codtipocliente"]) is DBNull) ? string.Empty : reader["codtipocliente"].ToString().Trim();
                                response.porcentajecoaseguro = ((reader["porcentajecoaseguro"]) is DBNull) ? 0 : (decimal)reader["porcentajecoaseguro"];
                                response.porcentajeimpuesto = ((reader["porcentajeimpuesto"]) is DBNull) ? 0 : (decimal)reader["porcentajeimpuesto"];
                                response.montopaciente = ((reader["montopaciente"]) is DBNull) ? 0 : (decimal)reader["montopaciente"];
                                response.montoaseguradora = ((reader["montoaseguradora"]) is DBNull) ? 0 : (decimal)reader["montoaseguradora"];

                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];

                                response.nombreestado = ((reader["nombreestado"]) is DBNull) ? string.Empty : reader["nombreestado"].ToString().Trim();
                                response.nombretipocliente = ((reader["nombretipocliente"]) is DBNull) ? string.Empty : reader["nombretipocliente"].ToString().Trim();
                                //response.nombrealmacen = ((reader["nombrealmacen"]) is DBNull) ? string.Empty : (string)reader["nombrealmacen"];
                                response.nombreplan = ((reader["nombreplan"]) is DBNull) ? string.Empty : reader["nombreplan"].ToString().Trim();
                                //response.tienedevolucion = ((reader["tienedevolucion"]) is DBNull) ? false : (bool)reader["tienedevolucion"];
                                response.ruccliente = ((reader["ruccliente"]) is DBNull) ? string.Empty : reader["ruccliente"].ToString().Trim();
                                response.dircliente = ((reader["dircliente"]) is DBNull) ? string.Empty : reader["dircliente"].ToString().Trim();
                                response.tipdocidentidad = ((reader["tipdocidentidad"]) is DBNull) ? string.Empty : reader["tipdocidentidad"].ToString().Trim();
                                response.docidentidad = ((reader["docidentidad"]) is DBNull) ? string.Empty : reader["docidentidad"].ToString().Trim();
                                response.nombretipdocidentidad = ((reader["nombretipdocidentidad"]) is DBNull) ? string.Empty : reader["nombretipdocidentidad"].ToString().Trim();
                                response.autorizado = ((reader["autorizado"]) is DBNull) ? string.Empty : reader["autorizado"].ToString().Trim();
                                response.correocliente = ((reader["correocliente"]) is DBNull) ? string.Empty : reader["correocliente"].ToString().Trim();
                                response.moneda_comprobantes = ((reader["moneda_comprobantes"]) is DBNull) ? string.Empty : reader["moneda_comprobantes"].ToString().Trim();
                                response.flg_gratuito = ((reader["flg_gratuito"]) is DBNull) ? false : (bool)reader["flg_gratuito"];
                                response.strTienedevolucion = ((reader["tienedevolucion"]) is DBNull) ? string.Empty : reader["tienedevolucion"].ToString();
                                response.tipomovimiento = ((reader["tipomovimiento"]) is DBNull) ? string.Empty : (string)reader["tipomovimiento"];

                            }
                        }

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
        public async Task<ResultadoTransaccion<string>> GetDatoCardCodeConsulta(string tipoCliente,string codCliente)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string cardcode = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DATOCARDCODE_CONSULTAR, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@tctip_cliente", tipoCliente));
                        cmd.Parameters.Add(new SqlParameter("@tccodigo", codCliente));

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cardcode = ((reader["cardcode"]) is DBNull) ? string.Empty : reader["cardcode"].ToString().Trim();
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = cardcode;

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
        public async Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaDetallePorCodVenta(string codVenta)
        {
            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VENTA_DETALLE_BY_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codventa", codVenta));

                        List<BE_VentasDetalle> ListaResponse = new List<BE_VentasDetalle>();
                        var response = new BE_VentasDetalle();

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = new BE_VentasDetalle();
                                response.codproducto = ((reader["codproducto"]) is DBNull) ? string.Empty : reader["codproducto"].ToString().Trim();
                                response.nombreproducto = ((reader["nombreproducto"]) is DBNull) ? string.Empty : (string)reader["nombreproducto"].ToString().Trim();
                                response.cantidad = ((reader["cantidad"]) is DBNull) ? 0 : (int)reader["cantidad"];
                                response.cantidad_fraccion = ((reader["cantidad_fraccion"]) is DBNull) ? 0 : (int)reader["cantidad_fraccion"];
                                response.cnt_unitario = ((reader["cnt_unitario"]) is DBNull) ? 0 : (int)reader["cnt_unitario"];
                                response.prc_unitario = ((reader["prc_unitario"]) is DBNull) ? 0 : (decimal)reader["prc_unitario"];
                                response.precioventaPVP = ((reader["precioventaPVP"]) is DBNull) ? 0 : (decimal)reader["precioventaPVP"];
                                response.porcentajedctoproducto = ((reader["porcentajedctoproducto"]) is DBNull) ? 0 : (decimal)reader["porcentajedctoproducto"];
                                response.porcentajedctoplan = ((reader["porcentajedctoplan"]) is DBNull) ? 0 : (decimal)reader["porcentajedctoplan"];
                                response.montototal = ((reader["montototal"]) is DBNull) ? 0 : (decimal)reader["montototal"];
                                response.montopaciente = ((reader["montopaciente"]) is DBNull) ? 0 : (decimal)reader["montopaciente"];
                                response.montoaseguradora = ((reader["montoaseguradora"]) is DBNull) ? 0 : (decimal)reader["montoaseguradora"];
                                response.valorVVP = ((reader["valorVVP"]) is DBNull) ? 0 : (decimal)reader["valorVVP"];
                                ListaResponse.Add(response);

                            }

                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", ListaResponse.Count());
                        vResultadoTransaccion.dataList = ListaResponse;

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

        public async Task<ResultadoTransaccion<AsegContPaci>> GetRucConsultav2PorFiltro(string codPaciente,string codAseguradora,string codCia)
        {
            ResultadoTransaccion<AsegContPaci> vResultadoTransaccion = new ResultadoTransaccion<AsegContPaci>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RUC_CONSULTAV2_BY_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codPaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codAseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", codCia));

                        SqlParameter oParamRucaseg = new SqlParameter("@rucaseg", SqlDbType.Char, 11)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamRucaseg);

                        SqlParameter oParamNombreaseg = new SqlParameter("@nombreaseg", SqlDbType.Char, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamNombreaseg);

                        SqlParameter oParamDireccionaseg = new SqlParameter("@direccionaseg", SqlDbType.Char, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamDireccionaseg);

                        //
                        SqlParameter oParamRuccia = new SqlParameter("@ruccia", SqlDbType.Char, 11)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamRuccia);

                        SqlParameter oParamNombrecia = new SqlParameter("@nombrecia", SqlDbType.Char, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamNombrecia);

                        SqlParameter oParamDireccioncia = new SqlParameter("@direccioncia", SqlDbType.Char, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamDireccioncia);

                        SqlParameter oParamRucpac = new SqlParameter("@rucpac", SqlDbType.NVarChar, 11)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamRucpac);

                        SqlParameter oParamNombrepac = new SqlParameter("@nombrepac", SqlDbType.NVarChar, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamNombrepac);

                        SqlParameter oParamDireccionpac = new SqlParameter("@direccionpac", SqlDbType.NVarChar, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamDireccionpac);

                        SqlParameter oParamCorreoaseg = new SqlParameter("@correoaseg", SqlDbType.NVarChar, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCorreoaseg);

                        SqlParameter oParamCorreocia = new SqlParameter("@correocia", SqlDbType.NVarChar, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCorreocia);

                        SqlParameter oParamCorreopac = new SqlParameter("@correopac", SqlDbType.NVarChar, 60)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCorreopac);

                        SqlParameter oParamCardcodeA = new SqlParameter("@cardcodeA", SqlDbType.NVarChar, 15)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCardcodeA);

                        SqlParameter oParamCardcodeC = new SqlParameter("@cardcodeC", SqlDbType.NVarChar, 15)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCardcodeC);

                        SqlParameter oParamCardcodeP = new SqlParameter("@cardcodeP", SqlDbType.NVarChar, 15)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamCardcodeP);


                        AsegContPaci response = new AsegContPaci();

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        //using (var reader = await cmd.ExecuteReaderAsync())
                        //{
                        //    while (await reader.ReadAsync())
                        //    {
                        //        response.seguradoraRuc = ((reader["@rucaseg"]) is DBNull) ? string.Empty : (string)reader["@rucaseg"];
                        //        response.seguradoraNombre = ((reader["@nombreaseg"]) is DBNull) ? string.Empty : (string)reader["@nombreaseg"];
                        //        response.seguradoraDireccion = ((reader["@direccionaseg"]) is DBNull) ? string.Empty : (string)reader["@direccionaseg"];
                        //        response.seguradoraCorreo = ((reader["@correoaseg"]) is DBNull) ? string.Empty : (string)reader["@correoaseg"];
                        //        response.seguradoraCardCode = ((reader["@cardcodeA"]) is DBNull) ? string.Empty : (string)reader["@cardcodeA"];

                        //        response.contratanteRuc = ((reader["@ruccia"]) is DBNull) ? string.Empty : (string)reader["@ruccia"];
                        //        response.contratanteNombre = ((reader["@nombrecia"]) is DBNull) ? string.Empty : (string)reader["@nombrecia"];
                        //        response.contratanteDireccion = ((reader["@direccioncia"]) is DBNull) ? string.Empty : (string)reader["@direccioncia"];
                        //        response.contratanteCorreo = ((reader["@correocia"]) is DBNull) ? string.Empty : (string)reader["@correocia"];
                        //        response.contratanteCardCode = ((reader["@cardcodeC"]) is DBNull) ? string.Empty : (string)reader["@cardcodeC"];

                        //        response.pacienteRuc = ((reader["@rucpac"]) is DBNull) ? string.Empty : (string)reader["@rucpac"];
                        //        response.pacienteNombre = ((reader["@nombrepac"]) is DBNull) ? string.Empty : (string)reader["@nombrepac"];
                        //        response.pacienteDireccion = ((reader["@direccionpac"]) is DBNull) ? string.Empty : (string)reader["@direccionpac"];
                        //        response.pacienteCorreo = ((reader["@correopac"]) is DBNull) ? string.Empty : (string)reader["@correopac"];
                        //        response.pacienteCardCode = ((reader["@cardcodeP"]) is DBNull) ? string.Empty : (string)reader["@cardcodeP"];

                        //    }
                        //}

                        response.seguradoraRuc = oParamRucaseg.Value.ToString().Trim();
                        response.seguradoraNombre = oParamNombreaseg.Value.ToString().Trim();
                        response.seguradoraDireccion = oParamDireccionaseg.Value.ToString().Trim();
                        response.seguradoraCorreo = oParamCorreoaseg.Value.ToString().Trim();
                        response.seguradoraCardCode = oParamCardcodeA.Value.ToString().Trim();

                        response.contratanteRuc = oParamRuccia.Value.ToString().Trim();
                        response.contratanteNombre = oParamNombrecia.Value.ToString().Trim();
                        response.contratanteDireccion = oParamDireccioncia.Value.ToString().Trim();
                        response.contratanteCorreo = oParamCorreocia.Value.ToString().Trim();
                        response.contratanteCardCode = oParamCardcodeC.Value.ToString().Trim();

                        response.pacienteRuc = oParamRucpac.Value.ToString().Trim();
                        response.pacienteNombre = oParamNombrepac.Value.ToString().Trim();
                        response.pacienteDireccion = oParamDireccionpac.Value.ToString().Trim();
                        response.pacienteCorreo = oParamCorreopac.Value.ToString().Trim();
                        response.pacienteCardCode = oParamCardcodeP.Value.ToString().Trim();

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

        public async Task<ResultadoTransaccion<B_MdsynPagos>> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva,string cod_liquidacion,string cod_venta,int orden)
        {
            ResultadoTransaccion<B_MdsynPagos> vResultadoTransaccion = new ResultadoTransaccion<B_MdsynPagos>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_MDSYN_PAGOS_CONSULTA_BY_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ide_pagos_bot", ide_pagos_bot));
                        cmd.Parameters.Add(new SqlParameter("@ide_mdsyn_reserva", ide_mdsyn_reserva));
                        cmd.Parameters.Add(new SqlParameter("@ide_correl_reserva", ide_correl_reserva));
                        cmd.Parameters.Add(new SqlParameter("@cod_liquidacion", cod_liquidacion));
                        cmd.Parameters.Add(new SqlParameter("@cod_venta", cod_venta));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        B_MdsynPagos response = new B_MdsynPagos();

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.ide_pagos_bot = ((reader["@ide_pagos_bot"]) is DBNull) ? 0 : (int)reader["@ide_pagos_bot"];
                                response.est_pagado = ((reader["@est_pagado"]) is DBNull) ? string.Empty : (string)reader["@est_pagado"];
                                response.nro_operacion = ((reader["@nro_operacion"]) is DBNull) ? string.Empty : (string)reader["@nro_operacion"];
                                response.tip_tarjeta = ((reader["@tip_tarjeta"]) is DBNull) ? string.Empty : (string)reader["@tip_tarjeta"];
                                response.num_tarjeta = ((reader["@num_tarjeta"]) is DBNull) ? string.Empty : (string)reader["@num_tarjeta"];
                                response.cnt_monto_pago = ((reader["@cnt_monto_pago"]) is DBNull) ? 0 : (decimal)reader["@cnt_monto_pago"];
                                response.terminal = ((reader["@terminal"]) is DBNull) ? string.Empty : (string)reader["@terminal"];
                                response.flg_pago_usado = ((reader["@flg_pago_usado"]) is DBNull) ? string.Empty : (string)reader["@flg_pago_usado"];
                                
                            }
                        }

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

        public async Task<ResultadoTransaccion<object>> GetLimiteConsumoPersonalPorCodPersonal(string codPersonal)
        {
            ResultadoTransaccion<object> vResultadoTransaccion = new ResultadoTransaccion<object>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIMITE_CONSUMO_PERSONAL_BY_CODPERSONAL, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpersonal", codPersonal));

                        await conn.OpenAsync();

                        var objSetVista = new object();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                               var  objVista = new { 
                                    fecha1 = ((reader["fecha1"]) is DBNull) ? string.Empty : reader["fecha1"].ToString(), 
                                    fecha2 = ((reader["fecha1"]) is DBNull) ? string.Empty : reader["fecha1"].ToString(),
                                    montolimite = (reader["montolimite"] is DBNull) ? 0 : (decimal)reader["montolimite"],
                                    montoconsumo = (reader["montoconsumo"] is DBNull) ? 0 : (decimal)reader["montoconsumo"],
                                    vender = ((reader["vender"]) is DBNull) ? string.Empty : reader["vender"].ToString(),
                                    codpersonal = ((reader["codpersonal"]) is DBNull) ? string.Empty : reader["codpersonal"].ToString(),
                                    nombre = ((reader["nombre"]) is DBNull) ? string.Empty : reader["nombre"].ToString()
                               };
                                objSetVista = objVista;
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = objSetVista;
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

        public async Task<ResultadoTransaccion<object>> GetCorrelativoConsulta()
        {
            // por el momento no se usa, se est usando SerieRepository GetListConfigDocumentoPorNombreMaquina
            
            ResultadoTransaccion<object> vResultadoTransaccion = new ResultadoTransaccion<object>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CORRELATIVO_CONSULTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        
                        await conn.OpenAsync();

                        var objSetVista = new object();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                var objVista = new
                                {
                                    boleta = ((reader["Boleta"]) is DBNull) ? string.Empty : reader["Boleta"].ToString(),
                                    factura = ((reader["Factura"]) is DBNull) ? string.Empty : reader["Factura"].ToString(),
                                    creditoB = (reader["CreditoB"] is DBNull) ? string.Empty : reader["CreditoB"],
                                    creditoF = (reader["CreditoF"] is DBNull) ? string.Empty : reader["CreditoF"],
                                    flg_electronico = ((reader["flg_electronico"]) is DBNull) ? string.Empty : reader["flg_electronico"].ToString(),
                                    generar_e = ((reader["generar_e"]) is DBNull) ? string.Empty : reader["generar_e"].ToString(),
                                    flg_otorgarf = (reader["flg_otorgarf"] is DBNull) ? string.Empty : reader["flg_otorgarf"],
                                    flg_otorgarb = (reader["flg_otorgarb"] is DBNull) ? string.Empty : reader["flg_otorgarb"]

                                };
                                objSetVista = objVista;
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = objSetVista;
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

      
        public async Task<ResultadoTransaccion<string>> ComprobantesRegistrar(BE_Comprobante value,string tipocomprobante,string correo,string codTipoafectacionigv,bool wFlg_electronico,string maquina)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            string codcomprobante = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        //REQ_ValeSalidaIns
                        using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTE_INSERT, conn, transaction))
                        {
                            
                            int li_contador = 0;
                            string variostipopago = "";
                            foreach (var item in value.cuadreCaja)
                            {
                                li_contador = li_contador + 1;
                                variostipopago = (li_contador == 1) ? "N" : "S";
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@tipocomprobante", tipocomprobante));
                                cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                                cmd.Parameters.Add(new SqlParameter("@monto", item.monto));
                                cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
                                cmd.Parameters.Add(new SqlParameter("@anombrede", value.anombrede));
                                cmd.Parameters.Add(new SqlParameter("@ruc", value.ruc));
                                cmd.Parameters.Add(new SqlParameter("@direccion", value.direccion));
                                cmd.Parameters.Add(new SqlParameter("@tipopago", item.tipopago));
                                cmd.Parameters.Add(new SqlParameter("@nombreentidad", item.nombreentidad.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@descripcionentidad", item.descripcionentidad.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@numeroentidad", item.numeroentidad.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@codterminal", item.codterminal));
                                cmd.Parameters.Add(new SqlParameter("@operacion", "T"));
                                cmd.Parameters.Add(new SqlParameter("@variostipopago", variostipopago));
                                cmd.Parameters.Add(new SqlParameter("@strcodcomprobante", codcomprobante));
                                cmd.Parameters.Add(new SqlParameter("@tipodecambio", value.tipodecambio));
                                cmd.Parameters.Add(new SqlParameter("@maquina", maquina));
                                
                                SqlParameter oParam = new SqlParameter("@codcomprobante", SqlDbType.Char, 11)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmd.Parameters.Add(oParam);

                                await cmd.ExecuteNonQueryAsync();

                                codcomprobante  = cmd.Parameters["@codcomprobante"].Value.ToString();
                                if(codcomprobante.Equals("")) throw new Exception("Error al insertar comprobante !!");
                                vResultadoTransaccion.IdRegistro = 0;
                                vResultadoTransaccion.ResultadoCodigo = 0;
                                vResultadoTransaccion.ResultadoDescripcion = "SE REGISTRO CORRECTAMENTE";
                                vResultadoTransaccion.data = codcomprobante;

                            }
                        }

                        if (codcomprobante != string.Empty) {
                            value.codcomprobante = codcomprobante;
                                //Actualizando valores

                                using (SqlCommand cmd = new SqlCommand(SP_POST_COMPROBANTE_VAL_UPDATE, conn, transaction))
                                {
                                  
                                    cmd.Parameters.Clear();
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    //cmd.Parameters.Add(new SqlParameter("@coduser", value.coduser));
                                    cmd.Parameters.Add(new SqlParameter("@idusuario", value.idusuario));
                                    //cmd.Parameters.Add(new SqlParameter("@codcentro", value.codcentro));// añadimos por bd  SBALogistica
                                    cmd.Parameters.Add(new SqlParameter("@cardcode", value.cardcode)); 
                                    cmd.Parameters.Add(new SqlParameter("@tipdocidentidad", value.tipdocidentidad));// default null
                                    cmd.Parameters.Add(new SqlParameter("@docidentidad", value.docidentidad));// default null
                                    // inicio clinica comprobante electronico
                                    cmd.Parameters.Add(new SqlParameter("@flg_gratuito", value.flg_gratuito));
                                    cmd.Parameters.Add(new SqlParameter("@correo", correo));
                                    cmd.Parameters.Add(new SqlParameter("@tipoafectacionigv", codTipoafectacionigv));
                                    // fin clinica comprobante electronico
                                    cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));

                                    await cmd.ExecuteNonQueryAsync();

                                    vResultadoTransaccion.IdRegistro = 0;
                                    vResultadoTransaccion.ResultadoCodigo = 0;
                                    vResultadoTransaccion.ResultadoDescripcion = "SE REGISTRO CORRECTAMENTE";
                                    vResultadoTransaccion.data = codcomprobante;
                             
                                }
                        }

                        if (wFlg_electronico) { 
                        


                        }

                        //bool wFlg_electronico

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
                        //throw;
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

       

        #region "Consulta Servicios"

        public async Task<ResultadoTransaccion<string>> GetWebServicesPorCodTabla(string codTabla)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_WEBSERVICES_CONSULTA_BY_CODTABLA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codtabla", codTabla));
                       
                        await conn.OpenAsync();
                        string nombre = string.Empty;
                        vResultadoTransaccion.ResultadoDescripcion = string.Empty;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                nombre = (reader["nombre"] is DBNull) ? string.Empty : (string)reader["nombre"];
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = "test";

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

        //SP_GET_CORRELATIVO_CONSULTA
        #endregion

        #region Validaciones
        //EFACT_FLAG_GENERAFACTURA
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetFlagGenerarFactura(string codVenta)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VENTA_CABECERA_BY_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codigo", codVenta)); //codventa

                        BE_VentasCabecera response = new BE_VentasCabecera();

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.fechagenera = ((reader["fechagenera"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechagenera"];
                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];
                                response.codcomprobante = ((reader["codcomprobante"]) is DBNull) ? string.Empty : reader["codcomprobante"].ToString().Trim();
                                response.planpoliza = ((reader["planpoliza"]) is DBNull) ? string.Empty : reader["planpoliza"].ToString().Trim();
                                response.codaseguradora = ((reader["codaseguradora"]) is DBNull) ? string.Empty : reader["codaseguradora"].ToString().Trim();
                                response.codcia = ((reader["codcia"]) is DBNull) ? string.Empty : reader["codcia"].ToString().Trim();
                                response.codventa = ((reader["codventa"]) is DBNull) ? string.Empty : reader["codventa"].ToString().Trim();
                                response.estado = ((reader["estado"]) is DBNull) ? string.Empty : reader["estado"].ToString().Trim();
                                response.cardcode = ((reader["cardcode"]) is DBNull) ? string.Empty : reader["cardcode"].ToString().Trim();
                                response.codcliente = ((reader["codcliente"]) is DBNull) ? string.Empty : reader["codcliente"].ToString().Trim();
                                response.codpaciente = ((reader["codpaciente"]) is DBNull) ? string.Empty : reader["codpaciente"].ToString().Trim();
                                response.nombre = ((reader["nombre"]) is DBNull) ? string.Empty : reader["nombre"].ToString().Trim();
                                response.codplan = ((reader["codplan"]) is DBNull) ? string.Empty : reader["codplan"].ToString().Trim();
                                response.porcentajedctoplan = ((reader["porcentajedctoplan"]) is DBNull) ? 0 : (decimal)reader["porcentajedctoplan"];
                                response.codtipocliente = ((reader["codtipocliente"]) is DBNull) ? string.Empty : reader["codtipocliente"].ToString().Trim();
                                response.porcentajecoaseguro = ((reader["porcentajecoaseguro"]) is DBNull) ? 0 : (decimal)reader["porcentajecoaseguro"];
                                response.porcentajeimpuesto = ((reader["porcentajeimpuesto"]) is DBNull) ? 0 : (decimal)reader["porcentajeimpuesto"];
                                response.montopaciente = ((reader["montopaciente"]) is DBNull) ? 0 : (decimal)reader["montopaciente"];
                                response.montoaseguradora = ((reader["montoaseguradora"]) is DBNull) ? 0 : (decimal)reader["montoaseguradora"];

                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];

                                response.nombreestado = ((reader["nombreestado"]) is DBNull) ? string.Empty : reader["nombreestado"].ToString().Trim();
                                response.nombretipocliente = ((reader["nombretipocliente"]) is DBNull) ? string.Empty : reader["nombretipocliente"].ToString().Trim();
                                //response.nombrealmacen = ((reader["nombrealmacen"]) is DBNull) ? string.Empty : (string)reader["nombrealmacen"];
                                response.nombreplan = ((reader["nombreplan"]) is DBNull) ? string.Empty : reader["nombreplan"].ToString().Trim();
                                //response.tienedevolucion = ((reader["tienedevolucion"]) is DBNull) ? false : (bool)reader["tienedevolucion"];
                                response.ruccliente = ((reader["ruccliente"]) is DBNull) ? string.Empty : reader["ruccliente"].ToString().Trim();
                                response.dircliente = ((reader["dircliente"]) is DBNull) ? string.Empty : reader["dircliente"].ToString().Trim();
                                response.tipdocidentidad = ((reader["tipdocidentidad"]) is DBNull) ? string.Empty : reader["tipdocidentidad"].ToString().Trim();
                                response.docidentidad = ((reader["docidentidad"]) is DBNull) ? string.Empty : reader["docidentidad"].ToString().Trim();
                                response.nombretipdocidentidad = ((reader["nombretipdocidentidad"]) is DBNull) ? string.Empty : reader["nombretipdocidentidad"].ToString().Trim();
                                response.autorizado = ((reader["autorizado"]) is DBNull) ? string.Empty : reader["autorizado"].ToString().Trim();
                                response.correocliente = ((reader["correocliente"]) is DBNull) ? string.Empty : reader["correocliente"].ToString().Trim();
                                response.moneda_comprobantes = ((reader["moneda_comprobantes"]) is DBNull) ? string.Empty : reader["moneda_comprobantes"].ToString().Trim();
                                response.flg_gratuito = ((reader["flg_gratuito"]) is DBNull) ? false : (bool)reader["flg_gratuito"];

                            }
                        }

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

        //
        #endregion

    }
}
