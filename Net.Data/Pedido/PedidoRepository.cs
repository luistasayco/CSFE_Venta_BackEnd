﻿using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;
using System.Net.Http;
using System.Data;

namespace Net.Data
{
    public class PedidoRepository : RepositoryBase<BE_Pedido>, IPedidoRepository
    {
        private readonly string _cnx;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_DELETE_MENSAJE = "VEN_DevolucionSRV_ProductosMensajeDel";
        const string SP_GET_PEDIDOS_SEGUIMIENTO_POR_FILTRO = DB_ESQUEMA + "VEN_ListaSeguimientoPedidosPorFiltrosGet";
        const string SP_GET_PEDIDOS_POR_ATENCION = DB_ESQUEMA + "VEN_ListaPedidosPorAtencionGet";
        const string SP_GET_PEDIDODETALLE_POR_PEDIDO = DB_ESQUEMA + "VEN_ListaPedidoDetallePorPedidoGet";
        const string SP_GET_PEDIDO_POR_FILTRO = DB_ESQUEMA + "VEN_ListaPedidosPorFiltrosGet";
        const string SP_GET_PEDIDO_SIN_VENTA_POR_FILTRO = DB_ESQUEMA + "VEN_ListaPedidosSinVentaPorFiltrosGet";
        const string SP_GET_DATOS_PEDIDO_POR_PEDIDO = DB_ESQUEMA + "VEN_DatosPedidoPorPedidoGet";
        const string SP_GET_PEDIDO_MENSAJE_POR_PEDIDO = DB_ESQUEMA + "VEN_DevolucionSRV_ProductosMensajeGet";
        const string SP_GET_LIST_PEDIDOS_VENTA_AUTOMATICA = DB_ESQUEMA + "VEN_ListaPedidoVentaAutomaticaGet";

        // Devoluciones
        const string SP_GET_LIST_PEDIDOS_VENTA_DEVOLUCION = DB_ESQUEMA + "VEN_DevolucionSRV_Productos";

        public PedidoRepository(IConnectionSQL context, IConfiguration configuration, IHttpClientFactory clientFactory)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _clientFactory = clientFactory;
            _configuration = configuration;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListaPedidosSeguimientoPorFiltro(DateTime fechainicio, DateTime fechafin, string ccosto, int opcion)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDOS_SEGUIMIENTO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechaini", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));
                        cmd.Parameters.Add(new SqlParameter("@ccosto", ccosto));
                        cmd.Parameters.Add(new SqlParameter("@opcion", opcion));

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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosPorAtencion(string codatencion, string codtercero)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDOS_POR_ATENCION, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));
                        cmd.Parameters.Add(new SqlParameter("@codtercero", codtercero));

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

        public async Task<ResultadoTransaccion<BE_PedidoDetalle>> GetListPedidoDetallePorPedido(string codpedido)
        {
            ResultadoTransaccion<BE_PedidoDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_PedidoDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_PedidoDetalle>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDODETALLE_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_PedidoDetalle>)context.ConvertTo<BE_PedidoDetalle>(reader);
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
        public async Task<ResultadoTransaccion<BE_PedidoDevolucion>> GetListPedidoDevolucionPorPedido(string codpedido)
        {
            ResultadoTransaccion<BE_PedidoDevolucion> vResultadoTransaccion = new ResultadoTransaccion<BE_PedidoDevolucion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_PedidoDevolucion>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PEDIDOS_VENTA_DEVOLUCION, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@as_codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_PedidoDevolucion>)context.ConvertTo<BE_PedidoDevolucion>(reader);
                        }

                        StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);

                        List<BE_PedidoDevolucion> pedidoDevolucions = new List<BE_PedidoDevolucion>();

                        foreach (BE_PedidoDevolucion item in response)
                        {
                            var data = await stockRepository.GetListStockPorFiltro(item.codalmacen, string.Empty, item.codproducto, true);

                            List<BE_Stock> listProducto = (List<BE_Stock>)data.dataList;

                            if (listProducto.Count > 0)
                            {

                                response.Find(xFila => xFila.codproducto == item.codproducto).stockalmacen = (decimal)listProducto[0].OnHandALM;
                                response.Find(xFila => xFila.codproducto == item.codproducto).manbtchnum = listProducto[0].ManBtchNum.Equals("Y") ? true : false;
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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosSinPedidoPorFiltro(DateTime fechainicio, DateTime fechafin, string codtipopedido, string codpedido)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDO_SIN_VENTA_POR_FILTRO, conn))
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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosPorFiltro(DateTime fechainicio, DateTime fechafin, string codtipopedido, string codpedido)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDO_POR_FILTRO, conn))
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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetDatosPedidoPorPedido(string codpedido)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DATOS_PEDIDO_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

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
        public async Task<ResultadoTransaccion<BE_PedidoDetalle_Mensaje>> GetPedidoMensajePorPedido(string codpedido)
        {
            ResultadoTransaccion<BE_PedidoDetalle_Mensaje> vResultadoTransaccion = new ResultadoTransaccion<BE_PedidoDetalle_Mensaje>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_PedidoDetalle_Mensaje>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDO_MENSAJE_POR_PEDIDO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@as_codpedido", codpedido));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_PedidoDetalle_Mensaje>)context.ConvertTo<BE_PedidoDetalle_Mensaje>(reader);
                        }

                        conn.Close();

                        //string mensaje = string.Empty;
                        //string mensajeFinal = string.Empty;

                        //foreach (BE_PedidoDetalle_Mensaje item in response)
                        //{
                        //    mensaje += string.Format("{0} - {1}", item.codproducto, item.nombreproducto);
                        //}

                        //if (response.Count > 0)
                        //{
                        //    mensajeFinal = string.Format("La cantidad a devolver de los siguientes productos excede lo facturado: <br> {0} <br> genere la devolución de estos productos uno a uno", mensaje);
                        //}

                        // Procedemos a eliminar
                        ResultadoTransaccion<string> resultadoTransaccionDelete = await EliminarPedidoMensajePorPedido(codpedido);

                        if (resultadoTransaccionDelete.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDelete.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetDatosPedidoPorPedido(SqlConnection conn, string codpedido)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                //using (SqlConnection conn = new SqlConnection(_cnx))
                //{
                var response = new List<BE_Pedido>();
                using (SqlCommand cmd = new SqlCommand(SP_GET_DATOS_PEDIDO_POR_PEDIDO, conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

                    //conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<BE_Pedido>)context.ConvertTo<BE_Pedido>(reader);
                    }

                    //conn.Close();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                    vResultadoTransaccion.dataList = response;
                }
                //}
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListPedidosPorPedido(string codpedido)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_PEDIDO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));

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

        public async Task<ResultadoTransaccion<BE_Pedido>> GetListaPedidoVentaAutomatica(string codpedido)
        {
            ResultadoTransaccion<BE_Pedido> vResultadoTransaccion = new ResultadoTransaccion<BE_Pedido>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Pedido>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LIST_PEDIDOS_VENTA_AUTOMATICA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));
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

        public async Task<ResultadoTransaccion<string>> EliminarPedidoMensajePorPedido(string codpedido)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_DELETE_MENSAJE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@as_codpedido", codpedido));

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
    }
}
