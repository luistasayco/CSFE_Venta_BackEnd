using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.Data;
using Net.CrossCotting;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Net.Data
{
    public class ConsolidadoPedidoRepository : RepositoryBase<BE_ConsolidadoPedidoDetalle>, IConsolidadoPedidoRepository
    {
        private readonly string _cnx;

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_PEDIDO_SIN_CONSOLIDAR_POR_FILTRO = DB_ESQUEMA + "VEN_ListaPedidosSinConsolidarPorFiltrosGet";
        const string SP_GET_CONSOLIDADOSPEDIDOCAB = DB_ESQUEMA + "VEN_ConsolidadosPedidoCabGetPorFiltros";
        const string SP_GET_CONSOLIDADOSPEDIDOPRODUCTO = DB_ESQUEMA + "VEN_ConsolidadosPedidoProductoGet";
        const string SP_SET_INSERT_CONSOLIDADOSPEDIDO = DB_ESQUEMA + "VEN_ConsolidadosPedido_Ins";
        const string SP_SET_UPDATE_CONSOLIDADOSPEDIDO = DB_ESQUEMA + "VEN_ConsolidadosPedido_Upd";
        const string SP_GET_CONSOLIDADOSPEDIDO = DB_ESQUEMA + "VEN_ConsolidadosPedidoGet";
        const string SP_GET_CONSOLIDADOSPEDIDOPICKING = DB_ESQUEMA + "VEN_ConsolidadosPedidoPickingPorIdConsolidadoGet";
        const string SP_GET_CONSOLIDADOSPEDIDO_POR_IDCONSOLIDADO = DB_ESQUEMA + "VEN_ConsolidadosPedidoPorIdConsolidadoGet";
        const string SP_SET_DELETE_CONSOLIDADOSPEDIDO_POR_PEDIDO = DB_ESQUEMA + "VEN_ConsolidadosPedidoPorPedido_Del";
        const string SP_SET_DELETE_CONSOLIDADOS = DB_ESQUEMA + "VEN_Consolidados_Del";
        const string SP_SET_UPDATE_CONSOLIDADOS = DB_ESQUEMA + "VEN_Consolidados_Upd";

        public ConsolidadoPedidoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosSinConsolidarPorFiltro(DateTime fechainicio, DateTime fechafin, string codtipopedido, string codpedido)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fechainicio = Utilidades.GetFechaHoraInicioActual(fechainicio);
            fechafin = Utilidades.GetFechaHoraFinActual(fechafin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDO_SIN_CONSOLIDAR_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));
                        cmd.Parameters.Add(new SqlParameter("@codtipopedido", codtipopedido == null ? string.Empty : codtipopedido));
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido == null ? string.Empty : codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Pedido>)context.ConvertTo<BE_Pedido>(reader);
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

        public async Task<ResultadoTransaccion<BE_Consolidado>> GetConsolidadoCabeceraPorIdConsolidado(int idconsolidado)
        {
            ResultadoTransaccion<BE_Consolidado> vResultadoTransaccion = new ResultadoTransaccion<BE_Consolidado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var lresponse = new List<BE_Consolidado>();
                    var response = new BE_Consolidado();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDOCAB, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            lresponse = (List<BE_Consolidado>)context.ConvertTo<BE_Consolidado>(reader);
                        }
                        conn.Close();

                    }
                    response = lresponse[0];

                    var responseDetalle = new List<BE_ConsolidadoPedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDO_POR_IDCONSOLIDADO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            //responseDetalle = (List<BE_ConsolidadoPedido>)context.ConvertTo<BE_ConsolidadoPedido>(reader);
                            while (await reader.ReadAsync())
                            {
                                responseDetalle.Add(new BE_ConsolidadoPedido
                                {
                                    codproducto = reader.GetString(0),
                                    desproducto = reader.GetString(1),
                                    cantidad = reader.GetDecimal(2)
                                });
                            }
                        }

                        conn.Close();
                    }

                    response.ListaConsolidadoPedido = responseDetalle;

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

        public async Task<ResultadoTransaccion<BE_Consolidado>> GetListConsolidadoCabecera(DateTime fechainicio, DateTime fechafin, int idconsolidado)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDOCAB, conn))
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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedido>> GetListConsolidadoPedido(int idconsolidado)
        {
            ResultadoTransaccion<BE_ConsolidadoPedido> vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var resultList = new List<BE_ConsolidadoPedido>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedido>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                resultList.Add(new BE_ConsolidadoPedido
                                {
                                    codpedido = reader.GetString(0),
                                    pedido = new BE_Pedido()
                                    {
                                        nomalmacen = reader.GetString(1),
                                        nompaciente = reader.GetString(2),
                                        codatencion = reader.GetString(3),
                                        tipopedido = reader.GetString(4)
                                    },
                                });
                            }

                            response = resultList;
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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoPicking>> GetListConsolidadoPedidoPickingPorIdConsolidado(int idconsolidado)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoPicking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var resultList = new List<BE_ConsolidadoPedidoPicking>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoPicking>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDOPICKING, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoPicking>)context.ConvertTo<BE_ConsolidadoPedidoPicking>(reader);
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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoProducto>> GetListConsolidadoPedidoProducto(int idconsolidado, string codpedido)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoProducto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var resultList = new List<BE_ConsolidadoPedidoProducto>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ConsolidadoPedidoProducto>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CONSOLIDADOSPEDIDOPRODUCTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", idconsolidado));
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConsolidadoPedidoProducto>)context.ConvertTo<BE_ConsolidadoPedidoProducto>(reader);

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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>> CreateConsolidadoPedido(BE_ConsolidadoPedidoXml value)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_SET_INSERT_CONSOLIDADOSPEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@XmlData", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@Opcion", value.Opcion));
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

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>> UpdateConsolidadoPedido(BE_ConsolidadoPedidoXml value)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedidoDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_CONSOLIDADOSPEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@XmlData", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", value.idconsolidado));
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

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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

        public async Task<ResultadoTransaccion<BE_ConsolidadoPedido>> DeleteConsolidadoPedidoPorPedido(BE_ConsolidadoPedido value)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_ConsolidadoPedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_SET_DELETE_CONSOLIDADOSPEDIDO_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", value.idconsolidado));
                        cmd.Parameters.Add(new SqlParameter("@codpedido", value.codpedido));
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

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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

        public async Task<ResultadoTransaccion<BE_Consolidado>> DeleteConsolidado(BE_Consolidado value)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_Consolidado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_SET_DELETE_CONSOLIDADOS, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", value.idconsolidado));
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

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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

        public async Task<ResultadoTransaccion<BE_Consolidado>> CloseConsolidado(BE_Consolidado value)
        {
            var vResultadoTransaccion = new ResultadoTransaccion<BE_Consolidado>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_SET_UPDATE_CONSOLIDADOS, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconsolidado", value.idconsolidado));
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

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
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

        public async Task<ResultadoTransaccion<MemoryStream>> GenerarConsolidadoPedidoPrint(int idconsolidado)
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

                var title = string.Format("CONSOLIDADO Nro {0}", idconsolidado, titulo);

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
                //Obtenemos los datos

                var resultadoTransaccion = await GetConsolidadoCabeceraPorIdConsolidado(idconsolidado);

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                // Generamos la Cabecera del vale de venta
                tbl = new PdfPTable(new float[] { 15f, 75f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Usuario Registro", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccion.data.usuario.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha Registro", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", resultadoTransaccion.data.fechahora.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle
                tbl = new PdfPTable(new float[] { 40f, 40f, 40f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Producto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nombre", parrafoNegroNegrita));
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
                //c1 = new PdfPCell(new Phrase("Lote", parrafoNegroNegrita));
                //c1.BorderWidth = 1;
                //c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                //c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                //c1.DisableBorderSide(Rectangle.TOP_BORDER);
                //tbl.AddCell(c1);
                //c1 = new PdfPCell(new Phrase("F. Vencimiento", parrafoNegroNegrita));
                //c1.BorderWidth = 1;
                //c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                //c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                //c1.DisableBorderSide(Rectangle.TOP_BORDER);
                //tbl.AddCell(c1);

                foreach (var item in resultadoTransaccion.data.ListaConsolidadoPedido)
                {

                        c1 = new PdfPCell(new Phrase(item.codproducto, parrafoNegro)) { Border = 0 };
                        tbl.AddCell(c1);
                        //c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                        //tbl.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(item.desproducto, parrafoNegro)) { Border = 0 };
                        tbl.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(item.cantidad.ToString(), parrafoNegro)) { Border = 0 };
                        tbl.AddCell(c1);
                        //c1 = new PdfPCell(new Phrase(((DateTime)itemLote.fechavencimiento).ToShortDateString(), parrafoNegro)) { Border = 0 };
                        //tbl.AddCell(c1);
                }
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Consolidado Pedido";
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
