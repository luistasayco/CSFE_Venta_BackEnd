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
using Net.CrossCotting;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Net.Http;

namespace Net.Data
{
    public class ConsolidadoRepository : RepositoryBase<BE_Consolidado>, IConsolidadoRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_CONSOLIDADO_POR_FILTRO = DB_ESQUEMA + "VEN_ConsolidadosPorFiltrosGet";
        const string SP_GET_CONSOLIDADO_CERRADOS_POR_FILTRO = DB_ESQUEMA + "VEN_ConsolidadosCerradosPorFiltrosGet";
        const string SP_GET_CONSOLIDADO_POR_PICKING = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingGet";
        const string SP_GET_CONSOLIDADO_INDIVIDUAL_POR_PICKING = DB_ESQUEMA + "VEN_ConsolidadosIndividualPedidoPickingGet";
        const string SP_GET_CONSOLIDADO_POR_PICKING_POR_PEDIDO = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingPorPedidoGet";
        const string SP_GET_CONSOLIDADO_INDIVIDUAL_POR_PICKING_POR_PEDIDO = DB_ESQUEMA + "VEN_ConsolidadosIndividualPedidoPickingPorPedidoGet";
        const string SP_GET_CONSOLIDADO_PICKING_POR_ID = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingPorIdConsolidadoGet";
        const string SP_GET_DETALLE_CONSOLIDADO_POR_ID = DB_ESQUEMA + "VEN_ConsolidadosPedidoPorIdGet";
        const string SP_GET_DETALLE_CONSOLIDADO_PICKING_POR_ID = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingPorIdGet";

