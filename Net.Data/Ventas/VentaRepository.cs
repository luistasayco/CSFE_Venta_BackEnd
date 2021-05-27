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
using System.Linq;

namespace Net.Data
{
    public class VentaRepository : RepositoryBase<BE_VentasCabecera>, IVentaRepository
    {
        private readonly string _cnx;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_ListaVentasCabeceraPorFiltrosGet";
        const string SP_GET_CABECERA_VENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ObtieneVentasCabeceraPorCodVentaGet";
        const string SP_GET_DETALLEVENTA_POR_CODVENTA = DB_ESQUEMA + "VEN_ListaVentaDetallePorCodVentaGet";
        const string SP_GET_CABECERA_VENTA_PENDIENTE_POR_FILTRO = DB_ESQUEMA + "VEN_ListaVentasCabeceraPendientesPorFiltrosGet";

        // Validaciones de la venta
        const string SP_GET_TIPOPRODUCTOPRESTACIONES = DB_ESQUEMA + "VEN_TipoproductoPrestacionesPorFiltroGet";
        const string SP_GET_GNCXVENTA = DB_ESQUEMA + "VEN_GncxVentaPorVentaGet";
        const string SP_GET_VALIDA_PRESOTOR = DB_ESQUEMA + "VEN_ValidaPresotorPorFiltroGet";

        const string SP_UPDATE_CABECERA_VENTA_ENVIO_PISO = DB_ESQUEMA + "VEN_VentaCabeceraEnvioPisoUpd";

        const string SP_INSERT = DB_ESQUEMA + "VEN_VentaCabeceraIns";
        const string SP_INSERT_DETALLE = DB_ESQUEMA + "VEN_VentaDetalleIns";
        const string SP_INSERT_DETALLE_DATOS = DB_ESQUEMA + "VEN_VentaDetalleDatosIns";

        const string SP_UPDATE = DB_ESQUEMA + "VEN_VentaCabeceraUpd";
        const string SP_UPDATE_DSCTO_DETALLE = DB_ESQUEMA + "VEN_VentaCabeceraUpdDscDet";


        // Clinica
        const string SP_PEDIDOXDEVOLVER_RECALCULO = DB_ESQUEMA + "Sp_PedidosxDevolver_Recalculo";
        const string SP_GET_PRESOTOR_CONSULTAVARIOS = DB_ESQUEMA + "Sp_Presotor_ConsultaVarios";
        const string SP_UPDATE_PRESOTOR = DB_ESQUEMA + "Sp_Presotor_Update";
        const string SP_UPDATE_PEDIDO = DB_ESQUEMA + "Sp_Pedidos_Update";
        const string SP_FARMACIA_PRESTACION_INSERT = DB_ESQUEMA + "Fa_FarmaciaPrestacion_Insert";

        public VentaRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
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

