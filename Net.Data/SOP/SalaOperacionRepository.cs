using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Data;
using Net.CrossCotting;
using System.Net.Http;

namespace Net.Data
{
    public class SalaOperacionRepository : RepositoryBase<BE_SalaOperacion>, ISalaOperacionRepository
    {
        private readonly string _cnx;
        private readonly string _cnx_clinica;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaSalaOperacionPorFiltro";
        const string SP_GET_ROL = DB_ESQUEMA + "Sp_RolSalaOperaciones3_Consulta";
        const string SP_INSERT = DB_ESQUEMA + "VEN_SalaOperacionXmlIns";
        const string SP_UPDATE = DB_ESQUEMA + "VEN_SalaOperacionXmlUpd";
        const string SP_UPDATE_ROL = DB_ESQUEMA + "VEN_SalaOperacionUpd";
        const string SP_DELETE = DB_ESQUEMA + "VEN_SalaOperacionDel";
        const string SP_ESTADO = DB_ESQUEMA + "VEN_SalaOperacionEstadoUpd";
        const string SP_GET_DETALLE = DB_ESQUEMA + "VEN_ListaSalaOperacionDetallePorId";
        const string SP_GET_POR_ID = DB_ESQUEMA + "VEN_SalaOperacionPorId";
        const string SP_GET_POR_DETALLE_ID = DB_ESQUEMA + "VEN_SalaOperacionDetallePorId";
        const string SP_GET_POR_DETALLE_LOTE_ID = DB_ESQUEMA + "VEN_SalaOperacionDetalleLotePorId";
        const string SP_GET_DETALLE_RESERVA = DB_ESQUEMA + "VEN_ListaDetalleReservaIdBorrador";
        const string SP_GET_DETALLE_LOTE_UBI = DB_ESQUEMA + "VEN_ListaSOPDetalleLotePorIdBorradorDetalleGet";

        public SalaOperacionRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnx_clinica = configuration.GetConnectionString("cnnSqlClinica");
        }
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> GetListSalaOperacionPorFiltro(FE_SalaOperacion value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            value.fechainicio = Utilidades.GetFechaHoraInicioActual(value.fechainicio);
            value.fechafin = Utilidades.GetFechaHoraFinActual(value.fechafin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", value.fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", value.fechafin));

