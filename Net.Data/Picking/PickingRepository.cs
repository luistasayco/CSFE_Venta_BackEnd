using Microsoft.Data.SqlClient;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Transactions;
using System.Net.Http;

namespace Net.Data
{
    public class PickingRepository : RepositoryBase<BE_Picking>, IPickingRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_PickingPorFiltroGet";
        const string SP_GET_POR_ID = DB_ESQUEMA + "VEN_PickingPorIdGet";
        const string SP_INSERT = DB_ESQUEMA + "VEN_PickingIns";
        const string SP_UPDATE = DB_ESQUEMA + "VEN_PickingEstadoUpd";
        const string SP_DELETE = DB_ESQUEMA + "VEN_PickingDel";
        const string SP_UPDATE_ID_RESERVA = DB_ESQUEMA + "VEN_PickingIdReservaUpd";

        public PickingRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_Picking>> GetListPickingPorPedido(string codpedido)
        {

            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
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
                        cmd.Parameters.Add(new SqlParameter("@codpedido", codpedido));
                        cmd.Parameters.Add(new SqlParameter("@opcion", 1));

                        var response = new List<BE_Picking>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Picking>)context.ConvertTo<BE_Picking>(reader);
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
        public async Task<ResultadoTransaccion<BE_Picking>> GetListPickingPorReceta(int id_receta)
        {

            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
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
                        cmd.Parameters.Add(new SqlParameter("@id_receta", id_receta));
                        cmd.Parameters.Add(new SqlParameter("@opcion", 2));

                        var response = new List<BE_Picking>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Picking>)context.ConvertTo<BE_Picking>(reader);
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
        public async Task<ResultadoTransaccion<BE_Picking>> GetPickingPorId(int idpicking)
        {

            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
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
                        cmd.Parameters.Add(new SqlParameter("@idpicking", idpicking));

                        var response = new BE_Picking();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_Picking>(reader);

                            if (response == null)
                            {
                                response = new BE_Picking();
                            }
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 0);
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
        public async Task<ResultadoTransaccion<BE_Picking>> Registrar(BE_Picking item)
        {
            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
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
                        using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn, transaction))
                        {
                            cmd.Parameters.Clear();

                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            SqlParameter oParam = new SqlParameter("@idpicking", item.idpicking);
                            oParam.SqlDbType = SqlDbType.Int;
                            oParam.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(oParam);
                            cmd.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
                            cmd.Parameters.Add(new SqlParameter("@id_receta", item.id_receta));
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

                            item.idpicking = (int)cmd.Parameters["@idpicking"].Value;

                            vResultadoTransaccion.IdRegistro = item.idpicking;
                            vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                            vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                            vResultadoTransaccion.data = item;


                            // Realiza la reservacion de Articulo
                            SapReserveStockRepository sapReserve = new SapReserveStockRepository(_clientFactory, _configuration, context);

                            var valueReserva = new SapReserveStockNew
                            {
                                Name = string.Format("APU-RESERVA-ORDEN-{0}", item.codusuarioapu),
                                U_ITEMCODE = item.codproducto,
                                U_BINABSENTRY = item.ubicacion,
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
                            ResultadoTransaccion<BE_Picking> resultadoTransaccionUpdate = await ActualizarIdReserva(conn, transaction , item.idpicking, item.idreserva, (int)item.RegIdUsuario);

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
        public async Task<ResultadoTransaccion<BE_Picking>> Eliminar(BE_Picking value)
        {
            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            // Obtenermos informacion de picking a eliminar
            ResultadoTransaccion<BE_Picking> resultadoTransaccionPicking = await GetPickingPorId(value.idpicking);

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
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, conn, transaction))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idpicking", value.idpicking));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(value.idpicking.ToString());
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

        public async Task<ResultadoTransaccion<BE_Picking>> ActualizarIdReserva(SqlConnection conn, SqlTransaction transaction, int idpicking, int idreserva, int RegIdUsuario)
        {
            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_UPDATE_ID_RESERVA, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@idpicking", idpicking));
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
        public async Task<ResultadoTransaccion<BE_Picking>> ModificarEstadoPedido(BE_Picking item)
        {
            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
                        cmd.Parameters.Add(new SqlParameter("@id_receta", item.id_receta));
                        cmd.Parameters.Add(new SqlParameter("@estado", item.estado));
                        cmd.Parameters.Add(new SqlParameter("@codusuarioapu", item.codusuarioapu));
                        cmd.Parameters.Add(new SqlParameter("@opcion", 1));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(item.idpicking.ToString());
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
        public async Task<ResultadoTransaccion<BE_Picking>> ModificarEstadoReceta(BE_Picking item)
        {
            ResultadoTransaccion<BE_Picking> vResultadoTransaccion = new ResultadoTransaccion<BE_Picking>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
                        cmd.Parameters.Add(new SqlParameter("@id_receta", item.id_receta));
                        cmd.Parameters.Add(new SqlParameter("@estado", item.estado));
                        cmd.Parameters.Add(new SqlParameter("@codusuarioapu", item.codusuarioapu));
                        cmd.Parameters.Add(new SqlParameter("@opcion", 2));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(item.idpicking.ToString());
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
    }
}
