using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class CheckListRegistroMovimientoRepository : RepositoryBase<BE_CheckListRegistroMovimiento>, ICheckListRegistroMovimientoRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        //--------
        const string SP_GET_HOSPITAL = DB_ESQUEMA + "Sp_Hospital_Consulta";
        const string SP_CHECKLIST_REGISTRO_MOVIMIENTO_INSERT = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_Insert";
        const string SP_CHECKLIST_REGISTRO_MOVIMIENTO_UPDATE = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_UpdateV2";
        const string CHECKLIST_REGISTRO_MOVIMIENTO_ANULAR = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_AnularCheckV2";
        const string SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_ConsultaV2";
        const string SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_ENVIAR_CORREO = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_Enviar_CorreoV2";
        const string SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_VERIFICAR = DB_ESQUEMA + "Sp_Chkl_Registro_Mov_Consulta_VerificarV2";
        const string SP_CHECKLIST_COMENTARIO_ERROR_INSERT = DB_ESQUEMA + "Sp_Chkl_Comentario_Error_InsertV2";
        const string SP_GET_HOSPITAL_DOCUMENTO = DB_ESQUEMA + "Sp_HospitalDoc_Consulta";
        const string SP_GET_HOSPITAL_DETALLE = DB_ESQUEMA + "Sp_HospitalDetalle_Consulta";
        const string SP_GET_USUARIO = DB_ESQUEMA + "Sp_Usuarios_Consulta";

        public CheckListRegistroMovimientoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = this.GetType().Name;
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
        }


        public async Task<ResultadoTransaccion<BE_Hospital>> GetHospital(string buscar, int key, int numerolineas, int orden, int tipoatencion,string activo, int filtrolocal)
        {
            ResultadoTransaccion<BE_Hospital> vResultadoTransaccion = new ResultadoTransaccion<BE_Hospital>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_Hospital();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_HOSPITAL, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));
                        cmd.Parameters.Add(new SqlParameter("@tipoatencion", tipoatencion));
                        cmd.Parameters.Add(new SqlParameter("@activo", activo));
                        cmd.Parameters.Add(new SqlParameter("@filtrolocal", filtrolocal));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_Hospital>(reader);
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



        public async Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoInsert(BE_CheckListRegistroMovimiento value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();
                
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_REGISTRO_MOVIMIENTO_INSERT, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", value.ide_tarea));
                        cmd.Parameters.Add(new SqlParameter("@ide_actividad", value.ide_actividad));
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", value.cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@est_respuesta", value.est_respuesta));
                        cmd.Parameters.Add(new SqlParameter("@usr_crea", value.usr_crea));
                        cmd.Parameters.Add(new SqlParameter("@flg_estado", value.flg_estado));
                        cmd.Parameters.Add(new SqlParameter("@flg_revisado", value.flg_revisado));

                        SqlParameter oParam = new SqlParameter("@ide_registro_mov", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParam);

                        var result = await cmd.ExecuteNonQueryAsync();
                        value.ide_registro_mov = (int)oParam.Value;

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"CHECK LIST PROCESADA { value.ide_registro_mov }";
                        vResultadoTransaccion.data = value.ide_registro_mov.ToString();
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

        public async Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoUpdatet(BE_CheckListRegistroMovimiento value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_REGISTRO_MOVIMIENTO_UPDATE, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ide_actividad", value.ide_actividad));
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", value.cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", value.ide_tarea));

                        var result = await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"CHECK LIST ACTUALIZADA { value.ide_tarea }";
                        vResultadoTransaccion.data = value.ide_registro_mov.ToString();
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

        public async Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoAnular(BE_CheckListRegistroMovimiento value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(CHECKLIST_REGISTRO_MOVIMIENTO_ANULAR, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", value.cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", value.ide_tarea));

                        var result = await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"CHECK LIST ANULADA { value.ide_registro_mov }";
                        vResultadoTransaccion.data = value.ide_registro_mov.ToString();
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

        public async Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimiento(string cod_atencion, int ide_tarea, int orden)
        {
            ResultadoTransaccion<BE_CheckListRegistroMovimiento> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListRegistroMovimiento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_CheckListRegistroMovimiento();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_CheckListRegistroMovimiento>(reader);
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

        public async Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCkeckListRegistroMovimientoEnviarCorreo(string cod_atencion, int ide_tarea, int orden)
        {
            ResultadoTransaccion<BE_CheckListRegistroMovimiento> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListRegistroMovimiento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_CheckListRegistroMovimiento();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_ENVIAR_CORREO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_CheckListRegistroMovimiento>(reader);
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

        public async Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimientoVerificar(string cod_atencion, int ide_tarea, int orden)
        {
            ResultadoTransaccion<BE_CheckListRegistroMovimiento> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListRegistroMovimiento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_CheckListRegistroMovimiento();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_VERIFICAR, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_CheckListRegistroMovimiento>(reader);
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

        public async Task<ResultadoTransaccion<string>> ChecklistComentarioErrorInsert(BE_CheckListComentarioError value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_COMENTARIO_ERROR_INSERT, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", value.cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@dsc_comentario", value.dsc_comentario));
                        cmd.Parameters.Add(new SqlParameter("@flg_estado", value.flg_estado));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", value.ide_tarea));

                        SqlParameter oParam = new SqlParameter("@ide_comentariochecklist", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(oParam);

                        var result = await cmd.ExecuteNonQueryAsync();
                        value.ide_comentariochecklist = (int)oParam.Value;

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"CHECK LIST PROCESADA { value.ide_comentariochecklist }";
                        vResultadoTransaccion.data = value.ide_comentariochecklist.ToString();
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

        public async Task<ResultadoTransaccion<BE_HospitalDocumento>> GetHospitalDocumento(string pcod_atencion, int porden)
        {
            ResultadoTransaccion<BE_HospitalDocumento> vResultadoTransaccion = new ResultadoTransaccion<BE_HospitalDocumento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_HospitalDocumento();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_HOSPITAL_DOCUMENTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@pcod_atencion", pcod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@porden", porden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_HospitalDocumento>(reader);
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

        public async Task<ResultadoTransaccion<BE_Hospital>> GetHospitalDetalle(string buscar, int key, int numerolineas, int tipoatencion, int filtrolocal)
        {
            ResultadoTransaccion<BE_Hospital> vResultadoTransaccion = new ResultadoTransaccion<BE_Hospital>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new BE_Hospital();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_HOSPITAL_DETALLE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@tipoatencion", tipoatencion));
                        cmd.Parameters.Add(new SqlParameter("@filtrolocal", filtrolocal));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_Hospital>(reader);
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

      }
}