        public async Task<ResultadoTransaccion<BE_VentasCabecera>> RegistrarVentaCabecera(BE_VentasCabecera value)
        {
            ResultadoTransaccion<BE_VentasCabecera> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasCabecera>();
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

                    if(!tablaUsuarioAcceso.estado.Equals("G"))
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Usted no tiene permiso para este tipo de Movimiento";

                        return vResultadoTransaccion;
                    }

                } else
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
                        }
                    }
                }

                /*Validar si se tiene stock en el Almacen 0000002 + 0000019 => Para el tema de consignacion PENDIENTE DE COLOCAR VALIDACIÓN*/

                /*Iniciamos a grabar la venta*/

                List<BE_VentasDetalleQuiebre> listVentasDetalleQuiebres = new List<BE_VentasDetalleQuiebre>();

                foreach (var item in value.listaVentaDetalle)
                {
                    int vExiste = listVentasDetalleQuiebres.FindAll(xFila => xFila.codtipoproducto == item.codtipoproducto && xFila.igvproducto == item.igvproducto && xFila.narcotico == item.narcotico).Count;
                
                    if (vExiste.Equals(0))
                    {
                        listVentasDetalleQuiebres.Add(new BE_VentasDetalleQuiebre { codtipoproducto = item.codtipoproducto, narcotico = item.narcotico, igvproducto = item.igvproducto });
                    }
                }

                // Conexion de Logistica
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    // Conexion de Clinica
                    using (SqlConnection connClinica = new SqlConnection(_cnxClinica))
                    {
                        using (CommittableTransaction transaction = new CommittableTransaction())
                        {
                            // Conexion de Logistica
                            await conn.OpenAsync();
                            conn.EnlistTransaction(transaction);

                            try
                            {
                                using (CommittableTransaction transactionClinica = new CommittableTransaction())
                                {
                                    // Conexion de Clinica
                                    await connClinica.OpenAsync();
                                    connClinica.EnlistTransaction(transactionClinica);

                                    try
                                    {
                                        if (!listVentasDetalleQuiebres.Count.Equals(0))
                                        {

                                            string vCodigoPrestacion = string.Empty;
                                            decimal vGncPorVenta = 0;
                                            BE_ValidaPresotor vValidaPresotor = new BE_ValidaPresotor();
                                            //string vGnc = string.Empty;
                                            string vNC = string.Empty;
                                            bool vNoCubierto = false;
                                            string vCodigoPresotor = string.Empty;
                                            string vFlagPaquete = "N";
                                            List<BE_VentasGenerado> listVentasGenerados = new List<BE_VentasGenerado>();

                                            foreach (BE_VentasDetalleQuiebre itemQuiebre in listVentasDetalleQuiebres)
                                            {
                                                using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                                                {
                                                    cmd.Parameters.Clear();
                                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                                    SqlParameter oParam = new SqlParameter("@codigo", SqlDbType.Char, 8)
                                                    {
                                                        Direction = ParameterDirection.Output
                                                    };
                                                    cmd.Parameters.Add(oParam);

                                                    cmd.Parameters.Add(new SqlParameter("@codatencion", value.codatencion));
                                                    cmd.Parameters.Add(new SqlParameter("@codpedido", value.codpedido));
                                                    cmd.Parameters.Add(new SqlParameter("@codalmacen", value.codalmacen));
                                                    cmd.Parameters.Add(new SqlParameter("@tipomovimiento", value.tipomovimiento));
                                                    cmd.Parameters.Add(new SqlParameter("@codtipocliente", value.codtipocliente));
                                                    cmd.Parameters.Add(new SqlParameter("@codempresa", value.codempresa));
                                                    cmd.Parameters.Add(new SqlParameter("@codcliente", value.codcliente));
                                                    cmd.Parameters.Add(new SqlParameter("@codpaciente", value.codpaciente));
                                                    cmd.Parameters.Add(new SqlParameter("@nombre", value.nombre));
                                                    cmd.Parameters.Add(new SqlParameter("@cama", value.cama));
                                                    cmd.Parameters.Add(new SqlParameter("@codmedico", value.codmedico));
                                                    cmd.Parameters.Add(new SqlParameter("@codpoliza", value.codpoliza));
                                                    cmd.Parameters.Add(new SqlParameter("@deducible", value.deducible));
                                                    cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
                                                    cmd.Parameters.Add(new SqlParameter("@codcia", value.codcia));
                                                    cmd.Parameters.Add(new SqlParameter("@porcentajecoaseguro", value.porcentajecoaseguro));
                                                    cmd.Parameters.Add(new SqlParameter("@montodctoplan", value.montodctoplan));
                                                    cmd.Parameters.Add(new SqlParameter("@porcentajedctoplan", value.porcentajedctoplan));
                                                    cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
                                                    cmd.Parameters.Add(new SqlParameter("@montototal", value.montototal));
                                                    cmd.Parameters.Add(new SqlParameter("@montoigv", value.montoigv));
                                                    cmd.Parameters.Add(new SqlParameter("@montoneto", value.montoneto));
                                                    cmd.Parameters.Add(new SqlParameter("@codplan", value.codplan));
                                                    cmd.Parameters.Add(new SqlParameter("@montopaciente", value.montopaciente));
                                                    cmd.Parameters.Add(new SqlParameter("@montoaseguradora", value.montoaseguradora));
                                                    cmd.Parameters.Add(new SqlParameter("@observacion", value.observacion));
                                                    cmd.Parameters.Add(new SqlParameter("@nombremedico", value.nombremedico));
                                                    cmd.Parameters.Add(new SqlParameter("@nombreaseguradora", value.nombreaseguradora));
                                                    cmd.Parameters.Add(new SqlParameter("@nombrecia", value.nombrecia));
                                                    cmd.Parameters.Add(new SqlParameter("@nombrediagnostico", value.nombrediagnostico));
                                                    cmd.Parameters.Add(new SqlParameter("@flagpaquete", value.flagpaquete));
                                                    cmd.Parameters.Add(new SqlParameter("@flg_gratuito", value.flg_gratuito));
                                                    cmd.Parameters.Add(new SqlParameter("@usuario", value.usuario));
                                                    cmd.Parameters.Add(new SqlParameter("@codcentro", value.codcentro));

                                                    // Datos de Auditoria
                                                    cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                                                    // Datos de Transaccion
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

                                                    value.codventa = (string)oParam.Value;

                                                    vResultadoTransaccion.IdRegistro = 0;
                                                    vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                                                    vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                                                }

                                                if (value.codpedido != null)
                                                {
                                                    if (value.codpedido.Length.Equals(14))
                                                    {

                                                        PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration);
                                                        ResultadoTransaccion<BE_Pedido> resultadoTransaccionPedido = await pedidoRepository.GetDatosPedidoPorPedido(value.codpedido);

                                                        if (resultadoTransaccionPedido.ResultadoCodigo == -1)
                                                        {
                                                            transactionClinica.Rollback();
                                                            transaction.Rollback();
                                                            vResultadoTransaccion.IdRegistro = -1;
                                                            vResultadoTransaccion.ResultadoCodigo = -1;
                                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPedido.ResultadoDescripcion;
                                                            return vResultadoTransaccion;
                                                        }

                                                        if (resultadoTransaccionPedido.dataList.Any())
                                                        {
                                                            vFlagPaquete = ((List<BE_Pedido>)resultadoTransaccionPedido.dataList)[0].flg_paquete;
                                                        }

                                                    }
                                                }

                                                List<BE_VentasDetalle> listVentasDetalle = value.listaVentaDetalle.FindAll(xFila => xFila.codtipoproducto == itemQuiebre.codtipoproducto && xFila.igvproducto == itemQuiebre.igvproducto && xFila.narcotico == itemQuiebre.narcotico).ToList();

                                                foreach (BE_VentasDetalle item in listVentasDetalle)
                                                {
                                                    using (SqlCommand cmdDetalle = new SqlCommand(SP_INSERT_DETALLE, conn))
                                                    {
                                                        if (vFlagPaquete.Equals("S"))
                                                        {
                                                            item.gnc = "N";
                                                        }

                                                        if (vNC.Equals("S"))
                                                        {
                                                            vNoCubierto = true;
                                                        }

                                                        cmdDetalle.Parameters.Clear();
                                                        cmdDetalle.CommandType = System.Data.CommandType.StoredProcedure;

                                                        cmdDetalle.Parameters.Add(new SqlParameter("@codventa", value.codventa));

                                                        SqlParameter oParamDetalle = new SqlParameter("@coddetalle", SqlDbType.Char, 10)
                                                        {
                                                            Direction = ParameterDirection.Output
                                                        };
                                                        cmdDetalle.Parameters.Add(oParamDetalle);

                                                        cmdDetalle.Parameters.Add(new SqlParameter("@codalmacen", item.codalmacen));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@tipomovimiento", item.tipomovimiento));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@codproducto", item.codproducto));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@dsc_producto", item.nombreproducto));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@cnt_unitario", item.cantidad));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@prc_unitario", item.precioventaPVP));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@preciounidadcondcto", 1));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@precioventaPVP", item.precioventaPVP));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@valorVVP", item.valorVVP));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@porcentajedctoproducto", item.porcentajedctoproducto));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@montototal", item.montototal));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@montopaciente", item.montopaciente));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@montoaseguradora", item.montoaseguradora));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@codpedido", item.codpedido));
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@gnc", item.gnc));

                                                        // Datos de Auditoria
                                                        cmdDetalle.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                                                        // Datos de Transaccion
                                                        SqlParameter outputIdTransaccionParamDetalle = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                                                        {
                                                            Direction = ParameterDirection.Output
                                                        };
                                                        cmdDetalle.Parameters.Add(outputIdTransaccionParamDetalle);

                                                        SqlParameter outputMsjTransaccionParamDetalle = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                                                        {
                                                            Direction = ParameterDirection.Output
                                                        };
                                                        cmdDetalle.Parameters.Add(outputMsjTransaccionParamDetalle);

                                                        await cmdDetalle.ExecuteNonQueryAsync();

                                                        item.codventa = value.codventa;
                                                        item.coddetalle = (string)oParamDetalle.Value;
                                                    }

                                                    if (item.VentasDetalleDatos != null)
                                                    {
                                                        if (!item.VentasDetalleDatos.tipodocumentoautorizacion.Equals("00"))
                                                        {
                                                            using (SqlCommand cmdDatos = new SqlCommand(SP_INSERT_DETALLE_DATOS, conn))
                                                            {
                                                                cmdDatos.Parameters.Clear();
                                                                cmdDatos.CommandType = System.Data.CommandType.StoredProcedure;

                                                                cmdDatos.Parameters.Add(new SqlParameter("@codventa", item.codventa));
                                                                cmdDatos.Parameters.Add(new SqlParameter("@coddetalle", item.coddetalle));

                                                                cmdDatos.Parameters.Add(new SqlParameter("@codproducto", item.codproducto));
                                                                cmdDatos.Parameters.Add(new SqlParameter("@tipodocumentoautorizacion", item.VentasDetalleDatos.tipodocumentoautorizacion));
                                                                cmdDatos.Parameters.Add(new SqlParameter("@numerodocumentoautorizacion", item.VentasDetalleDatos.numerodocumentoautorizacion));

                                                                // Datos de Auditoria
                                                                cmdDatos.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
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

                                                                await cmdDatos.ExecuteNonQueryAsync();
                                                            }
                                                        }
                                                    }

                                                    // Validar
                                                    if (value.codtipocliente.Equals("03"))
                                                    {
                                                        if (value.codpedido != null)
                                                        {
                                                            if (value.codpedido.Length.Equals(14))
                                                            {

                                                                ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccionPedido = await PedidoxDevolverRecalculo(connClinica, item.codpedido, item.codproducto, item.cantidad, 0, 0);

                                                                if (vResultadoTransaccionPedido.ResultadoCodigo == -1)
                                                                {
                                                                    transactionClinica.Rollback();
                                                                    transaction.Rollback();
                                                                    vResultadoTransaccion.IdRegistro = -1;
                                                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                                                    vResultadoTransaccion.ResultadoDescripcion = vResultadoTransaccionPedido.ResultadoDescripcion;
                                                                    return vResultadoTransaccion;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }

                                                /*Genera transferencia automatica*/

                                                if (value.codtipocliente.Equals("01"))
                                                {
                                                    if (value.codpedido != null)
                                                    {
                                                        if (value.codpedido.Length.Equals(14))
                                                        {
                                                            ResultadoTransaccion<string> resultadoTransaccionPrestacion = await GetTipoProductoPrestaciones(conn, value.codventa, itemQuiebre.codtipoproducto, "078");

                                                            if (resultadoTransaccionPrestacion.ResultadoCodigo == -1)
                                                            {
                                                                transactionClinica.Rollback();
                                                                transaction.Rollback();
                                                                vResultadoTransaccion.IdRegistro = -1;
                                                                vResultadoTransaccion.ResultadoCodigo = -1;
                                                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPrestacion.ResultadoDescripcion;
                                                                return vResultadoTransaccion;
                                                            }

                                                            vCodigoPrestacion = resultadoTransaccionPrestacion.data;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ResultadoTransaccion<string> resultadoTransaccionPrestacion = await GetTipoProductoPrestaciones(conn, value.codventa, itemQuiebre.codtipoproducto, string.Empty);

                                                        if (resultadoTransaccionPrestacion.ResultadoCodigo == -1)
                                                        {
                                                            transactionClinica.Rollback();
                                                            transaction.Rollback();
                                                            vResultadoTransaccion.IdRegistro = -1;
                                                            vResultadoTransaccion.ResultadoCodigo = -1;
                                                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPrestacion.ResultadoDescripcion;
                                                            return vResultadoTransaccion;
                                                        }

                                                        vCodigoPrestacion = resultadoTransaccionPrestacion.data;
                                                    }

                                                    ResultadoTransaccion<decimal> resultadoTransaccionGncxVenta = await GetGncPorVenta(conn, value.codventa);

                                                    if (resultadoTransaccionGncxVenta.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionGncxVenta.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    vGncPorVenta = resultadoTransaccionGncxVenta.data;

                                                    ResultadoTransaccion<BE_ValidaPresotor> resultadoTransaccionValidaPresotor = await GetValidaPresotor(conn, value.codatencion, value.codventa);

                                                    if (resultadoTransaccionValidaPresotor.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValidaPresotor.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    vValidaPresotor = resultadoTransaccionValidaPresotor.data;

                                                    if (vValidaPresotor.seguir_venta.Equals("N"))
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
                                                        return vResultadoTransaccion;
                                                    }

                                                    ResultadoTransaccion<string> resultadoTransaccionCodigoPresotor = await ResgistraPrestacion(connClinica ,value.codatencion, vCodigoPrestacion, (float)value.montototal, (float)vGncPorVenta, (float)value.montopaciente, 0, (float)value.porcentajecoaseguro, 0, value.codventa, 0, value.observacion, value.tipomovimiento);

                                                    if (resultadoTransaccionCodigoPresotor.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCodigoPresotor.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    vCodigoPresotor = resultadoTransaccionCodigoPresotor.data;

                                                    if (vValidaPresotor.codatencion_ver != vCodigoPresotor.Substring(0, 8))
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
                                                        return vResultadoTransaccion;
                                                    }

                                                    using (SqlCommand cmdUpdate = new SqlCommand(SP_UPDATE, conn))
                                                    {
                                                        cmdUpdate.Parameters.Clear();
                                                        cmdUpdate.CommandType = System.Data.CommandType.StoredProcedure;

                                                        cmdUpdate.Parameters.Add(new SqlParameter("@codventa", value.codventa));

                                                        cmdUpdate.Parameters.Add(new SqlParameter("@codpresotor", vCodigoPresotor));

                                                        // Datos de Auditoria
                                                        cmdUpdate.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                                                        // Datos de Transaccion
                                                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                                                        {
                                                            Direction = ParameterDirection.Output
                                                        };
                                                        cmdUpdate.Parameters.Add(outputIdTransaccionParam);

                                                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                                                        {
                                                            Direction = ParameterDirection.Output
                                                        };
                                                        cmdUpdate.Parameters.Add(outputMsjTransaccionParam);

                                                        await cmdUpdate.ExecuteNonQueryAsync();
                                                    }

                                                    ResultadoTransaccion<string> resultadoTransaccionPresotorConsultaVarios = await GetPresotorConsultaVarios(connClinica, value.codatencion, vCodigoPresotor, 3);

                                                    if (resultadoTransaccionPresotorConsultaVarios.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPresotorConsultaVarios.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    if (resultadoTransaccionPresotorConsultaVarios.data.Equals("N"))
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = "Error al generar la venta...!!!, Por favor grabar la venta de nuevo";
                                                        return vResultadoTransaccion;
                                                    }

                                                    ResultadoTransaccion<bool> resultadoTransaccionActualizarPresotor = new ResultadoTransaccion<bool>();

                                                    if (vFlagPaquete.Equals("S"))
                                                    {
                                                        resultadoTransaccionActualizarPresotor = await ActualizarPresotor(connClinica, "flagpaquete", vCodigoPresotor, "S");
                                                    }
                                                    else
                                                    {
                                                        resultadoTransaccionActualizarPresotor = await ActualizarPresotor(connClinica, "flagpaquete", vCodigoPresotor, "N");
                                                    }

                                                    if (resultadoTransaccionActualizarPresotor.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionActualizarPresotor.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    if (!value.codatencion.Substring(1, 1).Equals("A"))
                                                    {
                                                        if (vNoCubierto)
                                                        {
                                                            ResultadoTransaccion<bool> resultadoTransaccionValorGNC = await ActualizarPresotor(connClinica, "valorgnc", vCodigoPresotor, vGncPorVenta.ToString());

                                                            if (resultadoTransaccionValorGNC.ResultadoCodigo == -1)
                                                            {
                                                                transactionClinica.Rollback();
                                                                transaction.Rollback();
                                                                vResultadoTransaccion.IdRegistro = -1;
                                                                vResultadoTransaccion.ResultadoCodigo = -1;
                                                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionValorGNC.ResultadoDescripcion;
                                                                return vResultadoTransaccion;
                                                            }

                                                            ResultadoTransaccion<bool> resultadoTransaccionLiqTerceroContratante = await ActualizarPresotor(connClinica, "liqtercerocontratante", vCodigoPresotor, "S");

                                                            if (resultadoTransaccionLiqTerceroContratante.ResultadoCodigo == -1)
                                                            {
                                                                transactionClinica.Rollback();
                                                                transaction.Rollback();
                                                                vResultadoTransaccion.IdRegistro = -1;
                                                                vResultadoTransaccion.ResultadoCodigo = -1;
                                                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionLiqTerceroContratante.ResultadoDescripcion;
                                                                return vResultadoTransaccion;
                                                            }
                                                        }
                                                    }

                                                    ResultadoTransaccion<bool> resultadoTransaccionAfectoImpuesto = new ResultadoTransaccion<bool>();

                                                    if (itemQuiebre.igvproducto.Equals(0))
                                                    {
                                                        resultadoTransaccionAfectoImpuesto = await ActualizarPresotor(connClinica, "afectoimpuesto", vCodigoPresotor, "N");
                                                    }
                                                    else
                                                    {
                                                        resultadoTransaccionAfectoImpuesto = await ActualizarPresotor(connClinica, "afectoimpuesto", vCodigoPresotor, "S");
                                                    }

                                                    if (resultadoTransaccionAfectoImpuesto.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionAfectoImpuesto.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }

                                                    ResultadoTransaccion<bool> resultadoTransaccionCodMedicoEnvia = await ActualizarPresotor(connClinica, "codmedicoenvia", vCodigoPresotor, value.codmedico);

                                                    if (resultadoTransaccionCodMedicoEnvia.ResultadoCodigo == -1)
                                                    {
                                                        transactionClinica.Rollback();
                                                        transaction.Rollback();
                                                        vResultadoTransaccion.IdRegistro = -1;
                                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCodMedicoEnvia.ResultadoDescripcion;
                                                        return vResultadoTransaccion;
                                                    }
                                                }

                                                if (value.codtipocliente.Equals("03"))
                                                {
                                                    if (value.codpedido != null)
                                                    {
                                                        if (value.codpedido.Length.Equals(14))
                                                        {

                                                            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccionPedido = await PedidoxDevolverRecalculo(connClinica, value.codpedido, string.Empty, 0, 0, 1);

                                                            if (vResultadoTransaccionPedido.ResultadoCodigo == -1)
                                                            {
                                                                transactionClinica.Rollback();
                                                                transaction.Rollback();
                                                                vResultadoTransaccion.IdRegistro = -1;
                                                                vResultadoTransaccion.ResultadoCodigo = -1;
                                                                vResultadoTransaccion.ResultadoDescripcion = vResultadoTransaccionPedido.ResultadoDescripcion;
                                                                return vResultadoTransaccion;
                                                            }
                                                        }
                                                    }
                                                }

                                                listVentasGenerados.Add(new BE_VentasGenerado { codventa = value.codventa, codpresotor = vCodigoPresotor });

                                            }

                                            using (SqlCommand cmdDscDetalle = new SqlCommand(SP_UPDATE_DSCTO_DETALLE, conn))
                                            {
                                                foreach (BE_VentasGenerado itemVenta in listVentasGenerados)
                                                {
                                                    cmdDscDetalle.Parameters.Clear();
                                                    cmdDscDetalle.CommandType = System.Data.CommandType.StoredProcedure;

                                                    cmdDscDetalle.Parameters.Add(new SqlParameter("@codventa", itemVenta.codventa));
                                                    cmdDscDetalle.Parameters.Add(new SqlParameter("@codpresotor", itemVenta.codpresotor));
                                                    // Datos de Auditoria
                                                    cmdDscDetalle.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                                                    // Datos de Transaccion
                                                    SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                                                    {
                                                        Direction = ParameterDirection.Output
                                                    };
                                                    cmdDscDetalle.Parameters.Add(outputIdTransaccionParam);

                                                    SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                                                    {
                                                        Direction = ParameterDirection.Output
                                                    };
                                                    cmdDscDetalle.Parameters.Add(outputMsjTransaccionParam);

                                                    await cmdDscDetalle.ExecuteNonQueryAsync();
                                                }
                                            }

                                            if (value.codtipocliente.Equals("01"))
                                            {
                                                ResultadoTransaccion<bool> resultadoTransaccionVistoBueno = await ActualizarPedido(connClinica, "vistobueno", value.codpedido, "L", value.RegIdUsuario.ToString());

                                                if (resultadoTransaccionVistoBueno.ResultadoCodigo == -1)
                                                {
                                                    transactionClinica.Rollback();
                                                    transaction.Rollback();
                                                    vResultadoTransaccion.IdRegistro = -1;
                                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVistoBueno.ResultadoDescripcion;
                                                    return vResultadoTransaccion;
                                                }

                                                ResultadoTransaccion<bool> resultadoTransaccionFechaAtencion = await ActualizarPedido(connClinica, "fechaatencion_servidor", value.codpedido, "L", DateTime.Now.ToString());

                                                if (resultadoTransaccionFechaAtencion.ResultadoCodigo == -1)
                                                {
                                                    transactionClinica.Rollback();
                                                    transaction.Rollback();
                                                    vResultadoTransaccion.IdRegistro = -1;
                                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionFechaAtencion.ResultadoDescripcion;
                                                    return vResultadoTransaccion;
                                                }
                                            }
                                        }

                                        transactionClinica.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        transactionClinica.Rollback();
                                        transaction.Rollback();
                                        vResultadoTransaccion.IdRegistro = -1;
                                        vResultadoTransaccion.ResultadoCodigo = -1;
                                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                                    }
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
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_VentasDetalle>> PedidoxDevolverRecalculo(SqlConnection conn, string codpedido, string codproducto, decimal entero, decimal menudeo,int orden)
        {
            ResultadoTransaccion<BE_VentasDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_VentasDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            //using (SqlConnection connPedido = new SqlConnection(_cnxClinica))
            //{
            try
            {
                using (SqlCommand cmdPedido = new SqlCommand(SP_PEDIDOXDEVOLVER_RECALCULO, conn))
                {
                    cmdPedido.Parameters.Clear();
                    cmdPedido.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdPedido.Parameters.Add(new SqlParameter("@codpedido", codpedido));
                    cmdPedido.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                    cmdPedido.Parameters.Add(new SqlParameter("@cantE", entero));
                    cmdPedido.Parameters.Add(new SqlParameter("@cantM", menudeo));
                    cmdPedido.Parameters.Add(new SqlParameter("@orden", orden));

                    //await connPedido.OpenAsync();
                    await cmdPedido.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            //}

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<string>> ResgistraPrestacion(SqlConnection conn, string codatencion, string codpresentacion, float montototal, float gnc, float montopaciente, float porcentajededucible, float coaseguro, float descuento, string codventa, float deducible, string observacion, string tipomovimiento)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            //using (SqlConnection connPedido = new SqlConnection(_cnxClinica))
            //{
            try
            {

                string vCodPresotor = string.Empty;

                using (SqlCommand cmdPedido = new SqlCommand(SP_FARMACIA_PRESTACION_INSERT, conn))
                {
                    cmdPedido.Parameters.Clear();
                    cmdPedido.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdPedido.Parameters.Add(new SqlParameter("@codatencion", codatencion));
                    cmdPedido.Parameters.Add(new SqlParameter("@codprestacion", codpresentacion));
                    cmdPedido.Parameters.Add(new SqlParameter("@valorcorregido", montototal));
                    cmdPedido.Parameters.Add(new SqlParameter("@valorgnc", gnc));
                    cmdPedido.Parameters.Add(new SqlParameter("@valorcoaseguro", montopaciente));
                    cmdPedido.Parameters.Add(new SqlParameter("@porcentajededucible", porcentajededucible));
                    cmdPedido.Parameters.Add(new SqlParameter("@porcentajecoaseguro", coaseguro));
                    cmdPedido.Parameters.Add(new SqlParameter("@descuento", descuento));
                    cmdPedido.Parameters.Add(new SqlParameter("@documento", codventa));
                    cmdPedido.Parameters.Add(new SqlParameter("@deducible", deducible));
                    cmdPedido.Parameters.Add(new SqlParameter("@observaciones", observacion));
                    cmdPedido.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento));

                    SqlParameter outputCodPresotorParam = new SqlParameter("@codpresotor", SqlDbType.Char, 12)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmdPedido.Parameters.Add(outputCodPresotorParam);

                    //await connPedido.OpenAsync();
                    await cmdPedido.ExecuteNonQueryAsync();

                    vCodPresotor = (string)outputCodPresotorParam.Value;

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
                    vResultadoTransaccion.data = vCodPresotor;
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            //}

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<string>> GetTipoProductoPrestaciones(SqlConnection conn, string codventa, string codtipproducto, string codcentropedido)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                //using (SqlConnection connTipoProducto = new SqlConnection(_cnx))
                //{
                using (SqlCommand cmdTipoProductoPrestaciones = new SqlCommand(SP_GET_TIPOPRODUCTOPRESTACIONES, conn))
                {
                    cmdTipoProductoPrestaciones.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@codventa", codventa));
                    cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@tipoproducto", codtipproducto));
                    cmdTipoProductoPrestaciones.Parameters.Add(new SqlParameter("@centrocsf", codcentropedido));

                    string response = string.Empty;

                    //await conn.OpenAsync();

                    using (var reader = await cmdTipoProductoPrestaciones.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = ((reader["codprestacion"]) is DBNull) ? string.Empty : (string)reader["codprestacion"];
                        }
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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

        public async Task<ResultadoTransaccion<decimal>> GetGncPorVenta(SqlConnection conn, string codventa)
        {
            ResultadoTransaccion<decimal> vResultadoTransaccion = new ResultadoTransaccion<decimal>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                //using (SqlConnection connGncPorVenta = new SqlConnection(_cnx))
                //{
                using (SqlCommand cmdGncPorVenta = new SqlCommand(SP_GET_GNCXVENTA, conn))
                {
                    cmdGncPorVenta.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdGncPorVenta.Parameters.Add(new SqlParameter("@codventa", codventa));

                    decimal response = 0;

                    //await connGnkcPorVenta.OpenAsync();

                    using (var reader = await cmdGncPorVenta.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = ((reader["valorgnc"]) is DBNull) ? 0 : (decimal)reader["valorgnc"];
                        }
                    }


                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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

        public async Task<ResultadoTransaccion<BE_ValidaPresotor>> GetValidaPresotor(SqlConnection conn, string codatencion, string codventa)
        {
            ResultadoTransaccion<BE_ValidaPresotor> vResultadoTransaccion = new ResultadoTransaccion<BE_ValidaPresotor>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                //using (SqlConnection connValidaPresotor = new SqlConnection(_cnx))
                //{
                using (SqlCommand cmdValidaPresotor = new SqlCommand(SP_GET_VALIDA_PRESOTOR, conn))
                {
                    cmdValidaPresotor.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdValidaPresotor.Parameters.Add(new SqlParameter("@codatencion", codatencion));
                    cmdValidaPresotor.Parameters.Add(new SqlParameter("@codventa", codventa));

                    BE_ValidaPresotor response = new BE_ValidaPresotor();

                    //await connValidaPresotor.OpenAsync();

                    using (var reader = await cmdValidaPresotor.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.seguir_venta = ((reader["seguir_venta"]) is DBNull) ? string.Empty : (string)reader["seguir_venta"];
                            response.codatencion_ver = ((reader["codatencion_ver"]) is DBNull) ? string.Empty : (string)reader["codatencion_ver"];
                            response.codpresotor_ver = ((reader["codpresotor_ver"]) is DBNull) ? string.Empty : (string)reader["codpresotor_ver"];
                        }
                    }


                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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

        public async Task<ResultadoTransaccion<string>> GetPresotorConsultaVarios(SqlConnection conn, string codatencion, string codpresotor, int orden)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                //using (SqlConnection connPresotorConsultaVarios = new SqlConnection(_cnxClinica))
                //{
                using (SqlCommand cmdPresotorConsultaVarios = new SqlCommand(SP_GET_PRESOTOR_CONSULTAVARIOS, conn))
                {
                    cmdPresotorConsultaVarios.CommandType = System.Data.CommandType.StoredProcedure;
                    cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@codigo", codatencion));
                    cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@codpresotor", codpresotor));
                    cmdPresotorConsultaVarios.Parameters.Add(new SqlParameter("@orden", orden));

                    string response = string.Empty;

                    //await conn.OpenAsync();

                    using (var reader = await cmdPresotorConsultaVarios.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = ((reader["seguir_venta"]) is DBNull) ? string.Empty : (string)reader["seguir_venta"];
                        }
                    }


                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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

        public async Task<ResultadoTransaccion<bool>> ActualizarPresotor(SqlConnection conn, string campo, string codpresotor, string nuevovalor)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            //using (SqlConnection connPedido = new SqlConnection(_cnxClinica))
            //{
            try
            {
                using (SqlCommand cmdActualizarPresotor = new SqlCommand(SP_UPDATE_PRESOTOR, conn))
                {
                    cmdActualizarPresotor.Parameters.Clear();
                    cmdActualizarPresotor.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@campo", campo));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@codigo", codpresotor));
                    cmdActualizarPresotor.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));

                    //await connPedido.OpenAsync();
                    await cmdActualizarPresotor.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            //}

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<bool>> ActualizarPedido(SqlConnection conn, string campo, string codigo, string nuevovalor, string coduser)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            //using (SqlConnection connPedido = new SqlConnection(_cnxClinica))
            //{
            try
            {
                using (SqlCommand cmdActualizarPedido = new SqlCommand(SP_UPDATE_PEDIDO, conn))
                {
                    cmdActualizarPedido.Parameters.Clear();
                    cmdActualizarPedido.CommandType = System.Data.CommandType.StoredProcedure;

                    cmdActualizarPedido.Parameters.Add(new SqlParameter("@wcampo", campo));
                    cmdActualizarPedido.Parameters.Add(new SqlParameter("@codigo", codigo));
                    cmdActualizarPedido.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));
                    cmdActualizarPedido.Parameters.Add(new SqlParameter("@coduser", coduser));

                    //await connPedido.OpenAsync();
                    await cmdActualizarPedido.ExecuteNonQueryAsync();

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            //}

            return vResultadoTransaccion;
        }
    }
}
