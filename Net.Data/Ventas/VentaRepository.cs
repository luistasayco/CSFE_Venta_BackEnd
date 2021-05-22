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

namespace Net.Data
{
    public class VentaRepository : RepositoryBase<BE_VentasCabecera>, IVentaRepository
    {
        private readonly string _cnx;
        private readonly IConfiguration _configuration;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaVentasCabeceraPorFiltrosGet";
        const string SP_GET_CABECERA_VENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ObtieneVentasCabeceraPorCodVentaGet";
        const string SP_GET_DETALLEVENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ListaVentaDetallePorCodVentaGet";
        const string SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO = DB_ESQUEMA + "VEN_ListaVentasCabeceraPendientesPorFiltrosGet";

        const string SP_UPDATE_CABECERA_VENTA_ENVIO_PISO = DB_ESQUEMA + "VEN_VentaCabeceraEnvioPisoUpd";

        const string SP_UPDATE_INSERT = DB_ESQUEMA + "";

        public VentaRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _configuration = configuration;
            _aplicacionName = this.GetType().Name;
        }
        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetAll(string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
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

        public async Task<ResultadoTransaccion<BE_VentasCabecera>> GetVentaCabeceraPendientePorFiltro(DateTime fecha)
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
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fecha", fecha));

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
                        vResultadoTransaccion.data = true;
                    } else
                    {
                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.data = false;
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

        public async Task<ResultadoTransaccion<BE_VentasCabecera>> RegistrarVentaCabecera(BE_VentasCabecera value)
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

                if (((List<BE_SeriePorMaquina> )resultadoTransaccionSeriePorMaquina.dataList).Count > 0)
                {
                    BE_SeriePorMaquina seriePorMaquina = ((List<BE_SeriePorMaquina>)resultadoTransaccionSeriePorMaquina.dataList)[0];

                    if (!seriePorMaquina.codalmacen.Equals(value.codalmacen) && !seriePorMaquina.codalmacen.Equals(string.Empty))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Ud. solo puede vender del almacén: {0} - {1} ...Favor de revisar", seriePorMaquina.codalmacen, seriePorMaquina.desalmacen);

                        return vResultadoTransaccion;
                    }

                } else
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
                    } else
                    {
                        if (!listPaciente[0].activo.Equals(1)){
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "Atención desactivada...Favor de revisar";

                            return vResultadoTransaccion;
                        }
                    }
                }

                if (value.codpedido.Length.Equals(14))
                {
                    PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration);
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
                            
                        } else
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = "El centro de costo del pedido no tiene asignado un almacén";

                            return vResultadoTransaccion;
                        }
                    }
                }

                // Controla monto de venta a personal
                if (value.codtipocliente.Equals("03"))
                {
                    PersonalClinicaRepository personalClinicaRepository = new PersonalClinicaRepository(context, _configuration);
                    ResultadoTransaccion<BE_PersonalLimiteConsumo> resultadoTransaccionPersonalClinica = await personalClinicaRepository.GetListLimiteConsumoPorPersonal(value.codcliente);

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
                            vResultadoTransaccion.ResultadoDescripcion =  string.Format("El consumo al CREDITO es mayor al límite de consumo <br> en el periodo del {0} al {1} <br> Monto consumo (NO incluye esta venta): {2} <br> Monto Limite de Consumo {3}...Favor de revisar", personalClinicas[0].fecha1, personalClinicas[0].fecha2, personalClinicas[0].montoconsumo, personalClinicas[0].montolimite);

                            return vResultadoTransaccion;
                        }
                    } else
                    {
                        var nuevoConsumo = personalClinicas[0].montoconsumo + value.montoneto;
                        
                        if (nuevoConsumo <= personalClinicas[0].montolimite)
                        {
                            vResultadoTransaccion.ResultadoDescripcion = string.Format("El consumo al CREDITO en el periodo del {0} al {1} <br> Monto consumo (INCLUYE esta venta): {2} <br> Monto Limite de Consumo {3}...Favor de revisar", personalClinicas[0].fecha1, personalClinicas[0].fecha2, personalClinicas[0].montoconsumo, personalClinicas[0].montolimite);
                        }
                    }
                }

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
    }
}
