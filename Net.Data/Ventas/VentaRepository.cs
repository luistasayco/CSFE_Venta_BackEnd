﻿using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Data;
using System.Linq;
using System.Net.Http;
using Net.CrossCotting;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Xml.Serialization;
using Net.TCI;
using MSXML2;

namespace Net.Data
{
    public class VentaRepository : RepositoryBase<BE_VentasCabecera>, IVentaRepository
    {
        private readonly string _cnx;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaVentasCabeceraPorFiltrosGet";
        const string SP_GET_SIN_STOCK = DB_ESQUEMA + "VEN_ListaVentasCabeceraSinStockPorFiltrosGet";
        const string SP_GET_VENTA_CABECERA = DB_ESQUEMA + "VEN_ListaVentasCabeceraPorCodVentaGet";
        const string SP_GET_CABECERA_VENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ObtieneVentasCabeceraPorCodVentaGet";
        const string SP_GET_DETALLEVENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ListaVentaDetallePorCodVentaGet";
        const string SP_GET_DETALLEVENTA_LOTE_POR_CODVENTA = DB_ESQUEMA + "VEN_ListaVentaDetalleLotePorCodVentaDetalleGet";
        const string SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO = DB_ESQUEMA + "VEN_ListaVentasCabeceraPendientesPorFiltrosGet";
        const string SP_GET_VENTAS_CHEQUEA_1MES_ANTES = DB_ESQUEMA + "VEN_VentasChequea1MesAntesGet";
        const string SP_GET_VALIDA_EXISTE_VENTA_ANULACION = DB_ESQUEMA + "VEN_VentaValidaExisteAnulacionGet";
        const string SP_GET_PROJECT_POR_FILTRO = DB_ESQUEMA + "VEN_ListaProyectosPorFiltrosGet";
        const string SP_COMPROBANTE_ELECTRONICO_UPDATE = DB_ESQUEMA + "VEN_ComprobantesElectronicos_Update"; //clinica
        // SIN STOCK
        const string SP_UPDATE_CABECERA_SIN_STOCK = DB_ESQUEMA + "VEN_VentaSinStockUpd";
        const string SP_UPDATE_CABECERA_SAP = DB_ESQUEMA + "VEN_VentaSAPUpd";
        const string SP_UPDATE_CABECERA_NOTA_SAP = DB_ESQUEMA + "VEN_NotaCreditoSAPUpd";
        const string SP_POST_CUADRECAJA_NOTA_SAP_UPDATE = DB_ESQUEMA + "VEN_CuadreCajaNotaCreditoSapUpd";
        // Validaciones de la venta
        //const string SP_GET_TIPOPRODUCTOPRESTACIONES = DB_ESQUEMA + "VEN_TipoproductoPrestacionesPorFiltroGet";
        //const string SP_GET_GNCXVENTA = DB_ESQUEMA + "VEN_GncxVentaPorVentaGet";
        //const string SP_GET_VALIDA_PRESOTOR = DB_ESQUEMA + "VEN_ValidaPresotorPorFiltroGet";
        const string SP_GET_CAMA_POR_ATENCION = DB_ESQUEMA + "VEN_ListaCamaPorAtencionGet";
        const string SP_GET_TIPOPRODUCTO_PRESTACION = DB_ESQUEMA + "VEN_ListaTipoProductoPrestacionVentaAutomaticaGet";

        const string SP_UPDATE_CABECERA_VENTA_ENVIO_PISO = DB_ESQUEMA + "VEN_VentaCabeceraEnvioPisoUpd";

        //const string SP_INSERT = DB_ESQUEMA + "VEN_VentaCabeceraIns";
        const string SP_INSERT_XML = DB_ESQUEMA + "VEN_VentaXmlIns";
        //const string SP_INSERT_DETALLE = DB_ESQUEMA + "VEN_VentaDetalleIns";
        //const string SP_INSERT_DETALLE_DATOS = DB_ESQUEMA + "VEN_VentaDetalleDatosIns";
        //const string SP_INSERT_DETALLE_LOTE = DB_ESQUEMA + "VEN_VentaDetalleLoteIns";
        const string SP_INSERT_DEVOLUCION = DB_ESQUEMA + "VEN_VentaDevolucionIns";
        const string SP_INSERT_DEVOLUCION_PEDIDO = DB_ESQUEMA + "VEN_VentaPedidoDevolucionIns";

        //const string SP_UPDATE = DB_ESQUEMA + "VEN_VentaCabeceraUpd";
        //const string SP_UPDATE_DSCTO_DETALLE = DB_ESQUEMA + "VEN_VentaCabeceraUpdDscDet";
        const string SP_INSERT_VENTA_ANULAR = DB_ESQUEMA + "VEN_VentaAnularIns";
        const string SP_INSERT_VENTA_PEDIDO_ERROR = DB_ESQUEMA + "VEN_VentasPedidoErrorIns";


        // Clinica
        //const string SP_PEDIDOXDEVOLVER_RECALCULO = DB_ESQUEMA + "VEN_ClinicaPedidosxDevolver_Recalculo";
        //const string SP_GET_PRESOTOR_CONSULTAVARIOS = DB_ESQUEMA + "VEN_ClinicaPresotorConsultaVariosGet";
        //const string SP_UPDATE_PRESOTOR = DB_ESQUEMA + "VEN_ClinicaPresotorUpd";
        //const string SP_UPDATE_PEDIDO = DB_ESQUEMA + "VEN_ClinicaPedidosUpd";
        //const string SP_FARMACIA_PRESTACION_INSERT = DB_ESQUEMA + "VEN_ClinicaFarmaciaPrestacionIns";
        const string SP_PRESOTOR_CONSULTA = DB_ESQUEMA + "Sp_Presotor_Consulta";

