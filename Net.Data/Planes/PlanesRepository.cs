using Microsoft.Data.SqlClient;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Net.Data
{
    public class PlanesRepository : RepositoryBase<BE_Planes>, IPlanesRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_PlanesGet";
        const string SP_GET_ID = DB_ESQUEMA + "VEN_PlanesById";
        const string SP_INSERT = DB_ESQUEMA + "VEN_PlanesIns";
        const string SP_UPDATE = DB_ESQUEMA + "VEN_PlanesUpd";
        const string SP_DELETE = DB_ESQUEMA + "VEN_PlanesDel";

        public PlanesRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public Task<IEnumerable<BE_Planes>> GetByFiltros(BE_Planes value)
        {
            return Task.Run(() => {
                value.Nombre = value.Nombre == null ? "" : value.Nombre;
                return context.ExecuteSqlViewFindByCondition<BE_Planes>(SP_GET, new BE_Planes { Nombre = value.Nombre }, _cnx);
            });
        }
        public Task<BE_Planes> GetbyId(BE_Planes value)
        {
            return Task.Run(() => {
                return context.ExecuteSqlViewId<BE_Planes>(SP_GET_ID, new BE_Planes { IdPlan = value.IdPlan }, _cnx);
            });

        }

        public async Task<ResultadoTransaccion> Registrar(BE_Planes item)
        {
            ResultadoTransaccion vResultadoTransaccion = new ResultadoTransaccion();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.ResultadoMetodo = _metodoName;
            vResultadoTransaccion.ResultadoAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter oParam = new SqlParameter("@IdPlan", item.IdPlan);
                        oParam.SqlDbType = SqlDbType.Int;
                        oParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(oParam);

                        cmd.Parameters.Add(new SqlParameter("@Nombre", item.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@PorcentajeDescuento", item.PorcentajeDescuento));
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

                        vResultadoTransaccion.IdRegistro = (int)cmd.Parameters["@IdPlan"].Value;
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

        public async Task<ResultadoTransaccion> Modificar(BE_Planes item)
        {

            ResultadoTransaccion vResultadoTransaccion = new ResultadoTransaccion();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.ResultadoMetodo = _metodoName;
            vResultadoTransaccion.ResultadoAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@IdPlan", item.IdPlan));
                        cmd.Parameters.Add(new SqlParameter("@Nombre", item.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@FlgEstado", item.FlgEstado));
                        cmd.Parameters.Add(new SqlParameter("@PorcentajeDescuento", item.PorcentajeDescuento));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(item.IdPlan.ToString());
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
        public async Task<ResultadoTransaccion> Eliminar(BE_Planes value)
        {
            ResultadoTransaccion vResultadoTransaccion = new ResultadoTransaccion();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.ResultadoMetodo = _metodoName;
            vResultadoTransaccion.ResultadoAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@IdPlan", value.IdPlan));
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

                        vResultadoTransaccion.IdRegistro = int.Parse(value.IdPlan.ToString());
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
