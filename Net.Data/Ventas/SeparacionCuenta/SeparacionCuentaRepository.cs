using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Net.Http;

namespace Net.Data
{
    public class SeparacionCuentaRepository : RepositoryBase<BE_VentasGenerado>, ISeparacionCuentaRepository
    {
        private readonly string _cnx;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        
        const string SP_INSERT_SEPARACION_CUENTA = DB_ESQUEMA + "VEN_SeparacionCuentaIns";

        public SeparacionCuentaRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        public async Task<ResultadoTransaccion<BE_SeperacionCuenta>> RegistrarSeparacionCuenta(BE_SeparacionCuentaXml value)
        {
            ResultadoTransaccion<BE_SeperacionCuenta> vResultadoTransaccion = new ResultadoTransaccion<BE_SeperacionCuenta>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var response = new BE_SeperacionCuentaGenerado();
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_SEPARACION_CUENTA, conn, transaction))
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

                        //await conn.CloseAsync();

                        XmlSerializer serializer = new XmlSerializer(typeof(BE_SeperacionCuentaGenerado));
                        using (TextReader reader = new StringReader(outputGeneradoTransaccionParam.Value.ToString()))
                        {
                            response = (BE_SeperacionCuentaGenerado)serializer.Deserialize(reader);
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                        vResultadoTransaccion.dataList = response.ListSeperacionCuentaGenerado;

                        #region "Envio a SAP"
                        List<BE_SeperacionCuenta> seperacionCuentasVentas = response.ListSeperacionCuentaGenerado.FindAll(xFila => xFila.tipomovimiento.TrimEnd() == "DV");
                        List<BE_SeperacionCuenta> seperacionCuentasDevolucion = response.ListSeperacionCuentaGenerado.FindAll(xFila => xFila.tipomovimiento.TrimEnd() == "CD");

                        VentaRepository ventaRepository = new VentaRepository(_clientFactory, context, _configuration);

                        foreach (BE_SeperacionCuenta item in seperacionCuentasDevolucion)
                        {
                            // Realizando Devoluciones
                            ResultadoTransaccion<bool> resultadoTransaccionVenta = await ventaRepository.DevolucionVentaSAPBase(conn, transaction, item.codventa, (int)value.RegIdUsuario);

                            if (resultadoTransaccionVenta.IdRegistro == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }
                        }

                        foreach (BE_SeperacionCuenta item in seperacionCuentasVentas)
                        {
                            // Realizando Entregas
                            ResultadoTransaccion<bool> resultadoTransaccionVenta = await ventaRepository.EnviarVentaSAP(conn, transaction, item.codventa, (int)value.RegIdUsuario);

                            if (resultadoTransaccionVenta.IdRegistro == -1)
                            {
                                transaction.Rollback();
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionVenta.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }
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
    }
}