        const string SP_INSERT_CONSOLIDADO_PICKING = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingIns";
        const string SP_UPDATE_CONSOLIDADO_PICKING = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingUpd";
        const string SP_UPDATE_CONSOLIDADO_PICKING_ESTADO = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingEstadoUpd";
        const string SP_DELETE_CONSOLIDADO_PICKING = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingDel";
        const string SP_GET_CONSOLIDADO_SOLICITUD = DB_ESQUEMA + "REQ_ConsolidadoSolicitudGet";
        const string SP_UPDATE_ID_RESERVA = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingIdReservaUpd";
        public ConsolidadoRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoPorFiltro(DateTime fechainicio, DateTime fechafin)
        {
            ResultadoTransaccion<BE_Consolidado> vResultadoTransaccion = new ResultadoTransaccion<BE_Consolidado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fechainicio = Utilidades.GetFechaHoraInicioActual(fechainicio);
            fechafin = Utilidades.GetFechaHoraFinActual(fechafin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Consolidado>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Consolidado>)context.ConvertTo<BE_Consolidado>(reader);
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

        public async Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoCerradoPorFiltro(DateTime fechainicio, DateTime fechafin)
        {
            ResultadoTransaccion<BE_Consolidado> vResultadoTransaccion = new ResultadoTransaccion<BE_Consolidado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fechainicio = Utilidades.GetFechaHoraInicioActual(fechainicio);
            fechafin = Utilidades.GetFechaHoraFinActual(fechafin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Consolidado>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_CERRADOS_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Consolidado>)context.ConvertTo<BE_Consolidado>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPickingPorId(int idconsolidado)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_PICKING_POR_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPicking(string codpedido, string codproducto)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_POR_PICKING, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoIndividualPicking(string codpedido, string codproducto)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_INDIVIDUAL_POR_PICKING, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPorPedidoPicking(string codpedido)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_POR_PICKING_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoIndividualPorPedidoPicking(string codpedido)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_INDIVIDUAL_POR_PICKING_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedido>> GetListDetalleConsolidado(int idconsolidado)
        {
            ResultadoTransaccion<BE_ConsolidadoPedido> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedido>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_CONSOLIDADO_POR_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedido>)context.ConvertTo<BE_ConsolidadoPedido>(reader);
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetConsolidadoPedidoPickingPorId(int idconsolidadopicking)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new BE_ConsolidadoPedidoPicking();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_CONSOLIDADO_PICKING_POR_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidadopicking", idconsolidadopicking));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_ConsolidadoPedidoPicking>(reader);

                            if (response == null)
                            {
                                response = new BE_ConsolidadoPedidoPicking();
                            }
                        }

                        conn.Close();
                    }

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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> RegistrarConsolidadoPicking(BE_ConsolidadoPedidoPicking item)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SP_INSERT_CONSOLIDADO_PICKING, conn, transaction))
                        {
                            cmd.Parameters.Clear();

                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            SqlParameter oParam = new SqlParameter("@idconsolidadopicking", item.idconsolidadopicking);
                            oParam.SqlDbType = SqlDbType.Int;
                            oParam.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(oParam);
                            cmd.Parameters.Add(new SqlParameter("@idconsolidado", item.idconsolidado));
                            cmd.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
                            cmd.Parameters.Add(new SqlParameter("@codproducto", item.codproducto));
                            cmd.Parameters.Add(new SqlParameter("@cantidad", item.cantidad));
                            cmd.Parameters.Add(new SqlParameter("@cantidadpicking", item.cantidadpicking));
                            cmd.Parameters.Add(new SqlParameter("@lote", item.lote));
                            cmd.Parameters.Add(new SqlParameter("@fechavencimiento", item.fechavencimiento));
                            cmd.Parameters.Add(new SqlParameter("@codalmacen", item.codalmacen));
                            cmd.Parameters.Add(new SqlParameter("@ubicacion", item.ubicacion));
                            cmd.Parameters.Add(new SqlParameter("@codusuarioapu", item.codusuarioapu));
                            cmd.Parameters.Add(new SqlParameter("@estado", item.estado));
                            cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", item.RegIdUsuario));

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

                            item.idconsolidadopicking = (int)cmd.Parameters["@idconsolidadopicking"].Value;

                            vResultadoTransaccion.IdRegistro = item.idconsolidadopicking;
                            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                            vResultadoTransaccion.data = item;

                            // Realiza la reservacion de Articulo
                            SapReserveStockRepository sapReserve = new SapReserveStockRepository(_clientFactory, _configuration, context);

                            var valueReserva = new SapReserveStockNew
                            {
                                Name = string.Format("APU-RESERVA-CONSO-{0}", item.codusuarioapu),
                                U_ITEMCODE = item.codproducto,
                                U_BINABSENTRY = item.ubicacion == null ? 0 : (int)item.ubicacion,
                                U_QUANTITY = item.cantidad,
                                U_IDEXTERNO = string.Format("APU-{0}", item.codusuarioapu),
                                U_BATCHNUM = item.lote,
                                U_WHSCODE = item.codalmacen
                            };

                            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoSapDocument = await sapReserve.SetCreateReserve(valueReserva);

                            if (resultadoSapDocument.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            item.idreserva = resultadoSapDocument.data.code;

                            // Actualizamos ID de reserva
                            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionUpdate = await ActualizarIdReserva(conn, transaction, item.idconsolidadopicking, item.idreserva, (int)item.RegIdUsuario);

                            if (resultadoTransaccionUpdate.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionUpdate.ResultadoDescripcion;
                                return vResultadoTransaccion;
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ModificarConsolidadoPicking(BE_ConsolidadoPedidoPicking item)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                // Obtenermos informacion de picking
                ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionPicking = await GetConsolidadoPedidoPickingPorId(item.idconsolidadopicking);

                if (resultadoTransaccionPicking.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPicking.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CONSOLIDADO_PICKING, conn, transaction))
                        {
                            cmd.Parameters.Clear();

                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@idconsolidadopicking", item.idconsolidadopicking));
                            cmd.Parameters.Add(new SqlParameter("@cantidadpicking", item.cantidadpicking));
                            cmd.Parameters.Add(new SqlParameter("@codusuarioapu", item.codusuarioapu));
                            cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", item.RegIdUsuario));

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

                            var dataUpdate = new SapReserveStockUpdate
                            {
                                U_QUANTITY = item.cantidadpicking
                            };

                            // Actualizar la reserva, si es de sala de operación
                            SapReserveStockRepository sapReserveStockRepository = new SapReserveStockRepository(_clientFactory, _configuration, context);
                            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoTransaccionReservaDelete = await sapReserveStockRepository.SetUpdateReserve(resultadoTransaccionPicking.data.idreserva, dataUpdate);

                            if (resultadoTransaccionReservaDelete.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReservaDelete.ResultadoDescripcion;
                                return vResultadoTransaccion;
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> EliminarConsolidadoPicking(BE_ConsolidadoPedidoPicking value)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            // Obtenermos informacion de picking a eliminar
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionPicking = await GetConsolidadoPedidoPickingPorId(value.idconsolidadopicking);

            if (resultadoTransaccionPicking.ResultadoCodigo == -1)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPicking.ResultadoDescripcion;
                return vResultadoTransaccion;
            }

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE_CONSOLIDADO_PICKING, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idconsolidadopicking", value.idconsolidadopicking));
                        cmd.Parameters.Add(new SqlParameter("@codusuarioapu", value.codusuarioapu));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(value.idconsolidadopicking.ToString());
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;

                        // Eliminamos la reserva, si es de sala de operación
                        SapReserveStockRepository sapReserveStockRepository = new SapReserveStockRepository(_clientFactory, _configuration, context);
                        ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoTransaccionReservaDelete = await sapReserveStockRepository.SetDeleteReserve(resultadoTransaccionPicking.data.idreserva);

                        if (resultadoTransaccionReservaDelete.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionReservaDelete.ResultadoDescripcion;
                            return vResultadoTransaccion;
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

                conn.Close();

            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ModificarEstadoPedido(BE_ConsolidadoPedidoPicking item)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE_CONSOLIDADO_PICKING_ESTADO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", item.idconsolidado));
                        cmd.Parameters.Add(new SqlParameter("@estado", item.estado));
                        cmd.Parameters.Add(new SqlParameter("@codusuarioapu", item.codusuarioapu));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", item.RegIdUsuario));

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

                        vResultadoTransaccion.IdRegistro = int.Parse(item.idconsolidado.ToString());
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> ActualizarIdReserva(SqlConnection conn, SqlTransaction transaction, int idconsolidadopicking, int idreserva, int RegIdUsuario)
        {
            ResultadoTransaccion<BE_ConsolidadoPedidoPicking> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_UPDATE_ID_RESERVA, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@idconsolidadopicking", idconsolidadopicking));
                    cmd.Parameters.Add(new SqlParameter("@idreserva", idreserva));
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

                    await cmd.ExecuteNonQueryAsync();

                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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
        public async Task<ResultadoTransaccion<BE_ConsolidadoSolicitud>> GetListConsolidadoSolicitudGet(DateTime fechaInicio, DateTime fechaFin)
        {
            ResultadoTransaccion<BE_ConsolidadoSolicitud> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoSolicitud>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoSolicitud>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADO_SOLICITUD, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@FechaInicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@FechaFin", fechaFin));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoSolicitud>)context.ConvertTo<BE_ConsolidadoSolicitud>(reader);
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
        public async Task<ResultadoTransaccion<MemoryStream>> GenerarConsolidadoSolicitudPrint(DateTime fechaInicio, DateTime fechaFin)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            fechaFin = Utilidades.GetFechaHoraFinActual(fechaFin);

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
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 8f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                ResultadoTransaccion<BE_ConsolidadoSolicitud> resultadoTransaccion = await GetListConsolidadoSolicitudGet(fechaInicio, fechaFin);

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                List<BE_ConsolidadoSolicitud> response = (List<BE_ConsolidadoSolicitud>)resultadoTransaccion.dataList;

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

                var title = string.Format("REPORTE DE SOLICITUD", titulo);

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

                // Generamos la Cabecera

                //Cabecera

                tbl = new PdfPTable(new float[] { 20f, 80f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Almacén", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].DesAlmacenDestino), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle

                tbl = new PdfPTable(new float[] { 5f, 10f, 30f, 15f, 10f, 10, 10f, 10f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("ID", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Código", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Descripción", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Laboratorio", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Grupo", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cantidad", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Lote", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cant.Lote", parrafoNegro));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                int ID = 0;

                foreach (BE_ConsolidadoSolicitud item in response)
                {
                    ID++;

                    c1 = new PdfPCell(new Phrase(ID.ToString("N0"), parrafoNegro)) { Border = 0, VerticalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.CodArticulo, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.DesArticulo, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.CodLaboratorio, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.GroupName, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.CantidadProducto.ToString("N2"), parrafoNegro)) { Border = 0, VerticalAlignment = Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.NumLote, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.CantidadLote.ToString("N"), parrafoNegro)) { Border = 0, VerticalAlignment = Element.ALIGN_RIGHT };
                    tbl.AddCell(c1);
                }
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente el consolidado de la solicitudad";
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
    }
}
