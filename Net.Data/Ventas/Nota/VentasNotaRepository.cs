using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.CrossCotting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class VentasNotaRepository : RepositoryBase<BE_VentasNota>, IVentasNotaRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_NotaPorFiltroGet";
        const string SP_GET_CABECERA = DB_ESQUEMA + "VEN_ListaNotaCreditoCabeceraPorFiltrosGet";
        const string SP_GET_CABECERA_POR_CODNOTA = DB_ESQUEMA + "VEN_ListaNotaCreditoCabeceraPorCodNotaGet";

        const string SP_SET_NOTA_ELIMINAR = DB_ESQUEMA + "VEN_NotasDel";

        public VentasNotaRepository(IConnectionSQL context, IConfiguration configuration, IHttpClientFactory clientFactory)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_VentasNota>> GetNotaPorFiltro(string buscar, int key, int numerolineas, int orden, string tipo)
        {
            ResultadoTransaccion<BE_VentasNota> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasNota>();
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
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        var response = new BE_VentasNota();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_VentasNota>(reader);

                            if (response == null)
                            {
                                response = new BE_VentasNota();
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

        public async Task<ResultadoTransaccion<BE_VentasNota>> GetNotaCabeceraPorFiltro(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin, string codnota, string anombredequien, string codtipocliente)
        {
            ResultadoTransaccion<BE_VentasNota> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasNota>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fecinicio = Utilidades.GetFechaHoraInicioActual(fecinicio);
            fecfin = Utilidades.GetFechaHoraFinActual(fecfin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@codventa", codventa));
                        cmd.Parameters.Add(new SqlParameter("@fecinicio", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fecfin", fecfin));
                        cmd.Parameters.Add(new SqlParameter("@codnota", codnota));
                        cmd.Parameters.Add(new SqlParameter("@anombredequien", anombredequien));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente));

                        var response = new List<BE_VentasNota>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasNota>)context.ConvertTo<BE_VentasNota>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 0);
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
        public async Task<ResultadoTransaccion<BE_VentasNota>> GetNotaCabeceraPorCodNota(string codnota)
        {
            ResultadoTransaccion<BE_VentasNota> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasNota>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_POR_CODNOTA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codnota", codnota));

                        var response = new List<BE_VentasNota>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_VentasNota>)context.ConvertTo<BE_VentasNota>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 0);
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

        public async Task<ResultadoTransaccion<BE_VentasNota>> EliminarNotaCredito(string codnota, string usuario, int idusuario)
        {
            ResultadoTransaccion<BE_VentasNota> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasNota>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";

                ComprobanteRepository comprobanteRepository = new ComprobanteRepository(context, _configuration, _clientFactory);

                ResultadoTransaccion<BE_CuadreCaja> resultadoTransaccionCuadreCaja = await comprobanteRepository.GetListaCuadreCaja(codnota);

                if (resultadoTransaccionCuadreCaja.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCuadreCaja.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }
                BE_CuadreCaja bE_CuadreCaja = ((List<BE_CuadreCaja>)resultadoTransaccionCuadreCaja.dataList)[0];

                ResultadoTransaccion<BE_VentasNota> resultadoTransaccionNotaCredito = await GetNotaCabeceraPorCodNota(codnota);
                if (resultadoTransaccionNotaCredito.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionNotaCredito.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }
                BE_VentasNota bE_VentasNota = ((List<BE_VentasNota>)resultadoTransaccionNotaCredito.dataList)[0];

                if (bE_VentasNota.estado == "X")
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Nota de crédito {0} se encuentra ANULADO", bE_VentasNota.codnota);
                    return vResultadoTransaccion;
                }

                if (!string.IsNullOrEmpty(bE_VentasNota.numeroplanilla))
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("La Nota {0} esta incluida en la planilla", bE_VentasNota.codnota);
                    return vResultadoTransaccion;
                }

                VentaCajaRepository ventaCajaRepository = new VentaCajaRepository(_clientFactory, context, _configuration);

                ResultadoTransaccion<BE_ComprobanteElectronicoPrint> resultadoTransaccionComprobanteNota = await ventaCajaRepository.GetComprobanteElectroncioVB("001", codnota, "", "L", "", 5);

                if (resultadoTransaccionComprobanteNota.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionComprobanteNota.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionComprobanteNota.data.flg_electronico == "1")
                {
                    if (resultadoTransaccionComprobanteNota.data.anular == "S")
                    {

                    } else
                    {
                        var mensaje = string.Format (" No puede anular este comprobante <br> {0} <br> Regla: Tener CDR-Rechazado, Tener envío a baja aceptado.", resultadoTransaccionComprobanteNota.data.mensaje);
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = mensaje;
                        return vResultadoTransaccion;
                    }
                } else
                {
                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Ud. Eliminará un Comprobante físico";
                }

                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        ResultadoTransaccion<bool> resultadoTransaccionEliminarNotaCredito = await EliminarNotaCredito(conn, transaction, codnota, usuario, idusuario);
                        if (resultadoTransaccionEliminarNotaCredito.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionEliminarNotaCredito.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        SapDocumentsRepository sapDocuments = new SapDocumentsRepository(_clientFactory, _configuration, context);
                        ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocumentCancelNota = await sapDocuments.SetCancelDocumentNotaPago(bE_CuadreCaja.ide_trans);

                        if (resultadoSapDocumentCancelNota.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocumentCancelNota.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        ResultadoTransaccion<SapBaseResponse<SapDocument>> resultadoSapDocumentCancelNotaPago = await sapDocuments.SetCancelDocumentNota(bE_VentasNota.doc_entry);

                        if (resultadoSapDocumentCancelNotaPago.ResultadoCodigo == -1)
                        {
                            transaction.Rollback();
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoSapDocumentCancelNotaPago.ResultadoDescripcion;
                            return vResultadoTransaccion;
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

        public async Task<ResultadoTransaccion<bool>> EliminarNotaCredito(SqlConnection conn, SqlTransaction transaction, string codnota, string usuario, int idusuario)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlCommand cmdActualizarPresotor = new SqlCommand(SP_SET_NOTA_ELIMINAR, conn, transaction))
                {
                    cmdActualizarPresotor.Parameters.Clear();
                    cmdActualizarPresotor.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@codnota", codnota));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@usuario", usuario));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@RegIdUsuario", idusuario));

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
    }
}