                        var response = new List<BE_SalaOperacion>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_SalaOperacion>)context.ConvertTo<BE_SalaOperacion>(reader);
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
        public async Task<ResultadoTransaccion<BE_SalaOperacionRol>> GetListRolSalaOperacionPorAtencion(FE_SalaOperacionAtencion value)
        {
            ResultadoTransaccion<BE_SalaOperacionRol> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacionRol>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx_clinica))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_ROL, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codatencion", value.codatencion));

                        var response = new List<BE_SalaOperacionRol>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_SalaOperacionRol>)context.ConvertTo<BE_SalaOperacionRol>(reader);
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
        public async Task<ResultadoTransaccion<BE_SalaOperacionDetalle>> GetListSalaOperacionDetallePorId(int idborrador)
        {
            ResultadoTransaccion<BE_SalaOperacionDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacionDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idborrador", idborrador));

                        var response = new List<BE_SalaOperacionDetalle>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_SalaOperacionDetalle>)context.ConvertTo<BE_SalaOperacionDetalle>(reader);
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

        public async Task<ResultadoTransaccion<BE_SalaOperacion>> GetSalaOperacionPorId(int idborrador)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_POR_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idborrador", idborrador));

                        var response = new BE_SalaOperacion();

                        List<BE_SalaOperacionDetalleLote> salaOperacionDetalleLotes = new List<BE_SalaOperacionDetalleLote>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (BE_SalaOperacion)context.Convert<BE_SalaOperacion>(reader);

                            ResultadoTransaccion<BE_SalaOperacionDetalle> resultadoTransaccionDetalle = await GetSalaOperacionDetallePorId(idborrador);

                            if (resultadoTransaccionDetalle.ResultadoCodigo == -1)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetalle.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            response.SalaOperacionDetalle = (List<BE_SalaOperacionDetalle>)resultadoTransaccionDetalle.dataList;

                            ResultadoTransaccion<BE_SalaOperacionDetalleLote> resultadoTransaccionDetalleLote = await GetSalaOperacionDetalleLotePorId(idborrador);

                            if (resultadoTransaccionDetalleLote.ResultadoCodigo == -1)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetalleLote.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            salaOperacionDetalleLotes = (List<BE_SalaOperacionDetalleLote>)resultadoTransaccionDetalleLote.dataList;

                            foreach (BE_SalaOperacionDetalle item in response.SalaOperacionDetalle)
                            {
                                var lotePorDetalle = salaOperacionDetalleLotes.FindAll(xFila => xFila.idborradordetalle == item.idborradordetalle);

                                response.SalaOperacionDetalle.Find(xFila => xFila.idborradordetalle == item.idborradordetalle).listaSalaOperacionDetalleLote = lotePorDetalle;
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

        public async Task<ResultadoTransaccion<BE_SalaOperacionDetalle>> GetSalaOperacionDetallePorId(int idborrador)
        {
            ResultadoTransaccion<BE_SalaOperacionDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacionDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_POR_DETALLE_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idborrador", idborrador));

                        var response = new List<BE_SalaOperacionDetalle>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_SalaOperacionDetalle>)context.ConvertTo<BE_SalaOperacionDetalle>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
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

        public async Task<ResultadoTransaccion<BE_SalaOperacionDetalleLote>> GetSalaOperacionDetalleLotePorId(int idborrador)
        {
            ResultadoTransaccion<BE_SalaOperacionDetalleLote> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacionDetalleLote>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_POR_DETALLE_LOTE_ID, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idborrador", idborrador));

                        var response = new List<BE_SalaOperacionDetalleLote>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_SalaOperacionDetalleLote>)context.ConvertTo<BE_SalaOperacionDetalleLote>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
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

        public async Task<ResultadoTransaccion<BE_StockLote>> GetListSalaOperacionDetalleLoteUbiPorId(int idborradordetalle)
        {
            ResultadoTransaccion<BE_StockLote> vResultadoTransaccion = new ResultadoTransaccion<BE_StockLote>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_LOTE_UBI, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idborradordetalle", idborradordetalle));

                        var response = new List<BE_StockLote>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_StockLote>)context.ConvertTo<BE_StockLote>(reader);
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
        public async Task<ResultadoTransaccion<SapReserveStockNew>> GetListDetalleReservaPorIdBorrador(SqlConnection conn, SqlTransaction transaction, int idborrador)
        {
            ResultadoTransaccion<SapReserveStockNew> vResultadoTransaccion = new ResultadoTransaccion<SapReserveStockNew>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_GET_DETALLE_RESERVA, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idborrador", idborrador));

                    var response = new List<SapReserveStockNew>();

                    //conn.Open();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        response = (List<SapReserveStockNew>)context.ConvertTo<SapReserveStockNew>(reader);
                    }

                    //conn.Close();

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
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> Registrar(BE_SalaOperacionXml value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter oParam = new SqlParameter("@idborrador", value.idborrador);
                        oParam.SqlDbType = SqlDbType.Int;
                        oParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(oParam);

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

                        value.idborrador = (int)cmd.Parameters["@idborrador"].Value;

                        vResultadoTransaccion.IdRegistro = (int)cmd.Parameters["@idborrador"].Value;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;

                        ResultadoTransaccion<SapReserveStockNew> resultadoTransaccionLista = await GetListDetalleReservaPorIdBorrador(conn, transaction, vResultadoTransaccion.IdRegistro);

                        if (resultadoTransaccionLista.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionLista.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        SapReserveStockRepository sapReserve = new SapReserveStockRepository(_clientFactory, _configuration, context);

                        foreach (SapReserveStockNew item in resultadoTransaccionLista.dataList)
                        {
                            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoSapDocument = await sapReserve.SetCreateReserve(item);

                            if (resultadoSapDocument.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
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
                conn.Close();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> Modificar(BE_SalaOperacionXml value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

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

                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = value.idborrador;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;

                        // Eliminamos la reserva, si es de sala de operación
                        SapReserveStockRepository sapReserveStockRepository = new SapReserveStockRepository(_clientFactory, _configuration, context);
                        string u_idexterno = string.Format("SOP-{0}", value.idborrador);
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

                        // Obtenemos la informacion para realizar la reserva
                        ResultadoTransaccion<SapReserveStockNew> resultadoTransaccionLista = await GetListDetalleReservaPorIdBorrador(conn, transaction, vResultadoTransaccion.IdRegistro);

                        if (resultadoTransaccionLista.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionLista.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        // Realizamos la reserva de los productos actualizados.
                        SapReserveStockRepository sapReserve = new SapReserveStockRepository(_clientFactory, _configuration, context);

                        foreach (SapReserveStockNew item in resultadoTransaccionLista.dataList)
                        {
                            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> resultadoSapDocument = await sapReserve.SetCreateReserve(item);

                            if (resultadoSapDocument.ResultadoCodigo == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocument.ResultadoDescripcion;
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

                conn.Close();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> ModificarRol(BE_SalaOperacionXml value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE_ROL, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

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
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> Eliminar(FE_SalaOperacionId value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idborrador", value.idborrador));
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

                        vResultadoTransaccion.IdRegistro = value.idborrador;
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
        public async Task<ResultadoTransaccion<BE_SalaOperacion>> Estado(FE_SalaOperacionId value)
        {
            ResultadoTransaccion<BE_SalaOperacion> vResultadoTransaccion = new ResultadoTransaccion<BE_SalaOperacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_ESTADO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idborrador", value.idborrador));
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

                        vResultadoTransaccion.IdRegistro = value.idborrador;
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