        public VentaRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
        }
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin, string codatencion)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fecinicio = Utilidades.GetFechaHoraInicioActual(fecinicio);
            fecfin = Utilidades.GetFechaHoraFinActual(fecfin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasCabecera>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));
                        cmd.Parameters.Add(new SqlParameter("@fecinicio", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fecfin", fecfin));
                        cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasCabecera>)context.ConvertTo<BE_VentasCabecera>(reader);
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetAllSinStock(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fecinicio = Utilidades.GetFechaHoraInicioActual(fecinicio);
            fecfin = Utilidades.GetFechaHoraFinActual(fecfin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasCabecera>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_SIN_STOCK, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));
                        cmd.Parameters.Add(new SqlParameter("@fecinicio", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fecfin", fecfin));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasCabecera>)context.ConvertTo<BE_VentasCabecera>(reader);
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

        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetListVentaCabecera(string codventa)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasCabecera>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VENTA_CABECERA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasCabecera>)context.ConvertTo<BE_VentasCabecera>(reader);
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
        public async Task<ResultadoTransaccion<SapProject>> GetListProjectPorFiltro(string codventa, string codpresotor, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<SapProject> vResultadoTransaccion = new ResultadoTransaccion<SapProject>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var response = new List<SapProject>();
                using (SqlCommand cmd = new SqlCommand(SP_GET_PROJECT_POR_FILTRO, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codventa", codventa));
                    cmd.Parameters.Add(new SqlParameter("@codpresotor", codpresotor));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<SapProject>)context.ConvertTo<SapProject>(reader);
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaPorCodVenta(string codventa)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new BE_VentasCabecera();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_VENTA_POR_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_VentasCabecera>(reader);
                        }

                        conn.Close();

                    }

                    var responseDetalle = new List<BE_VentasDetalle>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_POR_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            responseDetalle = (List<BE_VentasDetalle>)context.ConvertTo<BE_VentasDetalle>(reader);
                        }

                        conn.Close();
                    }

                    var responseDetalleLote = new List<BE_VentasDetalleLote>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_LOTE_POR_CODVENTA, conn))
                    {
                        foreach (BE_VentasDetalle item in responseDetalle)
                        {
                            if (item.manBtchNum || item.binactivat)
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

                                conn.Open();

                                using (var reader = await cmd.ExecuteReaderAsync())
                                {
                                    responseDetalleLote = (List<BE_VentasDetalleLote>)context.ConvertTo<BE_VentasDetalleLote>(reader);
                                }

                                conn.Close();

                                responseDetalle.Find(xFila => xFila.coddetalle == item.coddetalle).listVentasDetalleLotes = responseDetalleLote;
                            }
                        }
                    }

                    response.listaVentaDetalle = responseDetalle;

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaPorCodVenta(string codventa, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var response = new BE_VentasCabecera();

                using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_VENTA_POR_CODVENTA, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                    //conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = context.Convert<BE_VentasCabecera>(reader);
                    }

                    //conn.Close();

                }

                var responseDetalle = new List<BE_VentasDetalle>();

                using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_POR_CODVENTA, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                    //conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        responseDetalle = (List<BE_VentasDetalle>)context.ConvertTo<BE_VentasDetalle>(reader);
                    }

                    //conn.Close();
                }

                var responseDetalleLote = new List<BE_VentasDetalleLote>();

                using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_LOTE_POR_CODVENTA, conn, transaction))
                {
                    foreach (BE_VentasDetalle item in responseDetalle)
                    {
                        if (item.manBtchNum || item.binactivat)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

                            //conn.Open();

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                responseDetalleLote = (List<BE_VentasDetalleLote>)context.ConvertTo<BE_VentasDetalleLote>(reader);
                            }

                            //conn.Close();

                            responseDetalle.Find(xFila => xFila.coddetalle == item.coddetalle).listVentasDetalleLotes = responseDetalleLote;
                        }
                    }
                }

                response.listaVentaDetalle = responseDetalle;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.data = response;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }
        public async Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaDetallePorCodVenta(string codventa)
        {
            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var responseDetalle = new List<BE_VentasDetalle>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_POR_CODVENTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            responseDetalle = (List<BE_VentasDetalle>)context.ConvertTo<BE_VentasDetalle>(reader);
                        }

                        conn.Close();
                    }


                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.dataList = responseDetalle;
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
        public async Task<ResultadoTransaccion<BE_VentasDetalleLote>> GetDetalleLoteVentaPorCodDetalle(string coddetalle)
        {
            ResultadoTransaccion<BE_VentasDetalleLote> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalleLote>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    

                    var responseDetalleLote = new List<BE_VentasDetalleLote>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_LOTE_POR_CODVENTA, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@coddetalle", coddetalle));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            responseDetalleLote = (List<BE_VentasDetalleLote>)context.ConvertTo<BE_VentasDetalleLote>(reader);
                        }

                        conn.Close();
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.dataList = responseDetalleLote;
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPendientePorFiltro(DateTime fecha)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            var fecinicio = Utilidades.GetFechaHoraInicioActual(fecha);
            var fecfin = Utilidades.GetFechaHoraFinActual(fecha);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasCabecera>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechaini", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fecfin));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasCabecera>)context.ConvertTo<BE_VentasCabecera>(reader);
                        }

                        conn.Close();
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
        public async Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentaChequea1MesPorFiltro(string codpaciente, int cuantosmesesantes)
        {
            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasDetalle>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@cuantosmesesantes", cuantosmesesantes));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasDetalle>)context.ConvertTo<BE_VentasDetalle>(reader);
                        }

                        conn.Close();
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> ModificarVentaCabeceraEnvioPiso(BE_VentasCabecera value)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (CommittableTransaction transaction = new CommittableTransaction())
                    {
                        await conn.OpenAsync();
                        conn.EnlistTransaction(transaction);
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CABECERA_VENTA_ENVIO_PISO, conn))
                            {
                                cmd.Parameters.Clear();

                                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                                cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));

                                SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmd.Parameters.Add(outputIdTransaccionParam);

                                SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmd.Parameters.Add(outputMsjTransaccionParam);

                                await cmd.ExecuteNonQueryAsync();

                                vResultadoTransaccion.IdRegistro = 0;
                                vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                                vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
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
        public async Task<ResultadoTransaccion<Boolean>> GetGastoCubiertoPorFiltro(string codaseguradora, string codproducto, int tipoatencion)
        {
            ResultadoTransaccion<Boolean> vResultadoTransaccion = new ResultadoTransaccion<Boolean>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                TablaRepository tablaRepository = new TablaRepository(context, _configuration);

                ResultadoTransaccion<BE_Tabla> resultadoTransaccionTabla = await tablaRepository.GetTablaLogisticaPorFiltros("ESTADOVENTAGASTOCUBIERTOV2", "01", 0, 0, 4);

                if (resultadoTransaccionTabla.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTabla.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionTabla.data.estado.Equals("X")) 
                {

                    AseguradoraxProductoRepository aseguradoraxProductoRepository = new AseguradoraxProductoRepository(context, _configuration);
                    //Lista los productos que no se encuentran cubierto por la aseguradora
                    ResultadoTransaccion<BE_AseguradoraxProducto> resultadoTransaccionAseguradora = await aseguradoraxProductoRepository.GetListAseguradoraxProductoPorFiltros(codaseguradora, codproducto, tipoatencion);
                    
                    if (resultadoTransaccionAseguradora.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionAseguradora.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<BE_AseguradoraxProducto>)resultadoTransaccionAseguradora.dataList).Count > 0)
                    {
                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.data = false;
                    } else
                    {
                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.data = true;
                    }

                } else
                {
                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.data = true;
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> ValidacionRegistraVentaCabecera(BE_VentasCabecera value)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                SeriePorMaquinaRepository seriePorMaquinaRepository = new SeriePorMaquinaRepository(context, _configuration);
                ResultadoTransaccion<BE_SeriePorMaquina> resultadoTransaccionSeriePorMaquina = await seriePorMaquinaRepository.GetListSeriePorMaquinaPorFiltro(new BE_SeriePorMaquina { nombremaquina = value.nombremaquina });

                if (resultadoTransaccionSeriePorMaquina.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionSeriePorMaquina.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionSeriePorMaquina.dataList.Any())
                {
                    BE_SeriePorMaquina seriePorMaquina = ((List<BE_SeriePorMaquina>)resultadoTransaccionSeriePorMaquina.dataList)[0];

                    if (!seriePorMaquina.codalmacen.Equals(value.codalmacen) && !seriePorMaquina.codalmacen.Equals(string.Empty))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Ud. solo puede vender del almacén: {0} - {1} ...Favor de revisar", seriePorMaquina.codalmacen, seriePorMaquina.desalmacen);

                        return vResultadoTransaccion;
                    }

                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Ud. no tiene acceso al almacén seleccionado...Favor de revisar";

                    return vResultadoTransaccion;
                }

                if (value.codtipocliente.Equals("01"))
                {
                    PacienteRepository atencionRepository = new PacienteRepository(context, _configuration);

                    ResultadoTransaccion<BE_Paciente> resultadoTransaccionPaciente = await atencionRepository.GetExistenciaPaciente(value.codatencion);

                    if (resultadoTransaccionPaciente.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPaciente.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    List<BE_Paciente> listPaciente = (List<BE_Paciente>)resultadoTransaccionPaciente.dataList;

                    if (listPaciente.Count == 0)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Atención no existe...Favor de revisar";

                        return vResultadoTransaccion;
                    }
                    else
                    {
                        if (!listPaciente[0].activo.Equals(1))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Atención desactivada...Favor de revisar";

                            return vResultadoTransaccion;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(value.codpedido))
                {
                    if (value.codpedido.Length.Equals(14))
                    {
                        PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration, _clientFactory);
                        ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetDatosPedidoPorPedido(value.codpedido);

                        if (resultadoTransaccionPedido.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion;

                            return vResultadoTransaccion;
                        }

                        List<BE_Pedido> listPedido = (List<BE_Pedido>)resultadoTransaccionPedido.dataList;

                        if (listPedido.Count > 0)
                        {
                            if (listPedido[0].TieneVenta)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = "Pedido ya fue atendido:" + value.codpedido + "en el almacén: " + listPedido[0].codalmacen;

                                return vResultadoTransaccion;
                            }

                            if (!string.IsNullOrEmpty(listPedido[0].codalmacen))
                            {
                                if (listPedido[0].codalmacen != value.codalmacen)
                                {
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = "Pedido ya fue atendido:" + value.codpedido + "en el almacén: " + listPedido[0].codalmacen;

                                    return vResultadoTransaccion;
                                }

                            }
                            else
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = "El centro de costo del pedido no tiene asignado un almacén";

                                return vResultadoTransaccion;
                            }
                        }
                    }
                }  

                // Controla monto de venta a personal
                if (value.codtipocliente.Equals("03"))
                {
                    PersonalClinicaRepository personalClinicaRepository = new PersonalClinicaRepository(context, _configuration);
                    ResultadoTransaccion<BE_PersonalLimiteConsumo> resultadoTransaccionPersonalClinica = await personalClinicaRepository.GetListLimiteConsumoPorPersonal(value.codcliente);
                    //ResultadoTransaccion<BE_PersonalLimiteConsumo> resultadoTransaccionPersonalClinica = new ResultadoTransaccion<BE_PersonalLimiteConsumo>();

                    if (resultadoTransaccionPersonalClinica.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPersonalClinica.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    List<BE_PersonalLimiteConsumo> personalClinicas = ((List<BE_PersonalLimiteConsumo>)resultadoTransaccionPersonalClinica.dataList);

                    if (personalClinicas.Count > 0)
                    {
                        if (personalClinicas[0].vender.Equals("N"))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = string.Format("El consumo al CREDITO es mayor al límite de consumo <br> en el periodo del {0} al {1} <br> Monto consumo (NO incluye esta venta): {2} <br> Monto Limite de Consumo {3}...Favor de revisar", personalClinicas[0].fecha1, personalClinicas[0].fecha2, personalClinicas[0].montoconsumo, personalClinicas[0].montolimite);

                            return vResultadoTransaccion;
                        }
                    }
                    else
                    {
                        var nuevoConsumo = personalClinicas[0].montoconsumo + value.montoneto;

                        if (nuevoConsumo <= personalClinicas[0].montolimite)
                        {
                            vResultadoTransaccion.ResultadoDescripcion = string.Format("El consumo al CREDITO en el periodo del {0} al {1} <br> Monto consumo (INCLUYE esta venta): {2} <br> Monto Limite de Consumo {3}...Favor de revisar", personalClinicas[0].fecha1, personalClinicas[0].fecha2, personalClinicas[0].montoconsumo, personalClinicas[0].montolimite);
                        }
                    }
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Validaciones Correctamente";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_VentasGenerado>> RegistrarVentaCabecera(BE_VentasCabecera value, bool ventaAutomatica)
        {
            ResultadoTransaccion<BE_VentasGenerado> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasGenerado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                TablaRepository tablaRepository = new TablaRepository(context, _configuration);
                ResultadoTransaccion<BE_Tabla> resultadoTransaccionTablaUsuarioAcceso = await tablaRepository.GetListTablaLogisticaPorFiltros("PERMISOUSERMOVDV", value.RegIdUsuario.ToString(), 50, 0, 2);

                if (resultadoTransaccionTablaUsuarioAcceso.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTablaUsuarioAcceso.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionTablaUsuarioAcceso.dataList.Any())
                {

                    BE_Tabla tablaUsuarioAcceso = ((List<BE_Tabla>)resultadoTransaccionTablaUsuarioAcceso.dataList)[0];

                    if (!tablaUsuarioAcceso.estado.Equals("G"))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

                        return vResultadoTransaccion;
                    }

                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

                    return vResultadoTransaccion;
                }

                // Repetido
                if (value.codtipocliente.Equals("01"))
                {
                    PacienteRepository atencionRepository = new PacienteRepository(context, _configuration);

                    ResultadoTransaccion<BE_Paciente> resultadoTransaccionPaciente = await atencionRepository.GetExistenciaPaciente(value.codatencion);

                    if (resultadoTransaccionPaciente.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPaciente.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    List<BE_Paciente> listPaciente = (List<BE_Paciente>)resultadoTransaccionPaciente.dataList;

                    if (listPaciente.Count == 0)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Atención no existe...Favor de revisar";

                        return vResultadoTransaccion;
                    }
                    else
                    {
                        if (!listPaciente[0].activo.Equals(1))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Atención desactivada...Favor de revisar";

                            return vResultadoTransaccion;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(value.codpedido))
                {
                    if (value.codpedido.Length.Equals(14))
                    {
                        PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration, _clientFactory);
                        ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetDatosPedidoPorPedido(value.codpedido);

                        if (resultadoTransaccionPedido.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion;

                            return vResultadoTransaccion;
                        }

                        List<BE_Pedido> listPedido = (List<BE_Pedido>)resultadoTransaccionPedido.dataList;

                        if (listPedido.Count > 0)
                        {
                            if (listPedido[0].TieneVenta)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = "Pedido ya fue atendido:" + value.codpedido + "en el almacén: " + listPedido[0].codalmacen;

                                return vResultadoTransaccion;
                            }
                        }
                    }
                }

                /*Validar si se tiene stock en el Almacen 0000002 + 0000019 => Para el tema de consignacion PENDIENTE DE COLOCAR VALIDACIÓN*/

                /*Iniciamos a grabar la venta*/

                List<BE_VentasDetalleQuiebre> listVentasDetalleQuiebres = new List<BE_VentasDetalleQuiebre>();

                // Lista para asignar el detalle por quiebre
                List<BE_VentasDetalle> listQuiebreVentasDetalle = new List<BE_VentasDetalle>();

                foreach (var item in value.listaVentaDetalle)
                {
                    // Inicializamos la variable
                    listQuiebreVentasDetalle = new List<BE_VentasDetalle>();

                    // Buscamos si existe
                    int vExiste = listVentasDetalleQuiebres.FindAll(xFila => xFila.codtipoproducto == item.codtipoproducto && xFila.igvproducto == item.igvproducto && xFila.narcotico == item.narcotico).Count;

                    if (vExiste.Equals(0))
                    {
                        // Clonamos el registro
                        var cabecera = value.Clone();
                        // Asignamos solo linea de detalle que le pertenece
                        cabecera.listaVentaDetalle = cabecera.listaVentaDetalle.FindAll(xFila => xFila.codtipoproducto == item.codtipoproducto && xFila.igvproducto == item.igvproducto && xFila.narcotico == item.narcotico);
                        // Agregamos linea por cada venta a realizar
                        listVentasDetalleQuiebres.Add(new BE_VentasDetalleQuiebre { codtipoproducto = item.codtipoproducto, narcotico = item.narcotico, igvproducto = item.igvproducto, ventascabecera = cabecera }) ;
                    }
                }

                string vCodCentroCSF = string.Empty;

                if (ventaAutomatica)
                {
                    vCodCentroCSF = value.codcentro;
                    value.codcentro = null;
                }

                BE_VentasCabecera newVentasCabecera = new BE_VentasCabecera();
                List<BE_VentaXml> listNewVentasCabeceraXml = new List<BE_VentaXml>();

                // Insertaresmos el detalle por venta
                List<BE_VentasDetalle> listVentasDetalle = new List<BE_VentasDetalle>();
                List<BE_VentasDetalle> listNewVentasDetalle = new List<BE_VentasDetalle>();

                // Insertaremos los lotes por detalle
                List<BE_VentasDetalleLote> listNewVentasDetalleLote = new List<BE_VentasDetalleLote>();

                string vCodigoPrestacion = string.Empty;
                decimal vGncPorVenta = 0;
                BE_ValidaPresotor vValidaPresotor = new BE_ValidaPresotor();
                string vNC = string.Empty;
                bool vNoCubierto = false;
                string vFlagPaquete = "N";
                int idborrador = 0;
                string codpedido = string.Empty;
                int ide_receta = 0;

                if (!listVentasDetalleQuiebres.Count.Equals(0))
                {
                    foreach (BE_VentasDetalleQuiebre itemQuiebreOrigen in listVentasDetalleQuiebres)
                    {
                        //Obtenemos la cabecera con los montos totales actualizados
                        newVentasCabecera = new BE_VentasCabecera();
                        newVentasCabecera = CalculaTotales(itemQuiebreOrigen, itemQuiebreOrigen.ventascabecera);

                        // Asignamos el porcentaje de Igv
                        newVentasCabecera.porcentajeimpuesto = itemQuiebreOrigen.igvproducto;

                        // Obtenemos el detalle por venta
                        listVentasDetalle = new List<BE_VentasDetalle>();
                        listNewVentasDetalle = new List<BE_VentasDetalle>();

                        listVentasDetalle = itemQuiebreOrigen.ventascabecera.listaVentaDetalle;

                        newVentasCabecera.listaVentaDetalle = new List<BE_VentasDetalle>();

                        // Realizamos la logica para la asignacion del lote
                        foreach (BE_VentasDetalle item in listVentasDetalle)
                        {
                            if (vFlagPaquete.Equals("S"))
                            {
                                item.gnc = "N";
                            }

                            if (vNC.Equals("S"))
                            {
                                vNoCubierto = true;
                            }

                            // Inicializamos la variables por cada linea de detalle
                            listNewVentasDetalleLote = new List<BE_VentasDetalleLote>();
                            item.listVentasDetalleLotes = new List<BE_VentasDetalleLote>();
                            // Validamos si el articulo trabaja por Lote
                            if (item.manBtchNum)
                            {
                                // Validamos si los lotes son seleccionados manuealmente
                                if (item.flgbtchnum)
                                {
                                    if (item.listStockLote.Count > 0)
                                    {
                                        foreach (var lote in item.listStockLote)
                                        {
                                            if (lote.Quantityinput > 0)
                                            {
                                                var itemLote = new BE_VentasDetalleLote { codproducto = lote.ItemCode, lote = lote.BatchNum, cantidad = lote.Quantityinput, coddetalle = item.coddetalle, fechavencimiento = (DateTime)lote.ExpDate, ubicacion = lote.BinAbs == null ? 0 :  (int)lote.BinAbs };

                                                listNewVentasDetalleLote.Add(itemLote);
                                            }
                                        }

                                        item.listVentasDetalleLotes = listNewVentasDetalleLote;
                                    }
                                }
                                else
                                {
                                    if (!newVentasCabecera.flgsinstock)
                                    {
                                        // Si no es seleccionado manualmente, obtenemos los lotes de hana
                                        StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
                                        ResultadoTransaccion<BE_StockLote> resultadoTransaccionListLote = await stockRepository.GetListStockLotePorFiltro(item.codalmacen, item.codproducto, true);

                                        if (resultadoTransaccionListLote.ResultadoCodigo == -1)
                                        {
                                            vResultadoTransaccion.IdRegistro = -1;
                                            vResultadoTransaccion.ResultadoCodigo = -1;
                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListLote.ResultadoDescripcion + " ; [GetListStockLotePorFiltro]";
                                            return vResultadoTransaccion;
                                        }

                                        decimal cantidadInput = item.cantidad;
                                        List<BE_StockLote> listLote = new List<BE_StockLote>();

                                        listLote = (List<BE_StockLote>)resultadoTransaccionListLote.dataList;

                                        if (listLote.Any())
                                        {
                                            foreach (BE_StockLote lote in listLote)
                                            {
                                                if (cantidadInput > 0)
                                                {
                                                    if (cantidadInput <= lote.QuantityLote)
                                                    {
                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = cantidadInput;
                                                        cantidadInput = 0;
                                                    }
                                                    else
                                                    {
                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = 0;

                                                        decimal cantidad = (decimal)listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).QuantityLote;

                                                        decimal newResul = (cantidadInput - cantidad);

                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = cantidadInput - newResul;

                                                        cantidadInput = newResul;
                                                    }
                                                }
                                            }

                                            if (cantidadInput > 0)
                                            {
                                                vResultadoTransaccion.IdRegistro = -1;
                                                vResultadoTransaccion.ResultadoCodigo = -1;
                                                vResultadoTransaccion.ResultadoDescripcion = "No existe suficiente stock para el producto: " + item.codproducto;
                                                return vResultadoTransaccion;
                                            }

                                            if (listLote.Count > 0)
                                            {
                                                foreach (BE_StockLote lote in listLote)
                                                {
                                                    if (lote.Quantityinput > 0)
                                                    {

                                                        var binAbs = (lote.BinAbs == null) ? 0 : (int)lote.BinAbs; 

                                                        if (binAbs > 0 && ventaAutomatica)
                                                        {
                                                            vResultadoTransaccion.IdRegistro = -1;
                                                            vResultadoTransaccion.ResultadoCodigo = -1;
                                                            vResultadoTransaccion.ResultadoDescripcion = "Venta automática no esta configurado para vender con Ubicación" + item.codproducto;
                                                            return vResultadoTransaccion;
                                                        }

                                                        var itemLote = new BE_VentasDetalleLote { codproducto = lote.ItemCode, lote = lote.BatchNum, cantidad = lote.Quantityinput, coddetalle = item.coddetalle, fechavencimiento = (DateTime)lote.ExpDate, ubicacion = binAbs };
                                                        listNewVentasDetalleLote.Add(itemLote);
                                                    }
                                                }
                                                item.listVentasDetalleLotes = listNewVentasDetalleLote;
                                            }
                                        }
                                        else
                                        {
                                            vResultadoTransaccion.IdRegistro = -1;
                                            vResultadoTransaccion.ResultadoCodigo = -1;
                                            vResultadoTransaccion.ResultadoDescripcion = "No se encuentran lotes con stock para el producto " + item.codproducto;
                                            return vResultadoTransaccion;
                                        }
                                    }
                                }
                            }
                            // Asignamos el detalle a la venta
                            listNewVentasDetalle.Add(item);
                        }

                        newVentasCabecera.listaVentaDetalle = listNewVentasDetalle;

                        //Asignamos el valor del id borrador si fuera de sala de operación
                        idborrador = newVentasCabecera.idborrador;
                        codpedido = newVentasCabecera.codpedido;
                        ide_receta = newVentasCabecera.ide_receta;
                        // Realizamos la conversion a XML
                        var entiDom = new BE_VentaXml();
                        var ser = new Serializador();
                        var ms = new MemoryStream();
                        ser.SerializarXml(newVentasCabecera, ms);
                        entiDom.XmlData = Encoding.UTF8.GetString(ms.ToArray());
                        ms.Dispose();

                        var venta = new BE_VentaXml
                        {
                            XmlData = entiDom.XmlData,
                            RegIdUsuario = newVentasCabecera.RegIdUsuario
                        };

                        listNewVentasCabeceraXml.Add(venta);
                    }
                }

                // Conexion de Logistica
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    // Conexion de Logistica
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Insertaremos las ventas generadas
                        List<BE_VentasGenerado> listVentasGenerados = new List<BE_VentasGenerado>();

                        foreach (BE_VentaXml ventaXml in listNewVentasCabeceraXml)
                        {
                            ResultadoTransaccion<BE_VentasGenerado> resultadoTransaccionVentasGenerado = await RegistraVentaXml(conn, transaction, ventaXml, (int)value.RegIdUsuario);

                            if (resultadoTransaccionVentasGenerado.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentasGenerado.ResultadoDescripcion + " ; [RegistraVentaXml]";
                                return vResultadoTransaccion;
                            }

                            listVentasGenerados.Add(new BE_VentasGenerado { codventa = resultadoTransaccionVentasGenerado.data.codventa, codpresotor = resultadoTransaccionVentasGenerado.data.codpresotor });

                            value.codventa = resultadoTransaccionVentasGenerado.data.codventa;
                            value.codpresotor = resultadoTransaccionVentasGenerado.data.codpresotor;

                            #region "Envio a SAP"
                            if (!value.flgsinstock)
                            {
                                #region Envia la Entrega

                                ResultadoTransaccion<bool> resultadoTransaccionVenta = await EnviarVentaSAP(conn, transaction, value.codventa, (int)value.RegIdUsuario);

                                if (resultadoTransaccionVenta.IdRegistro == -1)
                                {
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }

                                #endregion

                            } else
                            {
                                ResultadoTransaccion<bool> resultadoTransaccionProject = await EnviarProjectSAP(conn, transaction, value.codventa, value.codpresotor, (int)value.RegIdUsuario);

                                if (resultadoTransaccionProject.IdRegistro == -1)
                                {
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProject.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }
                            }

                            #endregion

                        }

                        #region Eliminamos la reserva, si es de sala de operación
                        SapReserveStockRepository sapReserveStockRepository = new SapReserveStockRepository(_clientFactory, _configuration, context);
                        string u_idexterno = string.Format("SOP-{0}", idborrador);
                        ResultadoTransaccion<SapReserveStock> resultadoTransaccionReserva = await sapReserveStockRepository.GetListReservaPorIdExterno(u_idexterno);

                        if (resultadoTransaccionReserva.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReserva.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        foreach (SapReserveStock item in resultadoTransaccionReserva.dataList)
                        {
                            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoTransaccionReservaDelete = await sapReserveStockRepository.SetDeleteReserve(item.Code);

                            if (resultadoTransaccionReservaDelete.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReservaDelete.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }
                        }
                        #endregion

                        #region Eliminamos la reserva, del picking realizado Pedido
                        if (!string.IsNullOrEmpty(codpedido))
                        {
                            ConsolidadoRepository consolidadoRepository = new ConsolidadoRepository(_clientFactory, context, _configuration);
                            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionConsolidado = await consolidadoRepository.GetListConsolidadoIndividualPorPedidoPicking(codpedido);

                            if (resultadoTransaccionConsolidado.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionConsolidado.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionConsolidadoEstado = await consolidadoRepository.ModificarEstadoWebPedido(conn, transaction, codpedido, "2", (int)value.RegIdUsuario);

                            if (resultadoTransaccionConsolidadoEstado.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionConsolidadoEstado.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            foreach (BE_ConsolidadoPedidoPicking item in resultadoTransaccionConsolidado.dataList)
                            {
                                if (item.idreserva > 0)
                                {
                                    ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoTransaccionReservaDelete = await sapReserveStockRepository.SetDeleteReserve(item.idreserva);

                                    if (resultadoTransaccionReservaDelete.ResultadoCodigo == -1)
                                    {
                                        transaction.Rollback();
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReservaDelete.ResultadoDescripcion;
                                        return vResultadoTransaccion;
                                    }
                                }
                            } 
                        }
                        #endregion

                        #region  Eliminamos la reserva, del picking realizado receta
                        if (ide_receta > 0)
                        {
                            PickingRepository pickingRepository = new PickingRepository(_clientFactory, context, _configuration);
                            ResultadoTransaccion<BE_Picking> resultadoTransaccionConsolidado = await pickingRepository.GetListPickingPorReceta(ide_receta);

                            if (resultadoTransaccionConsolidado.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionConsolidado.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            ResultadoTransaccion<BE_Picking> resultadoTransaccionEstado = await pickingRepository.ModificarEstadoReceta(new BE_Picking { codpedido = string.Empty, id_receta = ide_receta, estado = 2, codusuarioapu = "", RegIdUsuario = (int)value.RegIdUsuario });

                            if (resultadoTransaccionEstado.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionEstado.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            foreach (BE_Picking item in resultadoTransaccionConsolidado.dataList)
                            {
                                if (item.idreserva > 0)
                                {
                                    ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoTransaccionReservaDelete = await sapReserveStockRepository.SetDeleteReserve(item.idreserva);

                                    if (resultadoTransaccionReservaDelete.ResultadoCodigo == -1)
                                    {
                                        transaction.Rollback();
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReservaDelete.ResultadoDescripcion;
                                        return vResultadoTransaccion;
                                    }
                                }
                            }
                        }
                        #endregion

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente";

                        vResultadoTransaccion.dataList = listVentasGenerados;

                        transaction.Commit();
                        transaction.Dispose();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    }


                    conn.Close();
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
        public async Task<ResultadoTransaccion<bool>> EnviarVentaSAP(SqlConnection conn, SqlTransaction transaction, string codventa, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa, conn, transaction);

                if (resultadoTransaccionVenta.IdRegistro == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<bool> resultadoTransaccionProject = await EnviarProjectSAP(conn, transaction, codventa, resultadoTransaccionVenta.data.codpresotor, RegIdUsuario);

                if (resultadoTransaccionProject.IdRegistro == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProject.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);
                ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetCreateDocument(resultadoTransaccionVenta.data);

                if (resultadoSapDocument.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoSapDocument.data.DocEntry > 0)
                {

                    resultadoTransaccionVenta.data.ide_docentrysap = resultadoSapDocument.data.DocEntry;
                    resultadoTransaccionVenta.data.fec_docentrysap = DateTime.Now;
                    resultadoTransaccionVenta.data.RegIdUsuario = RegIdUsuario;

                    ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPVenta(resultadoTransaccionVenta.data, conn, transaction);

                    if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                    {
                        //transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<bool>> EnviarProjectSAP(SqlConnection conn, SqlTransaction transaction, string codventa, string codpresotor, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                ResultadoTransaccion<SapProject> resultadoTransaccionProject = await GetListProjectPorFiltro(codventa, codpresotor, conn, transaction);

                if (resultadoTransaccionProject.IdRegistro == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProject.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                

                foreach (SapProject itemProject in resultadoTransaccionProject.dataList)
                {

                    ResultadoTransaccion<SapProject> resultadoTransaccionListProject = await sapDocuments.GetProject(itemProject);
                    if (resultadoTransaccionListProject.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListProject.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<SapProject>)resultadoTransaccionListProject.dataList).Count == 0)
                    {
                        ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapProject = await sapDocuments.SetCreateProject(itemProject);

                        if (resultadoSapProject.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoSapProject.ResultadoDescripcion;
                            return vResultadoTransaccion;
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
        public async Task<ResultadoTransaccion<bool>> EnviarVentaAsociadaReservaSAP(SqlConnection conn, SqlTransaction transaction, string codventa, int docentryreserva, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa, conn, transaction);

                if (resultadoTransaccionVenta.IdRegistro == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "ERROR AL OBTENER VENTA";
                    return vResultadoTransaccion;
                }

                // Asignamos el Valor del Doc Entry de la reserva
                resultadoTransaccionVenta.data.ide_docentrysap = docentryreserva;

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetCreateAsociateReserveDocument(resultadoTransaccionVenta.data);

                if (resultadoSapDocument.ResultadoCodigo == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoSapDocument.data.DocEntry > 0)
                {

                    resultadoTransaccionVenta.data.ide_docentrysap = resultadoSapDocument.data.DocEntry;
                    resultadoTransaccionVenta.data.fec_docentrysap = DateTime.Now;
                    resultadoTransaccionVenta.data.RegIdUsuario = RegIdUsuario;

                    ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPVenta(resultadoTransaccionVenta.data, conn, transaction);

                    if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                    {
                        //transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<bool>> DevolucionVentaSAPBase(SqlConnection conn, SqlTransaction transaction, string codventa, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa, conn, transaction);

                if (resultadoTransaccionVenta.IdRegistro == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "ERROR AL OBTENER VENTA";
                    return vResultadoTransaccion;
                }

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetReturnsDocumentBase(resultadoTransaccionVenta.data);

                if (resultadoSapDocument.ResultadoCodigo == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoSapDocument.data.DocEntry > 0)
                {

                    resultadoTransaccionVenta.data.ide_docentrysap = resultadoSapDocument.data.DocEntry;
                    resultadoTransaccionVenta.data.fec_docentrysap = DateTime.Now;
                    resultadoTransaccionVenta.data.RegIdUsuario = RegIdUsuario;

                    ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPVenta(resultadoTransaccionVenta.data, conn, transaction);

                    if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                    {
                        //transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<bool>> DevolucionVentaSAP(SqlConnection conn, SqlTransaction transaction, string codventa, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa, conn, transaction);

                if (resultadoTransaccionVenta.IdRegistro == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "ERROR AL OBTENER VENTA";
                    return vResultadoTransaccion;
                }

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetReturnsDocument(resultadoTransaccionVenta.data);

                if (resultadoSapDocument.ResultadoCodigo == -1)
                {
                    //transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoSapDocument.data.DocEntry > 0)
                {

                    resultadoTransaccionVenta.data.ide_docentrysap = resultadoSapDocument.data.DocEntry;
                    resultadoTransaccionVenta.data.fec_docentrysap = DateTime.Now;
                    resultadoTransaccionVenta.data.RegIdUsuario = RegIdUsuario;

                    ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPVenta(resultadoTransaccionVenta.data, conn, transaction);

                    if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                    {
                        //transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_VentasGenerado>> RegistraVentaXml(SqlConnection conn, SqlTransaction transaction, BE_VentaXml item, int RegIdUsuario)
        {
            ResultadoTransaccion<BE_VentasGenerado> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasGenerado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmdDatos = new SqlCommand(SP_INSERT_XML, conn, transaction))
                {
                    cmdDatos.Parameters.Clear();
                    cmdDatos.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdDatos.CommandTimeout = 0;
                    SqlParameter outputCodVentaParamDato = new SqlParameter("@codventa", SqlDbType.Char, 8)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmdDatos.Parameters.Add(outputCodVentaParamDato);
                    SqlParameter outputCodPresotorParamDato = new SqlParameter("@codpresotor", SqlDbType.Char, 12)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmdDatos.Parameters.Add(outputCodPresotorParamDato);
                    cmdDatos.Parameters.Add(new SqlParameter("@xmldata", item.XmlData));
                    // Datos de Auditoria
                    cmdDatos.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
                    // Datos de Transaccion
                    SqlParameter outputIdTransaccionParamDato = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmdDatos.Parameters.Add(outputIdTransaccionParamDato);

                    SqlParameter outputMsjTransaccionParamDato = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmdDatos.Parameters.Add(outputMsjTransaccionParamDato);

                    //await conn.OpenAsync();
                    await cmdDatos.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParamDato.Value.ToString());
                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParamDato.Value;

                    var result = new BE_VentasGenerado { 
                        codventa = (string)outputCodVentaParamDato.Value,
                        codpresotor = (string)outputCodPresotorParamDato.Value
                    };

                    vResultadoTransaccion.data = result;
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
        //public async Task<ResultadoTransaccion<BE_VentasGenerado>> RegistrarVentaCabecera_Old(BE_VentasCabecera value, bool ventaAutomatica)
        //{
        //    ResultadoTransaccion<BE_VentasGenerado> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasGenerado>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        TablaRepository tablaRepository = new TablaRepository(context, _configuration);
        //        ResultadoTransaccion<BE_Tabla> resultadoTransaccionTablaUsuarioAcceso = await tablaRepository.GetListTablaLogisticaPorFiltros("PERMISOUSERMOVDV", value.RegIdUsuario.ToString(), 50, 0, 2);

        //        if (resultadoTransaccionTablaUsuarioAcceso.ResultadoCodigo == -1)
        //        {
        //            vResultadoTransaccion.IdRegistro = -1;
        //            vResultadoTransaccion.ResultadoCodigo = -1;
        //            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTablaUsuarioAcceso.ResultadoDescripcion;

        //            return vResultadoTransaccion;
        //        }

        //        if (resultadoTransaccionTablaUsuarioAcceso.dataList.Any())
        //        {
        //            BE_Tabla tablaUsuarioAcceso = ((List<BE_Tabla>)resultadoTransaccionTablaUsuarioAcceso.dataList)[0];

        //            if (!tablaUsuarioAcceso.estado.Equals("G"))
        //            {
        //                vResultadoTransaccion.IdRegistro = -1;
        //                vResultadoTransaccion.ResultadoCodigo = -1;
        //                vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

        //                return vResultadoTransaccion;
        //            }

        //        }
        //        else
        //        {
        //            vResultadoTransaccion.IdRegistro = -1;
        //            vResultadoTransaccion.ResultadoCodigo = -1;
        //            vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

        //            return vResultadoTransaccion;
        //        }

        //        // Repetido
        //        if (value.codtipocliente.Equals("01"))
        //        {
        //            PacienteRepository atencionRepository = new PacienteRepository(context, _configuration);

        //            ResultadoTransaccion<BE_Paciente> resultadoTransaccionPaciente = await atencionRepository.GetExistenciaPaciente(value.codatencion);

        //            if (resultadoTransaccionPaciente.ResultadoCodigo == -1)
        //            {
        //                vResultadoTransaccion.IdRegistro = -1;
        //                vResultadoTransaccion.ResultadoCodigo = -1;
        //                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPaciente.ResultadoDescripcion;

        //                return vResultadoTransaccion;
        //            }

        //            List<BE_Paciente> listPaciente = (List<BE_Paciente>)resultadoTransaccionPaciente.dataList;

        //            if (listPaciente.Count == 0)
        //            {
        //                vResultadoTransaccion.IdRegistro = -1;
        //                vResultadoTransaccion.ResultadoCodigo = -1;
        //                vResultadoTransaccion.ResultadoDescripcion = "Atención no existe...Favor de revisar";

        //                return vResultadoTransaccion;
        //            }
        //            else
        //            {
        //                if (!listPaciente[0].activo.Equals(1))
        //                {
        //                    vResultadoTransaccion.IdRegistro = -1;
        //                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                    vResultadoTransaccion.ResultadoDescripcion = "Atención desactivada...Favor de revisar";

        //                    return vResultadoTransaccion;
        //                }
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(value.codpedido))
        //        {
        //            if (value.codpedido.Length.Equals(14))
        //            {
        //                PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration);
        //                ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetDatosPedidoPorPedido(value.codpedido);

        //                if (resultadoTransaccionPedido.ResultadoCodigo == -1)
        //                {
        //                    vResultadoTransaccion.IdRegistro = -1;
        //                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion;

        //                    return vResultadoTransaccion;
        //                }

        //                List<BE_Pedido> listPedido = (List<BE_Pedido>)resultadoTransaccionPedido.dataList;

        //                if (listPedido.Count > 0)
        //                {
        //                    if (listPedido[0].TieneVenta)
        //                    {
        //                        vResultadoTransaccion.IdRegistro = -1;
        //                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                        vResultadoTransaccion.ResultadoDescripcion = "Pedido ya fue atendido:" + value.codpedido + "en el almacén: " + listPedido[0].codalmacen;

        //                        return vResultadoTransaccion;
        //                    }
        //                }
        //            }
        //        }

        //        /*Validar si se tiene stock en el Almacen 0000002 + 0000019 => Para el tema de consignacion PENDIENTE DE COLOCAR VALIDACIÓN*/

        //        /*Iniciamos a grabar la venta*/

        //        List<BE_VentasDetalleQuiebre> listVentasDetalleQuiebres = new List<BE_VentasDetalleQuiebre>();

        //        foreach (var item in value.listaVentaDetalle)
        //        {
        //            int vExiste = listVentasDetalleQuiebres.FindAll(xFila => xFila.codtipoproducto == item.codtipoproducto && xFila.igvproducto == item.igvproducto && xFila.narcotico == item.narcotico).Count;

        //            if (vExiste.Equals(0))
        //            {
        //                listVentasDetalleQuiebres.Add(new BE_VentasDetalleQuiebre { codtipoproducto = item.codtipoproducto, narcotico = item.narcotico, igvproducto = item.igvproducto });
        //            }
        //        }

        //        string vCodCentroCSF = string.Empty;

        //        if (ventaAutomatica)
        //        {
        //            vCodCentroCSF = value.codcentro;
        //            value.codcentro = null;
        //        }

        //        // Conexion de Logistica
        //        using (SqlConnection conn = new SqlConnection(_cnx))
        //        {
        //            using (CommittableTransaction transaction = new CommittableTransaction())
        //            {
        //                // Conexion de Logistica
        //                await conn.OpenAsync();
        //                conn.EnlistTransaction(transaction);

        //                try
        //                {
        //                    List<BE_VentasGenerado> listVentasGenerados = new List<BE_VentasGenerado>();
        //                    string vCodigoPresotor = string.Empty;

        //                    if (!listVentasDetalleQuiebres.Count.Equals(0))
        //                    {

        //                        string vCodigoPrestacion = string.Empty;
        //                        decimal vGncPorVenta = 0;
        //                        BE_ValidaPresotor vValidaPresotor = new BE_ValidaPresotor();
        //                        //string vGnc = string.Empty;
        //                        string vNC = string.Empty;
        //                        bool vNoCubierto = false;
        //                        string vFlagPaquete = "N";

        //                        foreach (BE_VentasDetalleQuiebre itemQuiebre in listVentasDetalleQuiebres)
        //                        {

        //                            //ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionCalculoCabeceraVenta = CalculaTotales(itemQuiebre, value);

        //                            //if (resultadoTransaccionCalculoCabeceraVenta.ResultadoCodigo == -1)
        //                            //{
        //                            //    transaction.Rollback();
        //                            //    vResultadoTransaccion.IdRegistro = -1;
        //                            //    vResultadoTransaccion.ResultadoCodigo = -1;
        //                            //    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCalculoCabeceraVenta.ResultadoDescripcion + " ; [CalculaTotales]";
        //                            //    return vResultadoTransaccion;
        //                            //}

        //                            value = CalculaTotales(itemQuiebre, value);

        //                            value.porcentajeimpuesto = itemQuiebre.igvproducto;

        //                            ResultadoTransaccion<string> resultadoTransaccionRegistraCabeceraVenta = await RegistraVentaCabecera(conn, value);

        //                            if (resultadoTransaccionRegistraCabeceraVenta.ResultadoCodigo == -1)
        //                            {
        //                                transaction.Rollback();
        //                                vResultadoTransaccion.IdRegistro = -1;
        //                                vResultadoTransaccion.ResultadoCodigo = -1;
        //                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionRegistraCabeceraVenta.ResultadoDescripcion + " ; [RegistraVentaCabecera]";
        //                                return vResultadoTransaccion;
        //                            }

        //                            value.codventa = resultadoTransaccionRegistraCabeceraVenta.data;

        //                            if (value.codpedido != null)
        //                            {
        //                                if (value.codpedido.Length.Equals(14))
        //                                {
        //                                    // Obtiene los datos del pedido
        //                                    PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration);
        //                                    ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetDatosPedidoPorPedido(conn, value.codpedido);

        //                                    if (resultadoTransaccionPedido.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion + " ; [GetDatosPedidoPorPedido]";
        //                                        return vResultadoTransaccion;
        //                                    }

        //                                    if (resultadoTransaccionPedido.dataList.Any())
        //                                    {
        //                                        vFlagPaquete = ((List<BE_Pedido>)resultadoTransaccionPedido.dataList)[0].flg_paquete;
        //                                    }

        //                                }
        //                            }

        //                            List<BE_VentasDetalle> listVentasDetalle = value.listaVentaDetalle.FindAll(xFila => xFila.codtipoproducto == itemQuiebre.codtipoproducto && xFila.igvproducto == itemQuiebre.igvproducto && xFila.narcotico == itemQuiebre.narcotico).ToList();

        //                            foreach (BE_VentasDetalle item in listVentasDetalle)
        //                            {
        //                                if (vFlagPaquete.Equals("S"))
        //                                {
        //                                    item.gnc = "N";
        //                                }

        //                                if (vNC.Equals("S"))
        //                                {
        //                                    vNoCubierto = true;
        //                                }

        //                                item.codventa = value.codventa;

        //                                ResultadoTransaccion<string> resultadoTransaccionDetalle = await RegistraVentaDetalle(conn, item, (int)value.RegIdUsuario);

        //                                if (resultadoTransaccionDetalle.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetalle.ResultadoDescripcion + " ; [RegistraVentaDetalle]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                item.coddetalle = resultadoTransaccionDetalle.data;

        //                                if (item.manBtchNum)
        //                                {
        //                                    BE_VentasDetalleLote itemLote = new BE_VentasDetalleLote();

        //                                    if (item.flgbtchnum)
        //                                    {
        //                                        if (item.listStockLote.Count > 0)
        //                                        {
        //                                            foreach (var lote in item.listStockLote)
        //                                            {
        //                                                if (lote.Quantityinput > 0)
        //                                                {
        //                                                    itemLote = new BE_VentasDetalleLote { lote = lote.BatchNum, cantidad = lote.Quantityinput, coddetalle = item.coddetalle, fechavencimiento = (DateTime)lote.ExpDate };

        //                                                    ResultadoTransaccion<bool> resultadoTransaccionRegistraVentaLote = await RegistraVentaDetalleLote(conn, itemLote, (int)value.RegIdUsuario);

        //                                                    if (resultadoTransaccionRegistraVentaLote.ResultadoCodigo == -1)
        //                                                    {
        //                                                        transaction.Rollback();
        //                                                        vResultadoTransaccion.IdRegistro = -1;
        //                                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionRegistraVentaLote.ResultadoDescripcion + " ; [RegistraVentaDetalleLote]";
        //                                                        return vResultadoTransaccion;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
        //                                        ResultadoTransaccion<BE_StockLote> resultadoTransaccionListLote = await stockRepository.GetListStockLotePorFiltro(item.codalmacen, item.codproducto, true);

        //                                        if (resultadoTransaccionListLote.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListLote.ResultadoDescripcion + " ; [GetListStockLotePorFiltro]";
        //                                            return vResultadoTransaccion;
        //                                        }

        //                                        decimal cantidadInput = item.cantidad;
        //                                        List<BE_StockLote> listLote = new List<BE_StockLote>();

        //                                        listLote = (List<BE_StockLote>)resultadoTransaccionListLote.dataList;

        //                                        if (listLote.Any())
        //                                        {
        //                                            foreach (BE_StockLote lote in listLote)
        //                                            {
        //                                                if (cantidadInput > 0)
        //                                                {
        //                                                    if (cantidadInput <= lote.QuantityLote)
        //                                                    {
        //                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = cantidadInput;
        //                                                        cantidadInput = 0;
        //                                                    }
        //                                                    else
        //                                                    {
        //                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = 0;

        //                                                        decimal cantidad = (decimal)listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).QuantityLote;

        //                                                        decimal newResul = (cantidadInput - cantidad);

        //                                                        listLote.Find(xFila => xFila.BatchNum == lote.BatchNum).Quantityinput = cantidadInput - newResul;

        //                                                        cantidadInput = newResul;
        //                                                    }
        //                                                }
        //                                            }

        //                                            if (cantidadInput > 0)
        //                                            {
        //                                                transaction.Rollback();
        //                                                vResultadoTransaccion.IdRegistro = -1;
        //                                                vResultadoTransaccion.ResultadoCodigo = -1;
        //                                                vResultadoTransaccion.ResultadoDescripcion = "No existe suficiente stock para el producto: " + item.codproducto;
        //                                                return vResultadoTransaccion;
        //                                            }

        //                                            if (listLote.Count > 0)
        //                                            {
        //                                                foreach (BE_StockLote lote in listLote)
        //                                                {
        //                                                    if (lote.Quantityinput > 0)
        //                                                    {
        //                                                        itemLote = new BE_VentasDetalleLote { lote = lote.BatchNum, cantidad = lote.Quantityinput, coddetalle = item.coddetalle, fechavencimiento = (DateTime)lote.ExpDate };

        //                                                        ResultadoTransaccion<bool> resultadoTransaccionRegistraVentaLote = await RegistraVentaDetalleLote(conn, itemLote, (int)value.RegIdUsuario);

        //                                                        if (resultadoTransaccionRegistraVentaLote.ResultadoCodigo == -1)
        //                                                        {
        //                                                            transaction.Rollback();
        //                                                            vResultadoTransaccion.IdRegistro = -1;
        //                                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionRegistraVentaLote.ResultadoDescripcion + " ; [RegistraVentaDetalleLote]";
        //                                                            return vResultadoTransaccion;
        //                                                        }
        //                                                    }
        //                                                }
        //                                            }

        //                                        }
        //                                        else
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = "No se encuentran lotes con stock para el producto " + item.codproducto;
        //                                            return vResultadoTransaccion;
        //                                        }

        //                                    }
        //                                }

        //                                if (item.VentasDetalleDatos != null)
        //                                {
        //                                    if (!item.VentasDetalleDatos.tipodocumentoautorizacion.Equals("00"))
        //                                    {
        //                                        ResultadoTransaccion<bool> resultadoTransaccionRegistraVentaDatos = await RegistraVentaDatos(conn, item, (int)value.RegIdUsuario);

        //                                        if (resultadoTransaccionRegistraVentaDatos.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionRegistraVentaDatos.ResultadoDescripcion + " ; [RegistraVentaDatos]";
        //                                            return vResultadoTransaccion;
        //                                        }
        //                                    }
        //                                }

        //                                // Validar
        //                                if (value.codtipocliente.Equals("03"))
        //                                {
        //                                    if (value.codpedido != null)
        //                                    {
        //                                        if (value.codpedido.Length.Equals(14))
        //                                        {

        //                                            ResultadoTransaccion<bool> vResultadoTransaccionPedido = await PedidoxDevolverRecalculo(conn, item.codpedido, item.codproducto, item.cantidad, 0, 0);

        //                                            if (vResultadoTransaccionPedido.ResultadoCodigo == -1)
        //                                            {
        //                                                transaction.Rollback();
        //                                                vResultadoTransaccion.IdRegistro = -1;
        //                                                vResultadoTransaccion.ResultadoCodigo = -1;
        //                                                vResultadoTransaccion.ResultadoDescripcion = vResultadoTransaccionPedido.ResultadoDescripcion + " ; [PedidoxDevolverRecalculo]";
        //                                                return vResultadoTransaccion;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                            }

        //                            /*Genera transferencia automatica*/

        //                            if (value.codtipocliente.Equals("01"))
        //                            {
        //                                if (value.codpedido != null)
        //                                {
        //                                    if (value.codpedido.Length.Equals(14))
        //                                    {
        //                                        ResultadoTransaccion<string> resultadoTransaccionPrestacion = await GetTipoProductoPrestaciones(conn, value.codventa, itemQuiebre.codtipoproducto, vCodCentroCSF);

        //                                        if (resultadoTransaccionPrestacion.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPrestacion.ResultadoDescripcion + " ; [GetTipoProductoPrestaciones]";
        //                                            return vResultadoTransaccion;
        //                                        }

        //                                        vCodigoPrestacion = resultadoTransaccionPrestacion.data;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    ResultadoTransaccion<string> resultadoTransaccionPrestacion = await GetTipoProductoPrestaciones(conn, value.codventa, itemQuiebre.codtipoproducto, string.Empty);

        //                                    if (resultadoTransaccionPrestacion.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPrestacion.ResultadoDescripcion + " ; [GetTipoProductoPrestaciones]";
        //                                        return vResultadoTransaccion;
        //                                    }

        //                                    vCodigoPrestacion = resultadoTransaccionPrestacion.data;
        //                                }

        //                                // Validar codigo de prestación que viene del pedido -- Falta Indicar por Cesar Medrano
        //                                if (ventaAutomatica)
        //                                {
        //                                    if (value.flagpaquete.Equals("S"))
        //                                    {
        //                                        vCodigoPrestacion = vCodigoPrestacion;
        //                                    }
        //                                }

        //                                if (!ventaAutomatica)
        //                                {
        //                                    ResultadoTransaccion<decimal> resultadoTransaccionGncxVenta = await GetGncPorVenta(conn, value.codventa);

        //                                    if (resultadoTransaccionGncxVenta.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionGncxVenta.ResultadoDescripcion + " ; [GetGncPorVenta]";
        //                                        return vResultadoTransaccion;
        //                                    }

        //                                    vGncPorVenta = resultadoTransaccionGncxVenta.data;

        //                                    ResultadoTransaccion<BE_ValidaPresotor> resultadoTransaccionValidaPresotor = await GetValidaPresotor(conn, value.codatencion, value.codventa);

        //                                    if (resultadoTransaccionValidaPresotor.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValidaPresotor.ResultadoDescripcion + " ; [GetValidaPresotor]";
        //                                        return vResultadoTransaccion;
        //                                    }

        //                                    vValidaPresotor = resultadoTransaccionValidaPresotor.data;

        //                                    if (vValidaPresotor.seguir_venta.Equals("N"))
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
        //                                        return vResultadoTransaccion;
        //                                    }
        //                                }

        //                                ResultadoTransaccion<string> resultadoTransaccionCodigoPresotor = await ResgistraPrestacion(conn, value.codatencion, vCodigoPrestacion, (float)value.montototal, (float)vGncPorVenta, (float)value.montopaciente, 0, (float)value.porcentajecoaseguro, 0, value.codventa, 0, value.observacion, value.tipomovimiento);

        //                                if (resultadoTransaccionCodigoPresotor.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCodigoPresotor.ResultadoDescripcion + " ; [ResgistraPrestacion]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                vCodigoPresotor = resultadoTransaccionCodigoPresotor.data;

        //                                if (vValidaPresotor.codatencion_ver != vCodigoPresotor.Substring(0, 8))
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                ResultadoTransaccion<bool> resultadoTransaccionActualizarVenta = await ActualizarVenta(conn, value.codventa, vCodigoPresotor, (int)value.RegIdUsuario);

        //                                if (resultadoTransaccionActualizarVenta.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionActualizarVenta.ResultadoDescripcion + " ; [ActualizarVenta]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                ResultadoTransaccion<string> resultadoTransaccionPresotorConsultaVarios = await GetPresotorConsultaVarios(conn, value.codatencion, vCodigoPresotor, 3);

        //                                if (resultadoTransaccionPresotorConsultaVarios.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPresotorConsultaVarios.ResultadoDescripcion + " ; [GetPresotorConsultaVarios]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                if (resultadoTransaccionPresotorConsultaVarios.data.Equals("N"))
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                ResultadoTransaccion<bool> resultadoTransaccionActualizarPresotor = new ResultadoTransaccion<bool>();

        //                                if (vFlagPaquete.Equals("S"))
        //                                {
        //                                    resultadoTransaccionActualizarPresotor = await ActualizarPresotor(conn, "flagpaquete", vCodigoPresotor, "S");
        //                                }
        //                                else
        //                                {
        //                                    resultadoTransaccionActualizarPresotor = await ActualizarPresotor(conn, "flagpaquete", vCodigoPresotor, "N");
        //                                }

        //                                if (resultadoTransaccionActualizarPresotor.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionActualizarPresotor.ResultadoDescripcion + " ; [ActualizarPresotor]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                if (!value.codatencion.Substring(1, 1).Equals("A"))
        //                                {
        //                                    if (vNoCubierto)
        //                                    {
        //                                        ResultadoTransaccion<bool> resultadoTransaccionValorGNC = await ActualizarPresotor(conn, "valorgnc", vCodigoPresotor, vGncPorVenta.ToString());

        //                                        if (resultadoTransaccionValorGNC.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValorGNC.ResultadoDescripcion + " ; [ActualizarPresotor]";
        //                                            return vResultadoTransaccion;
        //                                        }

        //                                        ResultadoTransaccion<bool> resultadoTransaccionLiqTerceroContratante = await ActualizarPresotor(conn, "liqtercerocontratante", vCodigoPresotor, "S");

        //                                        if (resultadoTransaccionLiqTerceroContratante.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionLiqTerceroContratante.ResultadoDescripcion + " ; [ActualizarPresotor]";
        //                                            return vResultadoTransaccion;
        //                                        }
        //                                    }
        //                                }

        //                                ResultadoTransaccion<bool> resultadoTransaccionAfectoImpuesto = new ResultadoTransaccion<bool>();

        //                                if (itemQuiebre.igvproducto.Equals(0))
        //                                {
        //                                    resultadoTransaccionAfectoImpuesto = await ActualizarPresotor(conn, "afectoimpuesto", vCodigoPresotor, "N");
        //                                }
        //                                else
        //                                {
        //                                    resultadoTransaccionAfectoImpuesto = await ActualizarPresotor(conn, "afectoimpuesto", vCodigoPresotor, "S");
        //                                }

        //                                if (resultadoTransaccionAfectoImpuesto.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionAfectoImpuesto.ResultadoDescripcion + " ; [ActualizarPresotor]";
        //                                    return vResultadoTransaccion;
        //                                }

        //                                ResultadoTransaccion<bool> resultadoTransaccionCodMedicoEnvia = await ActualizarPresotor(conn, "codmedicoenvia", vCodigoPresotor, value.codmedico);

        //                                if (resultadoTransaccionCodMedicoEnvia.ResultadoCodigo == -1)
        //                                {
        //                                    transaction.Rollback();
        //                                    vResultadoTransaccion.IdRegistro = -1;
        //                                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCodMedicoEnvia.ResultadoDescripcion + " ; [ActualizarPresotor]";
        //                                    return vResultadoTransaccion;
        //                                }
        //                            }

        //                            if (value.codtipocliente.Equals("03"))
        //                            {
        //                                if (value.codpedido != null)
        //                                {
        //                                    if (value.codpedido.Length.Equals(14))
        //                                    {

        //                                        ResultadoTransaccion<bool> vResultadoTransaccionPedido = await PedidoxDevolverRecalculo(conn, value.codpedido, string.Empty, 0, 0, 1);

        //                                        if (vResultadoTransaccionPedido.ResultadoCodigo == -1)
        //                                        {
        //                                            transaction.Rollback();
        //                                            vResultadoTransaccion.IdRegistro = -1;
        //                                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                                            vResultadoTransaccion.ResultadoDescripcion = vResultadoTransaccionPedido.ResultadoDescripcion + " ; [PedidoxDevolverRecalculo]";
        //                                            return vResultadoTransaccion;
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            listVentasGenerados.Add(new BE_VentasGenerado { codventa = value.codventa, codpresotor = vCodigoPresotor });

        //                        }
        //                    }

        //                    foreach (BE_VentasGenerado itemVenta in listVentasGenerados)
        //                    {
        //                        ResultadoTransaccion<bool> resultadoTransaccionActualizarVentaDcstDetalle = await ActualizarVentaDcstDetalle(conn, value.codventa, vCodigoPresotor, (int)value.RegIdUsuario);

        //                        if (resultadoTransaccionActualizarVentaDcstDetalle.ResultadoCodigo == -1)
        //                        {
        //                            transaction.Rollback();
        //                            vResultadoTransaccion.IdRegistro = -1;
        //                            vResultadoTransaccion.ResultadoCodigo = -1;
        //                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionActualizarVentaDcstDetalle.ResultadoDescripcion + " ; [ActualizarVentaDcstDetalle]";
        //                            return vResultadoTransaccion;
        //                        }
        //                    }

        //                    if (!ventaAutomatica)
        //                    {
        //                        if (value.codtipocliente.Equals("01"))
        //                        {
        //                            if (value.codpedido != null)
        //                            {
        //                                if (value.codpedido.Length.Equals(14))
        //                                {
        //                                    ResultadoTransaccion<bool> resultadoTransaccionVistoBueno = await ActualizarPedido(conn, "vistobueno", value.codpedido, "L", value.RegIdUsuario.ToString());

        //                                    if (resultadoTransaccionVistoBueno.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVistoBueno.ResultadoDescripcion + " ; [ActualizarPedido]";
        //                                        return vResultadoTransaccion;
        //                                    }

        //                                    ResultadoTransaccion<bool> resultadoTransaccionFechaAtencion = await ActualizarPedido(conn, "fechaatencion_servidor", value.codpedido, "L", DateTime.Now.ToString());

        //                                    if (resultadoTransaccionFechaAtencion.ResultadoCodigo == -1)
        //                                    {
        //                                        transaction.Rollback();
        //                                        vResultadoTransaccion.IdRegistro = -1;
        //                                        vResultadoTransaccion.ResultadoCodigo = -1;
        //                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionFechaAtencion.ResultadoDescripcion + " ; [ActualizarPedido]";
        //                                        return vResultadoTransaccion;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    vResultadoTransaccion.IdRegistro = 0;
        //                    vResultadoTransaccion.ResultadoCodigo = 0;
        //                    vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente";

        //                    vResultadoTransaccion.dataList = listVentasGenerados;

        //                    transaction.Commit();
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    vResultadoTransaccion.IdRegistro = -1;
        //                    vResultadoTransaccion.ResultadoCodigo = -1;
        //                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<string>> RegistraVentaCabecera(SqlConnection conn, BE_VentasCabecera value)
        //{
        //    ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnx))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
        //        {
        //            cmd.Parameters.Clear();
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //            SqlParameter oParam = new SqlParameter("@codigo", SqlDbType.Char, 8)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(oParam);

        //            cmd.Parameters.Add(new SqlParameter("@codatencion", value.codatencion));
        //            cmd.Parameters.Add(new SqlParameter("@codpedido", value.codpedido));
        //            cmd.Parameters.Add(new SqlParameter("@codalmacen", value.codalmacen));
        //            cmd.Parameters.Add(new SqlParameter("@tipomovimiento", value.tipomovimiento));
        //            cmd.Parameters.Add(new SqlParameter("@codtipocliente", value.codtipocliente));
        //            cmd.Parameters.Add(new SqlParameter("@codempresa", value.codempresa));
        //            cmd.Parameters.Add(new SqlParameter("@codcliente", value.codcliente));
        //            cmd.Parameters.Add(new SqlParameter("@codpaciente", value.codpaciente));
        //            cmd.Parameters.Add(new SqlParameter("@nombre", value.nombre));
        //            cmd.Parameters.Add(new SqlParameter("@cama", value.cama));
        //            cmd.Parameters.Add(new SqlParameter("@codmedico", value.codmedico));
        //            cmd.Parameters.Add(new SqlParameter("@planpoliza", value.planpoliza));
        //            cmd.Parameters.Add(new SqlParameter("@codpoliza", value.codpoliza));
        //            cmd.Parameters.Add(new SqlParameter("@deducible", value.deducible));
        //            cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
        //            cmd.Parameters.Add(new SqlParameter("@codcia", value.codcia));
        //            cmd.Parameters.Add(new SqlParameter("@porcentajecoaseguro", value.porcentajecoaseguro));
        //            cmd.Parameters.Add(new SqlParameter("@porcentajeimpuesto", value.porcentajeimpuesto));
        //            cmd.Parameters.Add(new SqlParameter("@montodctoplan", value.montodctoplan));
        //            cmd.Parameters.Add(new SqlParameter("@porcentajedctoplan", value.porcentajedctoplan));
        //            cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
        //            cmd.Parameters.Add(new SqlParameter("@montototal", value.montototal));
        //            cmd.Parameters.Add(new SqlParameter("@montoigv", value.montoigv));
        //            cmd.Parameters.Add(new SqlParameter("@montoneto", value.montoneto));
        //            cmd.Parameters.Add(new SqlParameter("@codplan", value.codplan));
        //            cmd.Parameters.Add(new SqlParameter("@montopaciente", value.montopaciente));
        //            cmd.Parameters.Add(new SqlParameter("@montoaseguradora", value.montoaseguradora));
        //            cmd.Parameters.Add(new SqlParameter("@observacion", value.observacion));
        //            cmd.Parameters.Add(new SqlParameter("@nombremedico", value.nombremedico));
        //            cmd.Parameters.Add(new SqlParameter("@nombreaseguradora", value.nombreaseguradora));
        //            cmd.Parameters.Add(new SqlParameter("@nombrecia", value.nombrecia));
        //            cmd.Parameters.Add(new SqlParameter("@tipocambio", value.tipocambio));
        //            cmd.Parameters.Add(new SqlParameter("@nombrediagnostico", value.nombrediagnostico));
        //            cmd.Parameters.Add(new SqlParameter("@flagpaquete", value.flagpaquete));
        //            cmd.Parameters.Add(new SqlParameter("@flg_gratuito", value.flg_gratuito));
        //            cmd.Parameters.Add(new SqlParameter("@usuario", value.usuario));
        //            cmd.Parameters.Add(new SqlParameter("@codcentro", value.codcentro));
        //            cmd.Parameters.Add(new SqlParameter("@flgsinstock", value.flgsinstock));

        //            // Datos de Auditoria
        //            cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(outputMsjTransaccionParam);

        //            await cmd.ExecuteNonQueryAsync();

        //            value.codventa = (string)oParam.Value;

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //            vResultadoTransaccion.data = value.codventa;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<string>> RegistraVentaDetalle(SqlConnection conn, BE_VentasDetalle item, int RegIdUsuario)
        //{
        //    ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnx))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdDetalle = new SqlCommand(SP_INSERT_DETALLE, conn))
        //        {
        //            cmdDetalle.Parameters.Clear();
        //            cmdDetalle.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdDetalle.Parameters.Add(new SqlParameter("@codventa", item.codventa));

        //            SqlParameter oParamDetalle = new SqlParameter("@coddetalle", SqlDbType.Char, 10)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDetalle.Parameters.Add(oParamDetalle);

        //            cmdDetalle.Parameters.Add(new SqlParameter("@codalmacen", item.codalmacen));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@tipomovimiento", item.tipomovimiento));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@codtipoproducto", item.codtipoproducto));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@codproducto", item.codproducto));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@dsc_producto", item.nombreproducto));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@cnt_unitario", item.cantidad));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@prc_unitario", item.valorVVP));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@preciounidadcondcto", item.preciounidadcondcto));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@precioventaPVP", item.precioventaPVP));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@valorVVP", item.valorVVP));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@porcentajedctoproducto", item.porcentajedctoproducto));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@montototal", item.montototal));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@montopaciente", item.montopaciente));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@montoaseguradora", item.montoaseguradora));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@gnc", item.gnc));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@manbtchnum", item.manBtchNum));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@flgbtchnum", item.flgbtchnum));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@flgnarcotico", item.flgnarcotico));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@stockfraccion", item.stockfraccion));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@stockalmacen", item.stockalmacen));

        //            // Datos de Auditoria
        //            cmdDetalle.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParamDetalle = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDetalle.Parameters.Add(outputIdTransaccionParamDetalle);

        //            SqlParameter outputMsjTransaccionParamDetalle = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDetalle.Parameters.Add(outputMsjTransaccionParamDetalle);

        //            //await conn.OpenAsync();
        //            await cmdDetalle.ExecuteNonQueryAsync();

        //            item.coddetalle = (string)oParamDetalle.Value;

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParamDetalle.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParamDetalle.Value;
        //            vResultadoTransaccion.data = item.coddetalle;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> RegistraVentaDetalleLote(SqlConnection conn, BE_VentasDetalleLote item, int RegIdUsuario)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        using (SqlCommand cmdDetalle = new SqlCommand(SP_INSERT_DETALLE_LOTE, conn))
        //        {
        //            cmdDetalle.Parameters.Clear();
        //            cmdDetalle.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdDetalle.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

        //            cmdDetalle.Parameters.Add(new SqlParameter("@lote", item.lote));
        //            //cmdDetalle.Parameters.Add(new SqlParameter("@ubicacion", item.ubicacion));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@fechavencimiento", item.fechavencimiento));
        //            cmdDetalle.Parameters.Add(new SqlParameter("@cantidad", item.cantidad));

        //            // Datos de Auditoria
        //            cmdDetalle.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParamDetalle = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDetalle.Parameters.Add(outputIdTransaccionParamDetalle);

        //            SqlParameter outputMsjTransaccionParamDetalle = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDetalle.Parameters.Add(outputMsjTransaccionParamDetalle);

        //            await cmdDetalle.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParamDetalle.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParamDetalle.Value;
        //            vResultadoTransaccion.data = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> RegistraVentaDatos(SqlConnection conn, BE_VentasDetalle item, int RegIdUsuario)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnx))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdDatos = new SqlCommand(SP_INSERT_DETALLE_DATOS, conn))
        //        {
        //            cmdDatos.Parameters.Clear();
        //            cmdDatos.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdDatos.Parameters.Add(new SqlParameter("@codventa", item.codventa));
        //            cmdDatos.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

        //            cmdDatos.Parameters.Add(new SqlParameter("@codproducto", item.codproducto));
        //            cmdDatos.Parameters.Add(new SqlParameter("@tipodocumentoautorizacion", item.VentasDetalleDatos.tipodocumentoautorizacion));
        //            cmdDatos.Parameters.Add(new SqlParameter("@numerodocumentoautorizacion", item.VentasDetalleDatos.numerodocumentoautorizacion));

        //            // Datos de Auditoria
        //            cmdDatos.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParamDato = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDatos.Parameters.Add(outputIdTransaccionParamDato);

        //            SqlParameter outputMsjTransaccionParamDato = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdDatos.Parameters.Add(outputMsjTransaccionParamDato);

        //            //await conn.OpenAsync();
        //            await cmdDatos.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParamDato.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParamDato.Value;
        //            vResultadoTransaccion.data = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> ActualizarVenta(SqlConnection conn, string codventa, string codigopresotor, int RegIdUsuario)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnx))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdUpdate = new SqlCommand(SP_UPDATE, conn))
        //        {
        //            cmdUpdate.Parameters.Clear();
        //            cmdUpdate.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdUpdate.Parameters.Add(new SqlParameter("@codventa", codventa));

        //            cmdUpdate.Parameters.Add(new SqlParameter("@codpresotor", codigopresotor));

        //            // Datos de Auditoria
        //            cmdUpdate.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdUpdate.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdUpdate.Parameters.Add(outputMsjTransaccionParam);

        //            //await conn.OpenAsync();
        //            await cmdUpdate.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //            vResultadoTransaccion.data = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> ActualizarVentaDcstDetalle(SqlConnection conn, string codventa, string codigopresotor, int RegIdUsuario)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnx))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdUpdate = new SqlCommand(SP_UPDATE_DSCTO_DETALLE, conn))
        //        {
        //            cmdUpdate.Parameters.Clear();
        //            cmdUpdate.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdUpdate.Parameters.Add(new SqlParameter("@codventa", codventa));
        //            cmdUpdate.Parameters.Add(new SqlParameter("@codpresotor", codigopresotor));

        //            // Datos de Auditoria
        //            //cmdUpdate.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdUpdate.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdUpdate.Parameters.Add(outputMsjTransaccionParam);

        //            //await conn.OpenAsync();
        //            await cmdUpdate.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //            vResultadoTransaccion.data = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> PedidoxDevolverRecalculo(SqlConnection conn, string codpedido, string codproducto, decimal entero, decimal menudeo,int orden)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection conn = new SqlConnection(_cnxClinica))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdPedido = new SqlCommand(SP_PEDIDOXDEVOLVER_RECALCULO, conn))
        //        {
        //            cmdPedido.Parameters.Clear();
        //            cmdPedido.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdPedido.Parameters.Add(new SqlParameter("@codpedido", codpedido));
        //            cmdPedido.Parameters.Add(new SqlParameter("@codproducto", codproducto));

        //            cmdPedido.Parameters.Add(new SqlParameter("@cantE", entero));
        //            cmdPedido.Parameters.Add(new SqlParameter("@cantM", menudeo));
        //            cmdPedido.Parameters.Add(new SqlParameter("@orden", orden));

        //            // Datos de Transaccion
        //            // Datos de Transaccion
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdPedido.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdPedido.Parameters.Add(outputMsjTransaccionParam);

        //            await cmdPedido.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //            vResultadoTransaccion.data = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<string>> ResgistraPrestacion(SqlConnection conn, string codatencion, string codpresentacion, float montototal, float gnc, float montopaciente, float porcentajededucible, float coaseguro, float descuento, string codventa, float deducible, string observacion, string tipomovimiento)
        //{
        //    ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection connPedido = new SqlConnection(_cnxClinica))
        //    //{
        //    try
        //    {

        //        string vCodPresotor = string.Empty;

        //        using (SqlCommand cmdPedido = new SqlCommand(SP_FARMACIA_PRESTACION_INSERT, conn))
        //        {
        //            cmdPedido.Parameters.Clear();
        //            cmdPedido.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdPedido.Parameters.Add(new SqlParameter("@codatencion", codatencion));
        //            cmdPedido.Parameters.Add(new SqlParameter("@codprestacion", codpresentacion));
        //            cmdPedido.Parameters.Add(new SqlParameter("@valorcorregido", montototal));
        //            cmdPedido.Parameters.Add(new SqlParameter("@valorgnc", gnc));
        //            cmdPedido.Parameters.Add(new SqlParameter("@valorcoaseguro", montopaciente));
        //            cmdPedido.Parameters.Add(new SqlParameter("@porcentajededucible", porcentajededucible));
        //            cmdPedido.Parameters.Add(new SqlParameter("@porcentajecoaseguro", coaseguro));
        //            cmdPedido.Parameters.Add(new SqlParameter("@descuento", descuento));
        //            cmdPedido.Parameters.Add(new SqlParameter("@documento", codventa));
        //            cmdPedido.Parameters.Add(new SqlParameter("@deducible", deducible));
        //            cmdPedido.Parameters.Add(new SqlParameter("@observaciones", observacion));
        //            cmdPedido.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento));

        //            SqlParameter outputCodPresotorParam = new SqlParameter("@codpresotor", SqlDbType.Char, 12)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdPedido.Parameters.Add(outputCodPresotorParam);

        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdPedido.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdPedido.Parameters.Add(outputMsjTransaccionParam);

        //            //await conn.OpenAsync();
        //            await cmdPedido.ExecuteNonQueryAsync();

        //            vCodPresotor = (string)outputCodPresotorParam.Value;

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //            vResultadoTransaccion.data = vCodPresotor;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<string>> GetTipoProductoPrestaciones(SqlConnection conn, string codventa, string codtipproducto, string codcentropedido)
        //{
        //    ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        //using (SqlConnection connTipoProducto = new SqlConnection(_cnx))
        //        //{
        //        using (SqlCommand cmdTipoProductoPrestaciones = new SqlCommand(SP_GET_TIPOPRODUCTOPRESTACIONES, conn))
        //        {
        //            cmdTipoProductoPrestaciones.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@codventa", codventa));
        //            cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@tipoproducto", codtipproducto));
        //            cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@centrocsf", codcentropedido));

        //            string response = string.Empty;

        //            //await connTipoProducto.OpenAsync();

        //            using (var reader = await cmdTipoProductoPrestaciones.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    response = ((reader["codprestacion"]) is DBNull) ? string.Empty : (string)reader["codprestacion"];
        //                }
        //            }

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = 0;
        //            vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
        //            vResultadoTransaccion.data = response;
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        //public async Task<ResultadoTransaccion<decimal>> GetGncPorVenta(SqlConnection conn, string codventa)
        //{
        //    ResultadoTransaccion<decimal> vResultadoTransaccion = new ResultadoTransaccion<decimal>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        //using (SqlConnection connGncPorVenta = new SqlConnection(_cnx))
        //        //{
        //        using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_GET_GNCXVENTA, conn))
        //        {
        //            cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmdGncPorVenta.Parameters.Add(new SqlParameter("@codventa", codventa));

        //            decimal response = 0;

        //            //await connGncPorVenta.OpenAsync();

        //            using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    response = ((reader["valorgnc"]) is DBNull) ? 0 : (decimal)reader["valorgnc"];
        //                }
        //            }


        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = 0;
        //            vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
        //            vResultadoTransaccion.data = response;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        //public async Task<ResultadoTransaccion<BE_ValidaPresotor>> GetValidaPresotor(SqlConnection conn, string codatencion, string codventa)
        //{
        //    ResultadoTransaccion<BE_ValidaPresotor> vResultadoTransaccion = new ResultadoTransaccion<BE_ValidaPresotor>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        //using (SqlConnection connValidaPresotor = new SqlConnection(_cnx))
        //        //{
        //        using (SqlCommand cmdValidaPresotor = new SqlCommand(SP_GET_VALIDA_PRESOTOR, conn))
        //        {
        //            cmdValidaPresotor.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmdValidaPresotor.Parameters.Add(new SqlParameter("@codatencion", codatencion));
        //            cmdValidaPresotor.Parameters.Add(new SqlParameter("@codventa", codventa));

        //            BE_ValidaPresotor response = new BE_ValidaPresotor();

        //            //await connValidaPresotor.OpenAsync();

        //            using (var reader = await cmdValidaPresotor.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    response.seguir_venta = ((reader["seguir_venta"]) is DBNull) ? string.Empty : (string)reader["seguir_venta"];
        //                    response.codatencion_ver = ((reader["codatencion_ver"]) is DBNull) ? string.Empty : (string)reader["codatencion_ver"];
        //                    response.codpresotor_ver = ((reader["codpresotor_ver"]) is DBNull) ? string.Empty : (string)reader["codpresotor_ver"];
        //                }
        //            }

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = 0;
        //            vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
        //            vResultadoTransaccion.data = response;
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        //public async Task<ResultadoTransaccion<string>> GetPresotorConsultaVarios(SqlConnection conn, string codatencion, string codpresotor, int orden)
        //{
        //    ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {
        //        //using (SqlConnection connPresotorConsultaVarios = new SqlConnection(_cnx))
        //        //{
        //        using (SqlCommand cmdPresotorConsultaVarios = new SqlCommand(SP_GET_PRESOTOR_CONSULTAVARIOS, conn))
        //        {
        //            cmdPresotorConsultaVarios.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@codigo", codatencion));
        //            cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@codpresotor", codpresotor));
        //            cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@orden", orden));

        //            string response = string.Empty;

        //            //await conn.OpenAsync();

        //            using (var reader = await cmdPresotorConsultaVarios.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    response = ((reader["seguir_venta"]) is DBNull) ? string.Empty : (string)reader["seguir_venta"];
        //                }
        //            }


        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = 0;
        //            vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
        //            vResultadoTransaccion.data = response;
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        //public async Task<ResultadoTransaccion<bool>> ActualizarPresotor(SqlConnection conn, string campo, string codpresotor, string nuevovalor)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection connPresotor = new SqlConnection(_cnxClinica))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdActualizarPresotor = new SqlCommand(SP_UPDATE_PRESOTOR, conn))
        //        {
        //            cmdActualizarPresotor.Parameters.Clear();
        //            cmdActualizarPresotor.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdActualizarPresotor.Parameters.Add(new SqlParameter("@campo", campo));
        //            cmdActualizarPresotor.Parameters.Add(new SqlParameter("@codigo", codpresotor));
        //            cmdActualizarPresotor.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdActualizarPresotor.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdActualizarPresotor.Parameters.Add(outputMsjTransaccionParam);
        //            //await connPresotor.OpenAsync();
        //            await cmdActualizarPresotor.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}

        //public async Task<ResultadoTransaccion<bool>> ActualizarPedido(SqlConnection conn, string campo, string codigo, string nuevovalor, string coduser)
        //{
        //    ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    //using (SqlConnection connPedido = new SqlConnection(conn))
        //    //{
        //    try
        //    {
        //        using (SqlCommand cmdActualizarPedido = new SqlCommand(SP_UPDATE_PEDIDO, conn))
        //        {
        //            cmdActualizarPedido.Parameters.Clear();
        //            cmdActualizarPedido.CommandType = System.Data.CommandType.StoredProcedure;

        //            cmdActualizarPedido.Parameters.Add(new SqlParameter("@wcampo", campo));
        //            cmdActualizarPedido.Parameters.Add(new SqlParameter("@codigo", codigo));
        //            cmdActualizarPedido.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));
        //            cmdActualizarPedido.Parameters.Add(new SqlParameter("@coduser", coduser));
        //            SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdActualizarPedido.Parameters.Add(outputIdTransaccionParam);

        //            SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmdActualizarPedido.Parameters.Add(outputMsjTransaccionParam);
        //            //await connPedido.OpenAsync();
        //            await cmdActualizarPedido.ExecuteNonQueryAsync();

        //            vResultadoTransaccion.IdRegistro = 0;
        //            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
        //            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }
        //    //}

        //    return vResultadoTransaccion;
        //}
        private BE_VentasCabecera CalculaTotales(BE_VentasDetalleQuiebre itemQuiebre, BE_VentasCabecera value)
        {
            BE_VentasCabecera ventasCabecera = new BE_VentasCabecera();

            bool isTrabajaVariosIGV = true;
            decimal isIGVTemp = 0;
            decimal isIGV = 0;
            decimal isSubTotal = 0;
            decimal isTotalPaciente = 0;
            decimal isTotalAseguradora = 0;

            decimal isSubTotal_0 = 0;
            decimal isTotalPaciente_0 = 0;
            decimal isTotalAseguradora_0 = 0;

            ventasCabecera = value;

            List<BE_VentasDetalle> ventasDetalles = new List<BE_VentasDetalle>();

            if (ventasCabecera.listaVentaDetalle.Any())
            {
                ventasDetalles = ventasCabecera.listaVentaDetalle;//.FindAll(xFila => xFila.codtipoproducto == itemQuiebre.codtipoproducto && xFila.igvproducto == itemQuiebre.igvproducto && xFila.narcotico == itemQuiebre.narcotico).ToList();

                foreach (BE_VentasDetalle item in ventasDetalles)
                {
                    isIGV = item.igvproducto;

                    if (isIGV > isIGVTemp)
                    {
                        isIGVTemp = isIGV;
                    }

                    if (!isTrabajaVariosIGV)
                    {
                        isSubTotal = isSubTotal + item.totalsinigv;
                        isTotalPaciente = isTotalPaciente + item.montopaciente;
                        isTotalAseguradora = isTotalAseguradora + item.montoaseguradora;
                    }
                    else
                    {
                        if (isIGV == 0)
                        {
                            isSubTotal_0 = isSubTotal_0 + item.totalsinigv;
                            isTotalPaciente_0 = isTotalPaciente_0 + item.montopaciente;
                            isTotalAseguradora_0 = isTotalAseguradora_0 + item.montoaseguradora;
                        }
                        else
                        {
                            isSubTotal = isSubTotal + item.totalsinigv;
                            isTotalPaciente = isTotalPaciente + item.montopaciente;
                            isTotalAseguradora = isTotalAseguradora + item.montoaseguradora;
                        }
                    }
                }

                isIGV = isIGVTemp;

                if (!isTrabajaVariosIGV)
                {
                    ventasCabecera.montototal = Math.Round(isSubTotal, 2);
                    ventasCabecera.montodctoplan = Math.Round(isSubTotal * (ventasCabecera.porcentajedctoplan / 100), 2);
                    ventasCabecera.montoigv = Math.Round(isSubTotal * (isIGV / 100), 2);
                    ventasCabecera.montoneto = Math.Round(isSubTotal + (isSubTotal * (isIGV / 100)), 2);

                    ventasCabecera.montopaciente = Math.Round(isTotalPaciente, 2);
                    ventasCabecera.montoaseguradora = Math.Round(isTotalAseguradora, 2);
                }
                else
                {
                    ventasCabecera.montodctoplan = Math.Round((isSubTotal + isSubTotal_0) * (ventasCabecera.porcentajedctoplan / 100), 2);

                    // SubTotal
                    ventasCabecera.montototal = Math.Round(isSubTotal + isSubTotal_0, 2);
                    // Igv
                    ventasCabecera.montoigv = Math.Round(isSubTotal * (isIGV / 100), 2);
                    // Totales
                    ventasCabecera.montoneto = Math.Round(isSubTotal + isSubTotal_0 + (isSubTotal * (isIGV / 100)), 2);

                    //ventasCabecera.montopaciente = Math.Round(isTotalPaciente + isTotalPaciente_0 + (isTotalPaciente * (isIGV / 100)), 2);
                    //ventasCabecera.montoaseguradora = Math.Round(isTotalAseguradora + isTotalAseguradora_0 + (isTotalAseguradora * (isIGV / 100)), 2);

                    ventasCabecera.montopaciente = Math.Round(isTotalPaciente + isTotalPaciente_0, 2);
                    ventasCabecera.montoaseguradora = Math.Round(isTotalAseguradora + isTotalAseguradora_0 , 2);
                }
            }
            else
            {
                ventasCabecera.montototal = Math.Round(isSubTotal + isSubTotal_0, 2);
                ventasCabecera.montodctoplan = Math.Round((isSubTotal + isSubTotal_0) * (ventasCabecera.porcentajedctoplan / 100), 2);
                ventasCabecera.montoigv = Math.Round(isSubTotal * (isIGV / 100), 2);
                ventasCabecera.montoneto = Math.Round((isSubTotal + isSubTotal_0) + (isSubTotal * (isIGV / 100)), 2);
                ventasCabecera.montopaciente = Math.Round(isTotalPaciente + isTotalPaciente_0 + (isTotalPaciente * (isIGV / 100)), 2);
                ventasCabecera.montoaseguradora = Math.Round(isTotalAseguradora + isTotalAseguradora_0 + (isTotalAseguradora * (isIGV / 100)), 2);
            }

            return ventasCabecera;

        }
        public async Task<ResultadoTransaccion<BE_VentasDetalle>> GetVentasChequea1MesAntes(string codpaciente, int cuantosmesesantes)
        {
            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_VentasDetalle>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VENTAS_CHEQUEA_1MES_ANTES, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@cuantosmesesantes", cuantosmesesantes));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasDetalle>)context.ConvertTo<BE_VentasDetalle>(reader);
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> ValidacionAnularVenta(BE_VentasCabecera value)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                if (value.codtipocliente.Equals("01"))
                {
                    PacienteRepository atencionRepository = new PacienteRepository(context, _configuration);

                    ResultadoTransaccion<BE_Paciente> resultadoTransaccionPaciente = await atencionRepository.GetExistenciaPaciente(value.codatencion);

                    if (resultadoTransaccionPaciente.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPaciente.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    List<BE_Paciente> listPaciente = (List<BE_Paciente>)resultadoTransaccionPaciente.dataList;

                    if (listPaciente.Count == 0)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Atención no existe...Favor de revisar";

                        return vResultadoTransaccion;
                    }
                    else
                    {
                        if (!listPaciente[0].activo.Equals(1))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Atención desactivada...Favor de revisar";

                            return vResultadoTransaccion;
                        }
                    }

                    ResultadoTransaccion<BE_Presotor> resultadoTransaccionPresotor = await GetConsultaPresotorPorCodigo(value.codpresotor);

                    if (resultadoTransaccionPresotor.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPresotor.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    if (resultadoTransaccionPresotor.data.existepresotor)
                    {
                        if (!string.IsNullOrEmpty(resultadoTransaccionPresotor.data.codliqpaciente.Trim()) ||
                            !string.IsNullOrEmpty(resultadoTransaccionPresotor.data.codliqaseguradora.Trim()) ||
                            !string.IsNullOrEmpty(resultadoTransaccionPresotor.data.codliqcontratante.Trim()))
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Venta tiene liquidación asociada";

                            return vResultadoTransaccion;
                        }
                    }
                    else
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Presetor no se encuentra";

                        return vResultadoTransaccion;
                    }
                }

                if (value.tienedevolucion)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "No puede anular una venta que tiene una devolución asociada o esta anulada";

                    return vResultadoTransaccion;
                }

                if (!value.tipomovimiento.Equals("DV"))
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "No puede anular este movimiento";

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<int> resultadoTransaccionValidaExisteVentaAnulacion = await GetValidaExisteVentaAnulacion(value.codventa);

                if (resultadoTransaccionValidaExisteVentaAnulacion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValidaExisteVentaAnulacion.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionValidaExisteVentaAnulacion.data > 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "La venta tiene generada una anulación";

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVentaCabecera = await GetListVentaCabecera(value.codventa);

                if (resultadoTransaccionVentaCabecera.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaCabecera.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionVentaCabecera.dataList.Any())
                {
                    BE_VentasCabecera ventasCabecera = ((List<BE_VentasCabecera>)resultadoTransaccionVentaCabecera.dataList)[0];

                    if(ventasCabecera.estado.Trim().Equals("C") && !string.IsNullOrEmpty(ventasCabecera.codcomprobante))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "No puede Anular la venta, tiene comprobante";

                        return vResultadoTransaccion;
                    }
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Validaciones Correctamente";
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Presotor>> GetConsultaPresotorPorCodigo(string codpresotor)
        {
            ResultadoTransaccion<BE_Presotor> vResultadoTransaccion = new ResultadoTransaccion<BE_Presotor>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_PRESOTOR_CONSULTA, conn))
                    {
                        cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdGncPorVenta.Parameters.Add(new SqlParameter("@codpresotor", codpresotor));

                        BE_Presotor response = new BE_Presotor();

                        await conn.OpenAsync();

                        using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.existepresotor = true;
                                response.codliqpaciente = ((reader["codliqpaciente"]) is DBNull) ? string.Empty : (string)reader["codliqpaciente"];
                                response.codliqaseguradora = ((reader["codliqaseguradora"]) is DBNull) ? string.Empty : (string)reader["codliqaseguradora"];
                                response.codliqcontratante = ((reader["codliqcontratante"]) is DBNull) ? string.Empty : (string)reader["codliqcontratante"];
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
        public async Task<ResultadoTransaccion<int>> GetValidaExisteVentaAnulacion(string codventa)
        {
            ResultadoTransaccion<int> vResultadoTransaccion = new ResultadoTransaccion<int>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_GET_VALIDA_EXISTE_VENTA_ANULACION, conn))
                    {
                        cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdGncPorVenta.Parameters.Add(new SqlParameter("@codventa", codventa));

                        int response = 0;

                        await conn.OpenAsync();

                        using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = ((reader["cantidad"]) is DBNull) ? 0 : (int)reader["cantidad"];
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> RegistrarAnularVenta(BE_VentasCabecera value)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                TablaRepository tablaRepository = new TablaRepository(context, _configuration);
                ResultadoTransaccion<BE_Tabla> resultadoTransaccionTablaUsuarioAcceso = await tablaRepository.GetListTablaLogisticaPorFiltros("PERMISOUSERMOVCA", value.RegIdUsuario.ToString(), 50, 0, 2);

                if (resultadoTransaccionTablaUsuarioAcceso.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTablaUsuarioAcceso.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionTablaUsuarioAcceso.dataList.Any())
                {
                    BE_Tabla tablaUsuarioAcceso = ((List<BE_Tabla>)resultadoTransaccionTablaUsuarioAcceso.dataList)[0];

                    if (!tablaUsuarioAcceso.estado.Equals("G"))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

                        return vResultadoTransaccion;
                    }

                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionValidacionAnularVenta = await ValidacionAnularVenta(value);

                if (resultadoTransaccionValidacionAnularVenta.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValidacionAnularVenta.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(value.codventa);

                if (resultadoTransaccionVenta.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Generar Aanulacion
                        ResultadoTransaccion<string> resultadoTransaccionAnularVenta = await AnularVenta(conn, transaction, value);

                        if (resultadoTransaccionAnularVenta.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionAnularVenta.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        // Enviar a SAP Anulacion

                        if (!resultadoTransaccionVenta.data.flgsinstock)
                        {
                            SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                            ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetCancelDocument(resultadoTransaccionVenta.data.ide_docentrysap);

                            if (resultadoSapDocument.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            // Obtenemos las doc entry asociados a esta venta
                            ResultadoTransaccion<SapSelectDocument> resultadoTransaccionSelectDocument = await sapDocuments.GetListSapDocument(string.Format("DV{0}", value.codventa));

                            if (resultadoTransaccionSelectDocument.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionSelectDocument.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            if (resultadoTransaccionSelectDocument.dataList.Any())
                            {
                                List<SapSelectDocument> listDocEntry = (List<SapSelectDocument>)resultadoTransaccionSelectDocument.dataList;

                                long ide_docentrysapAnulacion = 0;

                                foreach (SapSelectDocument item in listDocEntry)
                                {
                                    if (!item.DocEntry.Equals(resultadoTransaccionVenta.data.ide_docentrysap))
                                    {
                                        ide_docentrysapAnulacion = item.DocEntry;
                                    }
                                }

                                value.codventa = resultadoTransaccionAnularVenta.data;
                                value.ide_docentrysap = (int)ide_docentrysapAnulacion;
                                value.fec_docentrysap = DateTime.Now;

                                ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPVenta(value, conn, transaction);

                                if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                                {
                                    //transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }
                            }
                        }

                        transaction.Commit();
                        transaction.Dispose();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Validaciones Correctamente";

                    conn.Close();
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
        public async Task<ResultadoTransaccion<string>> AnularVenta(SqlConnection conn, SqlTransaction transaction, BE_VentasCabecera value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmdActualizarPresotor = new SqlCommand(SP_INSERT_VENTA_ANULAR, conn, transaction))
                {
                    cmdActualizarPresotor.Parameters.Clear();
                    cmdActualizarPresotor.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@usuario", value.usuario));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@motivoanulacion", value.motivoanulacion));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));

                    SqlParameter outputCodVentaAnulacionParam = new SqlParameter("@codventaanulacion", SqlDbType.Char, 8)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmdActualizarPresotor.Parameters.Add(outputCodVentaAnulacionParam);

                    SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmdActualizarPresotor.Parameters.Add(outputIdTransaccionParam);

                    SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmdActualizarPresotor.Parameters.Add(outputMsjTransaccionParam);
                    await cmdActualizarPresotor.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    vResultadoTransaccion.data = (string)outputCodVentaAnulacionParam.Value; ;
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
        public async Task<ResultadoTransaccion<bool>> GeneraVentaAutomatica(string codpedido)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration, _clientFactory);
                ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetListaPedidoVentaAutomatica(codpedido);

                if (resultadoTransaccionPedido.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionPedido.dataList.Any())
                {

                    List<BE_Pedido> listPedidos = ((List<BE_Pedido>)resultadoTransaccionPedido.dataList);

                    foreach (BE_Pedido pedido in listPedidos)
                    {

                        ResultadoTransaccion<bool> resultadoTransaccionVentaAutomatica = await VentaAutomatica(pedido);

                        if (resultadoTransaccionVentaAutomatica.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaAutomatica.ResultadoDescripcion;

                            ResultadoTransaccion<bool> resultadoTransaccionVentaPedidoError = await RegistrarVentaPedidoError(pedido.codpedido, resultadoTransaccionVentaAutomatica.ResultadoDescripcion, 1);

                            if (resultadoTransaccionVentaPedidoError.ResultadoCodigo == -1)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaPedidoError.ResultadoDescripcion;

                                return vResultadoTransaccion;
                            }
                        }
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se generó correctamente";
                    vResultadoTransaccion.data = true;
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
        public async Task<ResultadoTransaccion<bool>> VentaAutomatica(BE_Pedido pedido)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                PacienteRepository pacienteRepository = new PacienteRepository(context, _configuration);
                ResultadoTransaccion<BE_Paciente> resultadoTransaccionPaciente = await pacienteRepository.GetExistenciaPaciente(pedido.codatencion);

                if (resultadoTransaccionPaciente.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPaciente.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionPaciente.dataList.Any())
                {

                    TipoCambioRepository tipoCambioRepository = new TipoCambioRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_TipoCambio> resultadoTransaccionTipoCambio = await tipoCambioRepository.GetObtieneTipoCambio();

                    if (resultadoTransaccionTipoCambio.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTipoCambio.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    BE_TipoCambio tipoCambio = new BE_TipoCambio();

                    if (resultadoTransaccionTipoCambio.dataList.Any())
                    {
                        tipoCambio = ((List<BE_TipoCambio>)resultadoTransaccionTipoCambio.dataList)[0];
                    }


                    BE_Paciente paciente = ((List<BE_Paciente>)resultadoTransaccionPaciente.dataList)[0];

                    BE_VentasCabecera ventasCabecera = new BE_VentasCabecera
                    {
                        codalmacen = pedido.codalmacen,
                        codatencion = paciente.codatencion,
                        codpaciente = paciente.codpaciente,
                        nombre = paciente.nombrepaciente,
                        dircliente = paciente.direccion,
                        codaseguradora = paciente.codaseguradora,
                        nombreaseguradora = paciente.nombreaseguradora,
                        codcia = paciente.codcia,
                        nombrecia = paciente.nombrecontratante,
                        codpoliza = paciente.poliza,
                        planpoliza = paciente.planpoliza,
                        cama = paciente.cama,
                        deducible = paciente.deducible,
                        observacion = paciente.observacionespaciente,
                        porcentajecoaseguro = (decimal)paciente.coaseguro,
                        tipocambio = tipoCambio.Rate,
                        codmedico = pedido.codmedico,
                        nombremedico = pedido.nommedico,
                        flagpaquete = pedido.flg_paquete,
                        codpedido = pedido.codpedido,
                        codcentro = pedido.codcentro,
                        codtipocliente = "01", /*Por default Paciente*/
                        moneda = "S",
                        usuario = "admin",
                        tipomovimiento = "DV",
                        RegIdUsuario = 1,
                        idparaquien = 1, /*Para quien => Paciente*/
                        cardcodeparaquien = paciente.cardcode /*Para quien => Paciente*/
                    };

                    ResultadoTransaccion<string> resultadoTransaccionCama = await GetListaCamaPorAtencion(pedido.codatencion, pedido.codcentro);

                    if (resultadoTransaccionCama.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCama.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    string cama = resultadoTransaccionCama.data;

                    ventasCabecera.cama = cama;

                    ResultadoTransaccion<string> resultadoTransaccionTipoProductoPrestacion = new ResultadoTransaccion<string>();

                    if (!string.IsNullOrEmpty(pedido.codprestacion))
                    {
                        resultadoTransaccionTipoProductoPrestacion = await GetListaTipoProductoPrestacionVentaAutomatica(pedido.codprestacion);

                        if (resultadoTransaccionTipoProductoPrestacion.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionTipoProductoPrestacion.ResultadoDescripcion;

                            return vResultadoTransaccion;
                        }

                        ventasCabecera.flagpaquete = "S";

                    }
                    PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration, _clientFactory);
                    ResultadoTransaccion<BE_PedidoDetalle> resultadoTransaccionDetallePedido = await pedidoRepository.GetListPedidoDetallePorPedido(pedido.codpedido);

                    if (resultadoTransaccionDetallePedido.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetallePedido.ResultadoDescripcion;

                        return vResultadoTransaccion;
                    }

                    PlanesRepository planesRepository = new PlanesRepository(context, _configuration);

                    if (paciente.codaseguradora == "0019")
                    {
                        paciente.codplan = "00000006";
                        ResultadoTransaccion<BE_Planes> planes = await planesRepository.GetbyCodigo(new BE_Planes { CodPlan = "00000006" });

                        if (planes.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = planes.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        paciente.porcentajeplan = (decimal)planes.data.PorcentajeDescuento;

                    }
                    else
                    {
                        paciente.codplan = null;
                        paciente.porcentajeplan = 0;
                    }

                    if (resultadoTransaccionDetallePedido.dataList.Any())
                    {
                        List<BE_PedidoDetalle> listPedidoDetalle = ((List<BE_PedidoDetalle>)resultadoTransaccionDetallePedido.dataList);

                        List<BE_VentasDetalle> listVentasDetalles = new List<BE_VentasDetalle>();

                        foreach (BE_PedidoDetalle itemPedido in listPedidoDetalle)
                        {
                            ProductoRepository productoRepository = new ProductoRepository(_clientFactory, _configuration, context);
                            ResultadoTransaccion<BE_Producto> resultadoTransaccionProducto = await productoRepository.GetProductoPorCodigo(pedido.codalmacen, itemPedido.codproducto, paciente.codaseguradora, paciente.codcia, "DV", "01", string.Empty, paciente.codpaciente, paciente.idegctipoatencionmae, true);

                            if (resultadoTransaccionProducto.dataList.Any())
                            {
                                BE_Producto producto = ((List<BE_Producto>)resultadoTransaccionProducto.dataList)[0];

                                decimal isValorVVP = 0;
                                decimal isValorPVP = 0;
                                string isGrupoArticulo = string.Empty;

                                if (producto.FlgConvenio)
                                {
                                    isValorVVP = producto.valorVVP;
                                    isValorPVP = Math.Round((isValorVVP * (producto.valorIGV / 100 + 1)), 2);
                                }
                                else
                                {
                                    isValorVVP = producto.valorVVP;
                                    isValorPVP = producto.valorPVP;
                                }

                                if (producto.U_SYP_MONART != null)
                                {
                                    if (producto.U_SYP_MONART == "D")
                                    {
                                        isValorPVP = isValorPVP * tipoCambio.Rate;
                                        isValorVVP = isValorVVP * tipoCambio.Rate;
                                    }
                                }

                                switch (producto.ItemsGroupCode)
                                {
                                    case 111:
                                        isGrupoArticulo = "F";
                                        break;
                                    case 132:
                                        isGrupoArticulo = "T";
                                        break;
                                    case 134:
                                        isGrupoArticulo = "X";
                                        break;
                                    case 101:
                                        isGrupoArticulo = "A";
                                        break;
                                    default:
                                        isGrupoArticulo = string.Empty;
                                        break;
                                }

                                if (ventasCabecera.flagpaquete.Equals("S"))
                                {
                                    isGrupoArticulo = resultadoTransaccionTipoProductoPrestacion.data;
                                }

                                //calcula el preciounidadcondcto
                                //decimal valorDescuento = paciente.porcentajeplan;
                                decimal precioUnidadConDcto = isValorVVP - ((paciente.porcentajeplan / 100) * isValorVVP);

                                //calcula montontotaltemp
                                decimal montoTotalTemp = ((decimal)itemPedido.cantidad * precioUnidadConDcto);

                                decimal montoTotal = montoTotalTemp;

                                //calcula monto coaseguro para tipocliente = '01' = Paciente

                                decimal montoCoaseguro = montoTotal * ((decimal)paciente.coaseguro / 100); // Paga Paciente

                                decimal montoSeguro = montoTotal - montoCoaseguro; //Paga la Aseguradora

                                //montontotal con IGV

                                decimal porcentajeIgv = itemPedido.igv / 100;

                                decimal montoTotalIGV = montoTotal * (1 + porcentajeIgv);

                                string gnc = "N";

                                if (itemPedido.codproducto.Equals("00085516") || itemPedido.codproducto.Equals("00089245"))
                                {
                                    if ((paciente.codaseguradora.Equals("0013") || paciente.codaseguradora.Equals("0111")) && (paciente.codatencion.Substring(0, 1).Equals("E") || paciente.codatencion.Substring(0, 1).Equals("H")))
                                    {
                                        gnc = "S";
                                    }
                                }

                                if (!paciente.codaseguradora.Equals("0001") && !paciente.codatencion.Substring(0, 1).Equals("A"))
                                {
                                    gnc = producto.GastoCubierto ? "S" : "N";
                                }

                                if (ventasCabecera.flagpaquete.Equals("S"))
                                {
                                    gnc = "N";
                                }

                                BE_VentasDetalle ventasDetalle = new BE_VentasDetalle
                                {
                                    manBtchNum = producto.manbtchnum,
                                    codalmacen = pedido.codalmacen,
                                    tipomovimiento = "DV",
                                    codproducto = producto.ItemCode,
                                    nombreproducto = producto.ItemName,
                                    cantidad = (int)itemPedido.cantidad,
                                    precioventaPVP = isValorPVP,
                                    valorVVP = isValorVVP,
                                    porcentajedctoproducto = 0,
                                    porcentajedctoplan = paciente.porcentajeplan,
                                    montototal = montoTotal,
                                    montopaciente = montoCoaseguro,
                                    montoaseguradora = montoSeguro,
                                    codpedido = pedido.codpedido,
                                    gnc = gnc,
                                    codtipoproducto = isGrupoArticulo,
                                    preciounidadcondcto = precioUnidadConDcto,
                                    igvproducto = itemPedido.igv,
                                    narcotico = producto.Narcotico,
                                    flgbtchnum = false,
                                    stockfraccion = producto.U_SYP_CS_FRVTA == null ? 0 : (int)producto.U_SYP_CS_FRVTA,
                                    totalconigv = montoTotalIGV,
                                    totalsinigv = montoTotal,
                                    stockalmacen = (int)producto.ProductoStock
                                };

                                listVentasDetalles.Add(ventasDetalle);

                            }
                        }

                        ventasCabecera.listaVentaDetalle = listVentasDetalles;
                    }

                    ResultadoTransaccion<BE_VentasGenerado> newVenta = await RegistrarVentaCabecera(ventasCabecera, true);

                    if (newVenta.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = newVenta.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.data = true;

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<string>> GetListaCamaPorAtencion(string codatencion, string codcentro)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_GET_CAMA_POR_ATENCION, conn))
                    {
                        cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdGncPorVenta.Parameters.Add(new SqlParameter("@codatencion", codatencion));
                        cmdGncPorVenta.Parameters.Add(new SqlParameter("@codcentro", codcentro));

                        string response = string.Empty;

                        await conn.OpenAsync();

                        using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = ((reader["cama"]) is DBNull) ? string.Empty : (string)reader["cama"];
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
        public async Task<ResultadoTransaccion<string>> GetListaTipoProductoPrestacionVentaAutomatica(string pcodprestacionpaq)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_GET_TIPOPRODUCTO_PRESTACION, conn))
                    {
                        cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
                        cmdGncPorVenta.Parameters.Add(new SqlParameter("@pcodprestacionpaq", pcodprestacionpaq));

                        string response = string.Empty;

                        await conn.OpenAsync();

                        using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = ((reader["codtipoproducto"]) is DBNull) ? string.Empty : (string)reader["codtipoproducto"];
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
        public async Task<ResultadoTransaccion<bool>> RegistrarVentaPedidoError(string codpedido, string observacion, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_VENTA_PEDIDO_ERROR, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));
                        cmd.Parameters.Add(new SqlParameter("@observacion", observacion));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));

                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                        vResultadoTransaccion.data = true;
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
        public async Task<ResultadoTransaccion<MemoryStream>> GenerarValeVentaPrint(string codventa)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.Letter);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 14f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var title = string.Format("VENTAS Nro {0}", codventa, titulo);

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);
                
                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));
                //Obtenemos los datos de la venta

                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa);

                if (resultadoTransaccionVenta.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                // Generamos la Cabecera del vale de venta
                tbl = new PdfPTable(new float[] { 12f, 45f, 3f, 12f, 28f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Nombre", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.nombre), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Plan", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.porcentajedctoplan.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 2
                c1 = new PdfPCell(new Phrase("Atención", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.codatencion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Aseguradora", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.nombreaseguradora), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 3
                c1 = new PdfPCell(new Phrase("H. Cliníca", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.codpaciente), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Coaseguro", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.porcentajecoaseguro.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 4
                c1 = new PdfPCell(new Phrase("Médico", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.nombremedico), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cama", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.cama), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 5
                c1 = new PdfPCell(new Phrase("Observación", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.observacion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("F.Emisión", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.fechaemision.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                
                //Linea 6
                c1 = new PdfPCell(new Phrase("", parrafoNegroNegrita)) { Border = 0 };
                c1.Colspan = 3;
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Paquete", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", ""), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta
                tbl = new PdfPTable(new float[] { 30f, 15f, 8f, 8f, 8f, 8f, 8f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Producto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Lab.", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("P.Unit", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cant.", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("D.Prod.", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("D.Plan", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Valor", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);

                foreach (BE_VentasDetalle item in resultadoTransaccionVenta.data.listaVentaDetalle)
                {
                    c1 = new PdfPCell(new Phrase(item.nombreproducto, parrafoNegro)) { Border = 0};
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.nombrelaboratorio, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.preciounidadcondcto.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.cantidad.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.porcentajedctoproducto.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.porcentajedctoplan.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.montototal.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                }
                doc.Add(tbl);

                doc.Add(new Phrase(" "));
                doc.Add(new Phrase(" "));
                // Totales
                tbl = new PdfPTable(new float[] { 70f, 15f, 15f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("NO INCLUYE IGV", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Total :", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(resultadoTransaccionVenta.data.montototal.ToString(), parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(resultadoTransaccionVenta.data.usuario, parrafoNegroNegrita)) { Border = 0};
                c1.Colspan = 3;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));
                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Vendedor", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Personal de Reparto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Personal Recepciona", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Paciente", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.BOTTOM_BORDER);
                tbl.AddCell(c1);
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<MemoryStream>> GenerarValeVentaLotePrint(string codventa)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.Letter);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 14f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var title = string.Format("VENTAS Nro {0}", codventa, titulo);

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));
                //Obtenemos los datos de la venta

                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVenta(codventa);

                if (resultadoTransaccionVenta.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                // Generamos la Cabecera del vale de venta
                tbl = new PdfPTable(new float[] { 15f, 75f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("ALMACEN", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.nombrealmacen), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("FECHA MOV", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccionVenta.data.fechaemision.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta
                tbl = new PdfPTable(new float[] { 40f, 20f, 10f, 15f, 15f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Producto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Lab.", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cant.", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Lote", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("F. Vencimiento", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);

                foreach (BE_VentasDetalle item in resultadoTransaccionVenta.data.listaVentaDetalle)
                {
                    if (item.listVentasDetalleLotes != null)
                    {
                        if (item.listVentasDetalleLotes.Any())
                        {
                            foreach (var itemLote in item.listVentasDetalleLotes)
                            {
                                c1 = new PdfPCell(new Phrase(item.nombreproducto, parrafoNegro)) { Border = 0 };
                                tbl.AddCell(c1);
                                c1 = new PdfPCell(new Phrase(item.nombrelaboratorio, parrafoNegro)) { Border = 0 };
                                tbl.AddCell(c1);
                                c1 = new PdfPCell(new Phrase(itemLote.cantidad.ToString(), parrafoNegro)) { Border = 0 };
                                tbl.AddCell(c1);
                                c1 = new PdfPCell(new Phrase(itemLote.lote, parrafoNegro)) { Border = 0 };
                                tbl.AddCell(c1);
                                c1 = new PdfPCell(new Phrase(((DateTime)itemLote.fechavencimiento).ToShortDateString(), parrafoNegro)) { Border = 0 };
                                tbl.AddCell(c1);
                            }
                        }
                    }
                }
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<bool>> UpdateSAPVenta(BE_VentasCabecera value, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CABECERA_SAP, conn, transaction))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@codventa", value.codventa));
                    cmd.Parameters.Add(new SqlParameter("@ide_docentrysap", value.ide_docentrysap));
                    cmd.Parameters.Add(new SqlParameter("@fec_docentrysap", value.fec_docentrysap));
                    cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));

                    SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdTransaccionParam);

                    SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputMsjTransaccionParam);
                    //await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    vResultadoTransaccion.data = true;
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
        public async Task<ResultadoTransaccion<bool>> UpdateSAPNota(string codnota, int doc_entry, int RegIdUsuario, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CABECERA_NOTA_SAP, conn, transaction))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@codnota", codnota));
                    cmd.Parameters.Add(new SqlParameter("@doc_entry", doc_entry));
                    cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", RegIdUsuario));

                    SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdTransaccionParam);

                    SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputMsjTransaccionParam);
                    //await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    vResultadoTransaccion.data = true;
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
        public async Task<ResultadoTransaccion<bool>> UpdateSinStockVenta(BE_VentaXml value)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CABECERA_SIN_STOCK, conn, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        cmd.Parameters.Add(new SqlParameter("@xmldata", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));

                        SqlParameter outputDocEntryTransaccionParam = new SqlParameter("@doc_entry", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputDocEntryTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        //await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        var docentry = int.Parse(outputDocEntryTransaccionParam.Value.ToString());

                        if (docentry.Equals(0))
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Venta sin stock, no cuenta con Factura/Boleta de Reserva";
                            return vResultadoTransaccion;
                        }

                        vResultadoTransaccion.IdRegistro = int.Parse(outputDocEntryTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                        vResultadoTransaccion.data = true;
                    }

                    #region "Envio a SAP"
                    ResultadoTransaccion<bool> resultadoTransaccionVenta = await EnviarVentaAsociadaReservaSAP(conn, transaction, value.codventa, vResultadoTransaccion.IdRegistro, (int)value.RegIdUsuario);

                    if (resultadoTransaccionVenta.IdRegistro == -1)
                    {
                        transaction.Rollback();
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }
                    #endregion

                    transaction.Commit();
                    transaction.Dispose();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
                conn.Close();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_VentasGenerado>> RegistrarVentaDevolucion(BE_VentaDevolucionXml value)
        {
            ResultadoTransaccion<BE_VentasGenerado> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasGenerado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<BE_VentasGenerado> ventasGenerados = new List<BE_VentasGenerado>();

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_DEVOLUCION, conn, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        SqlParameter oParam = new SqlParameter("@codventa", SqlDbType.Char, 8)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParam);

                        SqlParameter oParamNota = new SqlParameter("@codcomprobanteNC", SqlDbType.Char, 11)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamNota);

                        SqlParameter oParamSinStock = new SqlParameter("@flgsinstock", SqlDbType.Bit, 0)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParamSinStock);

                        cmd.Parameters.Add(new SqlParameter("@xmldata", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        //await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        var codcomprobanteNota = oParamNota.Value == null ? "" : (string)oParamNota.Value;

                        ventasGenerados.Add(new BE_VentasGenerado { codventa = (string)oParam.Value, codpresotor = string.Empty, codcomprobanteNotaCredito = codcomprobanteNota });

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                        vResultadoTransaccion.dataList = ventasGenerados;

                        bool flgsinstock = bool.Parse(oParamSinStock.Value.ToString());

                        //NOTA DE CREDITO
                        if (value.tipodevolucion == "02")
                        {
                            #region "Envio a TCI"

                            if (value.flgelectronico && value.codcomprobante != null && codcomprobanteNota != "")
                            {
                                SerieRepository serieRepository = new SerieRepository(context, _configuration);
                                ResultadoTransaccion<BE_SerieConfig> resultadoTransSerie = new ResultadoTransaccion<BE_SerieConfig>();


                                ComprobanteElectronicoRepository CPERepository = new ComprobanteElectronicoRepository(context, _configuration);
                                ResultadoTransaccion<string> vResultado1 = new ResultadoTransaccion<string>();

                                vResultado1 = await CPERepository.GetValirdacionElectronicaNota("0001", value.codcomprobante, "", "L", null, 3, conn, transaction);

                                if (vResultado1.data == "N")
                                {
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = "No puede generar la Nota Electronica. Revisar datos de FV/BV";
                                    return vResultadoTransaccion;
                                }

                                resultadoTransSerie = await serieRepository.GetListConfigDocumentoPorNombreMaquinaTrans(value.nombremaquina, conn, transaction);

                                string wFlg_otorgar = string.Empty;
                                string wStrURL = string.Empty;
                                string xProximaNota = string.Empty;

                                if (resultadoTransSerie.IdRegistro == 0)
                                {
                                    switch (value.codcomprobante.Substring(0, 1))
                                    {
                                        case "B":
                                            wFlg_otorgar = resultadoTransSerie.data.flg_otorgarcb.ToString();
                                            break;
                                        case "F":
                                            wFlg_otorgar = resultadoTransSerie.data.flg_otorgarcf.ToString();
                                            break;
                                    }
                                }

                                ResultadoTransaccion<BE_Tabla> resultadoTransaTabla = null;
                                TablaRepository tablaRepository = new TablaRepository(context, _configuration);

                                resultadoTransaTabla = new ResultadoTransaccion<BE_Tabla>();

                                resultadoTransaTabla = await tablaRepository.GetTablasTCIWebService("EFACT_TCI_WS");

                                if (resultadoTransaTabla.IdRegistro == 0)
                                {
                                    wStrURL = resultadoTransaTabla.data.nombre;
                                }

                                resultadoTransaTabla = new ResultadoTransaccion<BE_Tabla>();
                                resultadoTransaTabla = await tablaRepository.GetTablasClinicaPorFiltros("EFACT_TIPOCODIGO_BARRAHASH", "01", 50, 1, -1);

                                string wTipoCodigo_BarraHash = string.Empty;

                                if (resultadoTransaTabla.IdRegistro == 0)
                                {
                                    wTipoCodigo_BarraHash = resultadoTransaTabla.data.valor.ToString();
                                }
                                if (wTipoCodigo_BarraHash == "" && wTipoCodigo_BarraHash != "0" && wTipoCodigo_BarraHash != "1")
                                {
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = "Solicite asignar se imprimirá Codigo de Barras o Codigo Hash.";
                                    return vResultadoTransaccion;
                                }

                                string TipoComp_TCI = string.Empty;
                                string wTipoCodigo = string.Empty;
                                string wOtorgar = string.Empty;

                                wTipoCodigo = wTipoCodigo_BarraHash; //  '"1" 'Se envía "0" si se desea obtener el código de barras; se envía "1" si se desea obtener el código hash.
                                wOtorgar = wFlg_otorgar;             //  '"1" 'Se envía "0" si se otorgará manualmente, se envía "1" si se otorgará automáticamente

                                //Factura,Boleta
                                resultadoTransaTabla = await tablaRepository.GetTablasClinicaPorFiltros("EFACT_TIPOCOMP_FORCALL_WS_TCI", codcomprobanteNota.Substring(0, 1), 50, 1, -1);
                                if (resultadoTransaTabla.IdRegistro == 0)
                                {
                                    TipoComp_TCI = resultadoTransaTabla.data.nombre;
                                }

                                //--Construir XML
                                string wXML = string.Empty;

                                ResultadoTransaccion<BE_ComprobanteElectronico> resultadoTransaccionComprobanteElectronico = await CPERepository.GetNotaElectronicaXml(codcomprobanteNota, 0, conn, transaction);

                                List<BE_ComprobanteElectronico> response = (List<BE_ComprobanteElectronico>)resultadoTransaccionComprobanteElectronico.dataList;

                                var pRetorno = Metodo_Registrar_FB(response, wTipoCodigo_BarraHash, wFlg_otorgar.ToString(), ref wXML);

                                if (pRetorno != 0)
                                {
                                    transaction.Rollback();
                                    if (pRetorno == -1)
                                    {
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = "Comprobante no generado: Error al construir XML. Detalle tiene monto negativo";
                                    }
                                    else
                                    {
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = "Comprobante no generado: Error al construir XML";
                                    }

                                    return vResultadoTransaccion;
                                }

                                //wStrURL = "http://200.106.52.10/WS_TCI/Service.asmx?wsdl";
                                string strURL = wStrURL;

                                string strSOAPAction = "http://tempuri.org/Registrar";

                                var parser = new DOMDocument30();
                                parser.loadXML(wXML);

                                if (!parser.loadXML(wXML))
                                {
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = "Error al construir XML. Comuniquese con el area de sistemas";
                                    return vResultadoTransaccion;
                                }

                                //Indicar el parámetro a enviar - AG: Lo que hacemos es asignar el parámetro que queremos pasarle a la función Registrar

                                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/oTipoComprobante").text = TipoComp_TCI;   //'TCI-Regla: enviar "Factura","Boleta","NotaCredito","NotaDebito"
                                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/TipoCodigo").text = wTipoCodigo_BarraHash;  //'TCI "1"
                                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/Otorgar").text = wFlg_otorgar.ToString();            //'TCI "1"
                                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/IdComprobanteCliente").text = "0";        //'TCI-Regla: enviar CodComprobante en int; 08/06/2015:No se enviara porq TCI no soporta(INTEGER) series mayores a 214; Mid(wCodComprobante, 2, 10)

                                string strXml;
                                strXml = parser.xml;

                                // 'I-Call PostWebservice y Leer rpta de TCI - usando code de csf
                                DOMDocument30 xmlResponse = new DOMDocument30();
                                string wResponseXML = string.Empty;
                                string wResponseTXT = string.Empty;
                                string wCodigoHash = string.Empty;
                                string wCodigoBarra = string.Empty;
                                string wCadenaRpta = string.Empty;

                                ComprobanteElectronicaTCIXml comprobanteElectronicaTCIXml = new ComprobanteElectronicaTCIXml();

                                if (comprobanteElectronicaTCIXml.InvokeWebServiceTCI(strXml.Trim(), strSOAPAction, strURL, ref xmlResponse))
                                {

                                    wResponseXML = xmlResponse.xml;
                                    wResponseTXT = xmlResponse.text;

                                    // 'I-Leer parametro de salida Retorno(true/false) para saber si se registró en SUNAT - (Ref: AgregarPacientesCSFaTisal)
                                    DOMDocument xml_document = new DOMDocument();
                                    xml_document.loadXML(wResponseXML);

                                    string ResultStatus = "/soap:Envelope/soap:Body/RegistrarResponse/";
                                    string wStatus = xml_document.selectSingleNode(ResultStatus + "RegistrarResult").text;

                                    if (wStatus == "true")
                                    {
                                        vResultado1 = await CPERepository.ModificarComprobanteElectronico_transac("fecha_registro_rpta", "", "", codcomprobanteNota, conn, transaction);
                                        vResultado1 = await CPERepository.ModificarComprobanteElectronico_transac("tipo_otorgamiento", wFlg_otorgar.ToString(), "", codcomprobanteNota, conn, transaction);
                                        vResultado1 = await CPERepository.ModificarComprobanteElectronico_transac("xml_registro", "", strXml.Trim(), codcomprobanteNota, conn, transaction);

                                        wStatus = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<RegistrarResult>", "</RegistrarResult>");
                                        wCadenaRpta = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<Cadena>", "</Cadena>");

                                        string observado = wStatus + "; " + (wCadenaRpta.Trim() == "" ? "" : wCadenaRpta.Trim().Substring(0, 3980)); // wCadenaRpta.Trim().Substring(0, 3980);

                                        vResultado1 = await CPERepository.ModificarComprobanteElectronico_transac("observacion_registro", observado, "", codcomprobanteNota, conn, transaction);
                                        //wCodigoHash = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<CodigoHash>", "</CodigoHash>");
                                        wCodigoBarra = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<CodigoBarras>", "</CodigoBarras>");
                                        vResultado1 = await CPERepository.ModificarComprobanteElectronico_transac("codigohash", wCodigoHash, "", codcomprobanteNota, conn, transaction);

                                        //zEfact.ConvertirCodigoBarraJPG txtCodComprobante.Text, wCodigoBarra, zFarmacia2
                                        // 'Convertimos el texto en un Array de byte()

                                        byte[] xObtenerByte = Convert.FromBase64String(wCodigoBarra);
                                        var resultCodBarra = ComprobanteElectronico_Update_transac("codigobarra", "", "", xObtenerByte, codcomprobanteNota, conn, transaction);
                                        if (resultCodBarra == false) throw new ArgumentException("Error ComprobanteElectronico_Update_transac codigobarra");
                                        //var resultCodBarra = ComprobanteElectronico_Update("codigobarra", "", "", xObtenerByte, codcomprobante);
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        wCadenaRpta = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<Cadena>", "</Cadena>");
                                        vResultado1 = await EnviarCorreoError((string)oParam.Value, codcomprobanteNota, response[0].ref_codcomprobantee, response[0].codatencion, response[0].codtipocliente, response[0].codtipocliente, response[0].nombrepaciente, response[0].anombrede, value.nombremaquina, wCadenaRpta);
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = "Cadena: " + wCadenaRpta + "<br> Metodo_Registrar: " + value.codcomprobante;
                                        return vResultadoTransaccion;
                                    }
                                }
                                else
                                {
                                    // SISQUE OCURRE UN ERROR AL ENVIAR A CTI
                                    transaction.Rollback();
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = "Metodo_Registrar: " + value.codcomprobante + "  Error al invocar WebService : InvokeWebService_CSF";
                                    //EnviarCorreo_Error wCodcomprobante, "Error al invocar WebService (Metodo_Registrar)"
                                    var mensaje = "Error al invocar WebService (Metodo_Registrar)";
                                    vResultado1 = await EnviarCorreoError((string)oParam.Value, codcomprobanteNota, response[0].ref_codcomprobantee, response[0].codatencion, response[0].codtipocliente, response[0].nomtipocliente, response[0].nombrepaciente, response[0].anombrede, value.nombremaquina, mensaje);

                                    return vResultadoTransaccion;
                                }
                            }

                            #endregion
                        }

                        #region "Envio a SAP"
                        if (!flgsinstock && value.tipodevolucion == "01")
                        {
                            //Solo cuando es devolucion sin NC
                            ResultadoTransaccion<bool> resultadoTransaccionVenta = await DevolucionVentaSAPBase(conn, transaction, (string)oParam.Value, (int)value.RegIdUsuario);

                            if (resultadoTransaccionVenta.IdRegistro == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }
                        }

                        if (!flgsinstock && value.tipodevolucion == "02")
                        {
                            //Solo cuando NC
                            ResultadoTransaccion<bool> resultadoTransaccionNotaCredito = await NotaVentaSAP(conn, transaction, codcomprobanteNota, (int)value.RegIdUsuario);

                            if (resultadoTransaccionNotaCredito.IdRegistro == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionNotaCredito.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }
                        }

                        if (flgsinstock && value.tipodevolucion == "02")
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Funcionalidad no habilitada para NC con vale de ventas Sin Stock";
                            return vResultadoTransaccion;
                        }

                        #endregion
                    }

                    transaction.Commit();
                    transaction.Dispose();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }

                conn.Close();

            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<MemoryStream>> GenerarHojaDatosPrint(string codatencion)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.Letter);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 9.5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                RecetaRepository recetaRepository = new RecetaRepository(context, _configuration);
                ResultadoTransaccion<BE_HojaDato> resultadoTransaccion = await recetaRepository.GetRpHojadeDatos(codatencion);

                List<BE_HojaDato> response = (List<BE_HojaDato>)resultadoTransaccion.dataList;

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var title = string.Format("HOJA DE DATOS DE PACIENTE", titulo);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos la Cabecera del vale de venta

                //Cabecera 01

                tbl = new PdfPTable(new float[] { 19f, 30f, 2f, 11f, 38f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Nro Atención", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].codatencion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                //Cabecera 02
                tbl = new PdfPTable(new float[] { 19f, 30f, 2f, 11f, 38f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Historia Clínica", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].codpaciente), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 2
                c1 = new PdfPCell(new Phrase("Fecha Inicio", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].fechainicio), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Usuario", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].login), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 3
                c1 = new PdfPCell(new Phrase("Fecha Fin", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].fechafin), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cama", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].cama), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 4
                c1 = new PdfPCell(new Phrase("Apellidos y Nombres", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombres), parrafoNegro)) { Border = 0 };
                c1.Colspan = 4;
                tbl.AddCell(c1);

                //Linea 5
                c1 = new PdfPCell(new Phrase("Doc. Identidad", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].docidentidad), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Provincia", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombreprovincia), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 6
                c1 = new PdfPCell(new Phrase("Sexo", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombresexo), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Distrito", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombredistrito), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 7
                c1 = new PdfPCell(new Phrase("Edad", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].edad.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Dirección", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].direccion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 8
                c1 = new PdfPCell(new Phrase("Estado Civil", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombrecivil), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Parentesco", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombreparentesco), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 9
                c1 = new PdfPCell(new Phrase("Telefóno", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].telefono), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Titular", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].titular), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 10
                c1 = new PdfPCell(new Phrase("Médico", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].medicotratante), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 11
                c1 = new PdfPCell(new Phrase("Especialidad", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].Especialidad), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 12
                c1 = new PdfPCell(new Phrase("Tipo de consulta médica", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].est_consulta_medica), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                //doc.Add(new Phrase(""));

                //Cabecera 03
                tbl = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100 };

                //Linea 1: Linea en blanco
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 2
                c1 = new PdfPCell(new Phrase("Observaciones como Asegurado", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Observaciones como Paciente", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 3
                c1 = new PdfPCell(new Phrase(response[0].observacionesasegurado, parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(response[0].observacionespaciente, parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                //Cabecera 04
                tbl = new PdfPTable(new float[] { 19f, 30f, 2f, 20f, 29f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Aseguradora", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombreaseguradora), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha Inicio Vigencia", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].fechafinvigencia), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 2
                c1 = new PdfPCell(new Phrase("Contratante", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombrecontratante), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha Fin Vigencia", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].fechafinvigencia), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 3
                c1 = new PdfPCell(new Phrase("Poliza", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].npoliza), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Duración Solicitud", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].numerodiasatencion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 4
                c1 = new PdfPCell(new Phrase("Tarifa", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombretarifa), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);

                //Linea 5: Linea blanco
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                //doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta

                tbl = new PdfPTable(new float[] { 25f, 20f, 10f, 10f, 5f, 30f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("COBERTURA", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("CONCEPTO", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("TIPO", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("MONTO", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("IGV", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("OBSERVACIONES", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);

                foreach (BE_HojaDato item in response)
                {
                    c1 = new PdfPCell(new Phrase(item.nombre, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.nombreconcepto, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(((item.tipomonto == "P") ? "%" : item.nombremoneda), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.monto.ToString("N2"), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.igv.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.observaciones.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                }
                doc.Add(tbl);

                doc.Add(new Phrase(" "));


                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_VentasGenerado>> RegistrarPedidoDevolucion(BE_VentaXml value)
        {
            ResultadoTransaccion<BE_VentasGenerado> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasGenerado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<BE_VentasGenerado> ventasGenerados = new List<BE_VentasGenerado>();

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var response = new BE_SeperacionCuentaGenerado();

                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_DEVOLUCION_PEDIDO, conn, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter outputGeneradoTransaccionParam = new SqlParameter("@cadenacodventa", SqlDbType.Xml, 0)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputGeneradoTransaccionParam);

                        cmd.Parameters.Add(new SqlParameter("@xmldata", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        //await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        XmlSerializer serializer = new XmlSerializer(typeof(BE_SeperacionCuentaGenerado));
                        using (TextReader reader = new StringReader(outputGeneradoTransaccionParam.Value.ToString()))
                        {
                            response = (BE_SeperacionCuentaGenerado)serializer.Deserialize(reader);
                        }

                        foreach (BE_SeperacionCuenta item in response.ListSeperacionCuentaGenerado)
                        {
                            ResultadoTransaccion<bool> resultadoTransaccionVenta = await DevolucionVentaSAP(conn, transaction, item.codventa, (int)value.RegIdUsuario);

                            if (resultadoTransaccionVenta.IdRegistro == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            var ventasGenerado = new BE_VentasGenerado
                            {
                                codventa = item.codventa,
                                codpresotor = string.Empty
                            };

                            ventasGenerados.Add(ventasGenerado);

                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                        vResultadoTransaccion.dataList = ventasGenerados;

                    }

                    transaction.Commit();
                    transaction.Dispose();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //conn.Close();
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }

                conn.Close();

            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<string>> EnviarCorreoError(string codventa, string pCodcomprobante, string codcomprobanteref, string codatencion, string codtipocliente, string nombretipocliente, string nombrepacientecliente, string anombredequien, string nombremaquina, string mensaje)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            string destino = string.Empty;
            string asunto = string.Empty;
            string body = string.Empty;
            string secuencia = "\r";

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    CorreoRepository correoRepository = new CorreoRepository(context, _configuration);
                    vResultadoTransaccion = await correoRepository.GetCorreoDestinatario("EFACT_CORREO_ERRORENREGISTRO", "TO");

                    if (vResultadoTransaccion.IdRegistro == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "NO HAY CORREO DESTINO CONFIGURADO";
                    }

                    destino = vResultadoTransaccion.data;

                    UsuarioRepository usuarioRepository = new UsuarioRepository(context, _configuration);
                    ResultadoTransaccion<BE_Usuario> vResultadoTransaccionUsuario = new ResultadoTransaccion<BE_Usuario>();
                    vResultadoTransaccionUsuario = await usuarioRepository.GetUsuarioPorCodVenta(codventa);


                    asunto = "EFact-LOG error en metodo registrar Nota a Pac  " + pCodcomprobante + " - " + codatencion + " - " + nombretipocliente + " - " + vResultadoTransaccionUsuario.data.login;

                    body = "WebService indica que proceso de registrar comprobantes presentó inconvenientes: " + secuencia + secuencia +
                           "Nota (PK)        : " + pCodcomprobante?.Trim() + secuencia +
                           "FT/BV            : " + codcomprobanteref?.Trim() + secuencia +
                           "Tipo Cliente     : " + codtipocliente?.Trim() + " " + nombretipocliente?.Trim() + secuencia +
                           "CodAtencion      : " + codatencion?.Trim() +
                           "Paciente         : " + nombrepacientecliente?.Trim() +
                           "Anombre de       : " + anombredequien?.Trim() + secuencia +
                           "Usuario          : " + vResultadoTransaccionUsuario.data.nombrecreate?.Trim() + secuencia +
                           "PC               : " + nombremaquina?.Trim() + secuencia +
                           "Mensaje_Error    : " + mensaje?.Trim();


                    var correo = new BE_Correo()
                    {
                        enviara = destino,
                        asunto = asunto,
                        cuerpo = body,
                    };

                    vResultadoTransaccion = await correoRepository.Registrar(correo);

                    if (vResultadoTransaccion.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "ERROR AL GUARDAR EL CORREO";
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
        public async Task<ResultadoTransaccion<bool>> NotaVentaSAP(SqlConnection conn, SqlTransaction transaction, string codcomprobante, int RegIdUsuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ResultadoTransaccion<BE_VentasCabecera> resultadoTransaccionVenta = await GetVentaPorCodVentaNota(codcomprobante, conn, transaction);

                if (resultadoTransaccionVenta.IdRegistro == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "ERROR AL OBTENER VENTA";
                    return vResultadoTransaccion;
                }

                SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);

                ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocument = await sapDocuments.SetReturnsDocumentNota(resultadoTransaccionVenta.data);

                if (resultadoSapDocument.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoSapDocument.data.DocEntry > 0)
                {
                    resultadoTransaccionVenta.data.ide_docentrysap = resultadoSapDocument.data.DocEntry;
                    resultadoTransaccionVenta.data.fec_docentrysap = DateTime.Now;
                    resultadoTransaccionVenta.data.RegIdUsuario = RegIdUsuario;

                    ResultadoTransaccion<bool> resultadoTransaccionVentaUpd = await UpdateSAPNota(codcomprobante, resultadoSapDocument.data.DocEntry , RegIdUsuario, conn, transaction);

                    if (resultadoTransaccionVentaUpd.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVentaUpd.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    #region Pago Nota de Credito
                    //ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocumentNotaPago = await sapDocuments.SetReturnsDocumentNotaPago(resultadoTransaccionVenta.data);

                    //if (resultadoSapDocumentNotaPago.ResultadoCodigo == -1)
                    //{
                    //    vResultadoTransaccion.IdRegistro = -1;
                    //    vResultadoTransaccion.ResultadoCodigo = -1;
                    //    vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocumentNotaPago.ResultadoDescripcion;
                    //    return vResultadoTransaccion;
                    //}

                    //if (resultadoSapDocumentNotaPago.data.DocEntry != 0)
                    //{
                    //    using (SqlCommand cmd = new SqlCommand(SP_POST_CUADRECAJA_NOTA_SAP_UPDATE, conn, transaction))
                    //    {
                    //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //        cmd.Parameters.Add(new SqlParameter("@documento", codcomprobante));
                    //        cmd.Parameters.Add(new SqlParameter("@doc_entry", resultadoSapDocument.data.DocEntry));
                    //        cmd.Parameters.Add(new SqlParameter("@ide_trans", resultadoSapDocumentNotaPago.data.DocEntry));
                    //        cmd.Parameters.Add(new SqlParameter("@fec_enviosap", DateTime.Now));

                    //        await cmd.ExecuteNonQueryAsync();
                    //    }
                    //}
                    //else
                    //{
                    //    vResultadoTransaccion.IdRegistro = -1;
                    //    vResultadoTransaccion.ResultadoCodigo = -1;
                    //    vResultadoTransaccion.ResultadoDescripcion = "ERROR AL ENVIAR EL PAGO A SAP.";
                    //    return vResultadoTransaccion;
                    //}
                    #endregion
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
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaPorCodVentaNota(string codcomprobante, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                ComprobanteElectronicoRepository comprobante = new ComprobanteElectronicoRepository(context, _configuration);
                ResultadoTransaccion<BE_ComprobanteElectronico> resultadoTransaccionComprobanteElectronico = new ResultadoTransaccion<BE_ComprobanteElectronico>();

                resultadoTransaccionComprobanteElectronico = await comprobante.GetNotaElectronicaXml(codcomprobante, 0, conn, transaction);

                List<BE_ComprobanteElectronico> responseNotaElectronica = (List<BE_ComprobanteElectronico>)resultadoTransaccionComprobanteElectronico.dataList;

                var responseVenta = new BE_VentasCabecera()
                {
                    codventa = responseNotaElectronica[0].codventa,
                    codcomprobante = responseNotaElectronica[0].codcomprobante,
                    fechaemision = responseNotaElectronica[0].fechaemision,
                    cardcode = responseNotaElectronica[0].cardcode.Trim(),
                    moneda = responseNotaElectronica[0].c_simbolomoneda.Trim(),
                    observacion = responseNotaElectronica[0].observaciones.Trim(),
                    porcentajeimpuesto = responseNotaElectronica[0].porcentajeimpuesto,
                    montototal = responseNotaElectronica[0].d_total_conigv,
                    CuentaEfectivoPago = responseNotaElectronica[0].CuentaEfectivoPago,
                    cardcodeparaquien = responseNotaElectronica[0].cardcodeparaquien.Trim(),
                    codpresotor = responseNotaElectronica[0].codpresotor,
                    codventadevolucion = responseNotaElectronica[0].codventadevolucion,
                    flg_gratuito = responseNotaElectronica[0].flg_gratuito == "1" ? true : false,
                    U_SYP_CS_DNI_PAC = responseNotaElectronica[0].U_SYP_CS_DNI_PAC,
                    U_SYP_CS_NOM_PAC = responseNotaElectronica[0].U_SYP_CS_NOM_PAC,
                    U_SYP_CS_RUC_ASEG = responseNotaElectronica[0].U_SYP_CS_RUC_ASEG,
                    U_SYP_CS_NOM_ASEG = responseNotaElectronica[0].U_SYP_CS_NOM_ASEG,
                    U_SYP_MDTD = responseNotaElectronica[0].U_SYP_MDTD,
                    U_SYP_MDSD = responseNotaElectronica[0].U_SYP_MDSD,
                    U_SYP_MDCD = responseNotaElectronica[0].U_SYP_MDCD,
                    U_SYP_STATUS = responseNotaElectronica[0].U_SYP_STATUS,
                    NumAtCard = responseNotaElectronica[0].NumAtCard,
                    U_SYP_MDTO = responseNotaElectronica[0].U_SYP_MDTO,
                    U_SYP_MDSO = responseNotaElectronica[0].U_SYP_MDSO,
                    U_SYP_MDCO = responseNotaElectronica[0].U_SYP_MDCO,
                    U_SYP_FECHA_REF = responseNotaElectronica[0].U_SYP_FECHA_REF,
                    FederalTaxID = responseNotaElectronica[0].FederalTaxID,
                    U_SYP_MDMT = responseNotaElectronica[0].U_SYP_MDMT,
                    Comments = responseNotaElectronica[0].Comments,
                    JournalMemo = responseNotaElectronica[0].JournalMemo,
                    U_SYP_CS_USUARIO = responseNotaElectronica[0].U_SYP_CS_USUARIO,
                    ControlAccount = responseNotaElectronica[0].ControlAccount,
                    U_SYP_CS_OA_CAB = responseNotaElectronica[0].U_SYP_CS_OA_CAB,
                    U_SYP_CS_PAC_HC = responseNotaElectronica[0].U_SYP_CS_PAC_HC,
                    U_SYP_CS_FINI_ATEN = responseNotaElectronica[0].U_SYP_CS_FINI_ATEN,
                    U_SYP_CS_FFIN_ATEN = responseNotaElectronica[0].U_SYP_CS_FFIN_ATEN,
                    U_SBA_TIPONC = responseNotaElectronica[0].U_SBA_TIPONC
                };

                var responseDetalle = new List<BE_VentasDetalle>();

                foreach (var item in responseNotaElectronica)
                {
                    var linea = new BE_VentasDetalle()
                    {
                        coddetalle = item.d_orden,
                        codproducto = item.d_codproducto.Trim(),
                        cantsunat = item.d_cant_sunat,
                        preciounidad = responseNotaElectronica[0].flg_gratuito == "1" ? item.d_ventaunitario_sinigv_g : item.d_ventaunitario_sinigv,
                        destributo = item.des_tributo,
                        codalmacen = item.codalmacen,
                        //baseentry = item.baseentry,
                        //baseline = item.baseline,
                        AccountCode = item.AccountCode,
                        CostingCode = item.CostingCode,
                        CostingCode2 = item.CostingCode2,
                        CostingCode3 = item.CostingCode3,
                        CostingCode4 = item.CostingCode4,
                        manBtchNum = item.manbtchnum,
                        binactivat = item.binactivat,
                        TaxCode = item.TaxCode,
                        TaxOnly = item.TaxOnly,
                        Project = item.Project,
                        U_SYP_CS_OA = item.U_SYP_CS_OA,
                        U_SYP_CS_DNI_MED = item.U_SYP_CS_DNI_MED,
                        U_SYP_CS_NOM_MED = item.U_SYP_CS_NOM_MED,
                        U_SYP_CS_RUC_MED = item.U_SYP_CS_RUC_MED
                    };
                    responseDetalle.Add(linea);
                }

                var responseDetalleLote = new List<BE_VentasDetalleLote>();

                using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLEVENTA_LOTE_POR_CODVENTA, conn, transaction))
                {
                    foreach (BE_VentasDetalle item in responseDetalle)
                    {
                        if (item.manBtchNum || item.binactivat)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

                            //conn.Open();

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                responseDetalleLote = (List<BE_VentasDetalleLote>)context.ConvertTo<BE_VentasDetalleLote>(reader);
                            }

                            //conn.Close();

                            responseDetalle.Find(xFila => xFila.coddetalle == item.coddetalle).listVentasDetalleLotes = responseDetalleLote;
                        }
                    }
                }

                responseVenta.listaVentaDetalle = responseDetalle;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.data = responseVenta;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }
        private long Metodo_Registrar_FB(List<BE_ComprobanteElectronico> response, string pTipoCodigo_BarraHash, string pTipoOtorgamiento, ref string pXML)
        {
            pXML = string.Empty;
            string wTipoComp_TCI = string.Empty;

            //' ------------------- Invocación del Web Service
            string secuencia = "\r";

            wTipoComp_TCI = response[0].tipocomp_tci;

            var CPTCI_XML = new ComprobanteElectronicaTCIXml();
            string XML_Cabecera = CPTCI_XML.f_ENComprobante_LOG(response);

            if (XML_Cabecera != "")
            {
                string Metodo;
                Metodo = "";
                Metodo = Metodo + "<?xml version= " + "\"1.0\"" + " encoding = " + "\"utf-8\"" + "?> " + secuencia;
                Metodo = Metodo + "<soap:Envelope xmlns:xsi=" + "\"http://www.w3.org/2001/XMLSchema-instance\"" + " xmlns:xsd=" + "\"http://www.w3.org/2001/XMLSchema\"" + " ";
                Metodo = Metodo + " xmlns:soap=" + "\"http://schemas.xmlsoap.org/soap/envelope/\"" + " > " + secuencia +
                        "<soap:Body>" + secuencia +
                            "<Registrar xmlns=" + "\"http://tempuri.org/\"" + ">" + secuencia +
                                "<oGeneral>" + secuencia + XML_Cabecera + "</oGeneral>" + secuencia +
                                "<oTipoComprobante>" + wTipoComp_TCI?.Trim() + "</oTipoComprobante>" + secuencia +
                                "<Cadena></Cadena>" + secuencia +
                                "<TipoCodigo>" + pTipoCodigo_BarraHash + "</TipoCodigo>" + secuencia +
                                "<CogigoBarras></CogigoBarras>" + secuencia +
                                "<CogigoHash></CogigoHash>" + secuencia +
                                "<Otorgar>" + pTipoOtorgamiento.ToString() + "</Otorgar>" + secuencia +
                                "<IdComprobanteCliente>0</IdComprobanteCliente>" + secuencia +
                            "</Registrar>" + secuencia +
                        "</soap:Body>" + secuencia +
                    "</soap:Envelope>";
                pXML = Metodo;
            }
            else
            {
                pXML = "";
                return -3; // 'Error al construir XML
            }

            return 0;

        }

        private Boolean ComprobanteElectronico_Update_transac(string campo, string nuevovalor, string xml, byte[] codigobarrajpg, string codigo, SqlConnection conn, SqlTransaction transaction)
        {

            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_COMPROBANTE_ELECTRONICO_UPDATE, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@campo", campo));
                    cmd.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));
                    cmd.Parameters.Add(new SqlParameter("@xml", xml));
                    cmd.Parameters.Add(new SqlParameter("@codigobarrajpg", codigobarrajpg));
                    cmd.Parameters.Add(new SqlParameter("@codigo", codigo));

                    var resultado = cmd.ExecuteNonQuery();
                    if (resultado >= -1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
