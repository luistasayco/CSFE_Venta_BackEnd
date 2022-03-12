using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.CrossCotting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private readonly string _cnxLogistica;
        private readonly IConfiguration _configuration;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";

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
        const string SP_GET_LISTA_CORREO = DB_ESQUEMA + "Sp_CorreoDestinatario_Consulta";
        const string SP_CHECKLIST_ENVIAR_CORREO = DB_ESQUEMA + "Ut_EnviarCorreov2";
        const string SP_CHECKLIST_REPORTE = DB_ESQUEMA + "Rp_Chkl_Registro_Mov_Reporte";

        public CheckListRegistroMovimientoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _aplicacionName = this.GetType().Name;
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _cnxLogistica = configuration.GetConnectionString("cnnSqlLogistica");
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
                        value.ide_registro_mov = oParam.Value is DBNull ? 0 : (int)oParam.Value;

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

        public async Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoUpdatet(string value, string comentario)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            List<BE_CheckListRegistroMovimiento> data = JsonConvert.DeserializeObject<List<BE_CheckListRegistroMovimiento>>(value);

            string cod_atencion = string.Empty;
            string vTextoComentario = comentario;

            using (SqlConnection conn = new SqlConnection(_cnxClinica))
            {
                conn.Open();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_REGISTRO_MOVIMIENTO_UPDATE, conn))
                    {
                        foreach (var item in data)
                        {

                            cod_atencion = item.cod_atencion;

                            cmd.Parameters.Clear();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@ide_actividad", item.ide_actividad));
                            cmd.Parameters.Add(new SqlParameter("@cod_atencion", item.cod_atencion));
                            cmd.Parameters.Add(new SqlParameter("@ide_tarea", item.ide_tarea));

                            var result = await cmd.ExecuteNonQueryAsync();

                            vResultadoTransaccion.IdRegistro = 0;
                            vResultadoTransaccion.ResultadoCodigo = 0;
                            vResultadoTransaccion.ResultadoDescripcion = $"CHECK LIST ACTUALIZADA { item.ide_tarea }";
                            //vResultadoTransaccion.data = item.ide_registro_mov.ToString();
                        }
                        
                    }

                    ResultadoTransaccion<BE_CheckListRegistroMovimiento> resultadoTransaccioEnviarCorreo = await GetCkeckListRegistroMovimientoEnviarCorreo(cod_atencion, 1);

                    if (resultadoTransaccioEnviarCorreo.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccioEnviarCorreo.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    string vCuerpo = string.Empty;

                    vCuerpo += string.Format("Nombre del Paciente: <br>");
                    vCuerpo += string.Format("Nro. de Atención: {0} <br><br>", cod_atencion);

                    vCuerpo += string.Format("Se registró de forma incorrecta los siguientes Ítems del Check List de Alta Paciente: <br><br>");

                    foreach (BE_CheckListRegistroMovimiento item in resultadoTransaccioEnviarCorreo.dataList)
                    {
                        if (item.num_orden <= 16)
                        {
                            vCuerpo += string.Format("{0} . {1} (Usuario: {2} - Fecha: {3} <br>", item.num_orden, item.dsc_nombre, item.nombre, item.fec_registro);
                        }
                        else
                        {
                            vCuerpo += string.Format("<br> REPORTE USUARIO DE ALTAS MEDICAS (Usuario: {0} - Fecha: {1} <br>", item.nombre, item.fec_registro);
                        }
                    }

                    if (!string.IsNullOrEmpty(vTextoComentario))
                    {
                        vCuerpo += string.Format("<br> <br> Descripcion de los Errores Registrados <br><br> {0}", vTextoComentario);
                    }

                    ResultadoTransaccion<BE_CheckListCorreoDestinatario> resultadoTransaccionDestinatario = await GetCkeckListDestinatariosCorreo("CORREO_CHECK_ALTA_PACIENTE", "TO");

                    if (resultadoTransaccionDestinatario.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDestinatario.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    string vDestinatarios = ((List<BE_CheckListCorreoDestinatario>)resultadoTransaccionDestinatario.dataList)[0].destinatario;

                    if (!string.IsNullOrEmpty(vDestinatarios))
                    {
                        ResultadoTransaccion<string> resultadoTransaccionEnvioCorreo = await CheckListEnviarCorreo(vDestinatarios, "", "", "Error: Check List de Alta Paciente", vCuerpo, null);

                        if (resultadoTransaccionDestinatario.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDestinatario.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }
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

        public async Task<ResultadoTransaccion<string>> CheckListEnviarCorreo(string enviara, string copiara, string copiarh, string asunto, string cuerpo, string fila)
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
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_ENVIAR_CORREO, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@enviara", enviara));
                        cmd.Parameters.Add(new SqlParameter("@copiara", copiara));
                        cmd.Parameters.Add(new SqlParameter("@copiarh", copiarh));
                        cmd.Parameters.Add(new SqlParameter("@asunto", asunto));
                        cmd.Parameters.Add(new SqlParameter("@cuerpo", cuerpo));
                        cmd.Parameters.Add(new SqlParameter("@fila", fila));

                        var result = await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"CHECK ENVIO DE CORREO CORRECTAMENTE";
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
                    var response = new List<BE_CheckListRegistroMovimiento>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CheckListRegistroMovimiento>)context.ConvertTo<BE_CheckListRegistroMovimiento>(reader);
                            //response = context.Convert<BE_CheckListRegistroMovimiento>(reader);
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

        public async Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCkeckListRegistroMovimientoEnviarCorreo(string cod_atencion, int ide_tarea)
        {
            ResultadoTransaccion<BE_CheckListRegistroMovimiento> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListRegistroMovimiento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new List<BE_CheckListRegistroMovimiento>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_ENVIAR_CORREO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CheckListRegistroMovimiento>)context.ConvertTo<BE_CheckListRegistroMovimiento>(reader);
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

        public async Task<ResultadoTransaccion<BE_CheckListCorreoDestinatario>> GetCkeckListDestinatariosCorreo(string cod_lista, string dsc_tipo)
        {
            ResultadoTransaccion<BE_CheckListCorreoDestinatario> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListCorreoDestinatario>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new List<BE_CheckListCorreoDestinatario>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_CORREO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_lista", cod_lista));
                        cmd.Parameters.Add(new SqlParameter("@dsc_tipo", dsc_tipo));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CheckListCorreoDestinatario>)context.ConvertTo<BE_CheckListCorreoDestinatario>(reader);
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

        public async Task<ResultadoTransaccion<BE_CheckListReporte>> GetCkeckListReporte(string codatencion)
        {
            ResultadoTransaccion<BE_CheckListReporte> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListReporte>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new List<BE_CheckListReporte>();
                    using (SqlCommand cmd = new SqlCommand(SP_CHECKLIST_REPORTE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CheckListReporte>)context.ConvertTo<BE_CheckListReporte>(reader);
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
        public async Task<ResultadoTransaccion<MemoryStream>> GenerarCkeckListReportePrint(string codatencion)
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

                var tbl = new PdfPTable(new float[] { 20f, 60f, 20f }) { WidthPercentage = 100 };

                var title = string.Format("CHECK LIST DE ALTA DE PACIENTE");

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
                //Obtenemos los datos de la venta

                ResultadoTransaccion<BE_CheckListReporte> resultadoTransaccionCheckListReporte = await GetCkeckListReporte(codatencion);

                if (resultadoTransaccionCheckListReporte.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionCheckListReporte.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                List<BE_CheckListReporte> bE_CheckListReportes = ((List<BE_CheckListReporte>)resultadoTransaccionCheckListReporte.dataList);
                BE_CheckListReporte bE_CheckListReporte = ((List<BE_CheckListReporte>)resultadoTransaccionCheckListReporte.dataList)[0];

                // Generamos la Cabecera del vale de venta
                tbl = new PdfPTable(new float[] { 20f, 80f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Nombre del Paciente: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.NombrePaciente), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                //Linea 2
                tbl = new PdfPTable(new float[] { 20f, 30f, 20f, 30f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Nro.Habitación: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.cama), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nro.Atención", parrafoNegroNegrita)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.codatencion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                //Linea 3
                tbl = new PdfPTable(new float[] { 20f, 80f }) { WidthPercentage = 100 };
                //Linea 1
                c1 = new PdfPCell(new Phrase("Diagnostico de Egreso: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.Descripcion_Diagnostico), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta
                tbl = new PdfPTable(new float[] { 5f, 40f, 5f, 5f, 5f, 8f, 17f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Nro", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Descripción Item", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("SI", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("NO", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("NO AP", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Usuario", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);

                string grupo = string.Empty;

                foreach (BE_CheckListReporte item in bE_CheckListReportes)
                {
                    if (grupo != item.grupo)
                    {
                        c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegroNegrita)) { Border = 0 };
                        tbl.AddCell(c1);
                        c1 = new PdfPCell(new Phrase(item.grupo.ToString(), parrafoNegroNegrita)) { Border = 0 };
                        c1.Colspan = 6;
                        tbl.AddCell(c1);
                    }

                    c1 = new PdfPCell(new Phrase(item.num_orden.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.Descripcion_Check, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.SI.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.NO.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.NO_APLICA.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.Usuario.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.fec_registro.ToString(), parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);

                    grupo = item.grupo;
                }
                doc.Add(tbl);

                doc.Add(new Phrase(" "));
                // Totales
                tbl = new PdfPTable(new float[] { 30f, 20f, 30f, 20f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Recepcionista de H. clinica: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.auditoria_recepcion), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Elabora cuenta del paciente", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.auditoria_elabcuentapac), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                tbl = new PdfPTable(new float[] { 30f, 20f, 30f, 20f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Hora de recepción: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.fecha_recepcion.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Hora de elaboración", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.fecha_elabcuentapac.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                tbl = new PdfPTable(new float[] { 30f, 70f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Recepcionista de la cuenta: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.auditoria_reccuentaSAP), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Hora de recepción: ", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", bE_CheckListReporte.fecha_reccuentaSAP.ToString()), parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("___________________________________________", parrafoNegroNegrita)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("___________________________________________", parrafoNegroNegrita)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format("CAJA"), parrafoNegro)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format("FIRMA PACIENTE O FAMILIAR RESPONSABLE"), parrafoNegro)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 20f, 80f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("OBSERVACIONES:", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("______________________________________________________________________________", parrafoNegro)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                tbl = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("* Estimado paciente sirvase a presentar este documento al personal de seguridad a la salida", parrafoNegroNegrita)) { Border = 0 };
                tbl.AddCell(c1);
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
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
        public async Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimientoVerificar(string cod_atencion, int ide_tarea)
        {
            ResultadoTransaccion<BE_CheckListRegistroMovimiento> vResultadoTransaccion = new ResultadoTransaccion<BE_CheckListRegistroMovimiento>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new List<BE_CheckListRegistroMovimiento>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_CHECKLIST_REGISTRO_MOVIMIENTO_VERIFICAR, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cod_atencion", cod_atencion));
                        cmd.Parameters.Add(new SqlParameter("@ide_tarea", ide_tarea));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_CheckListRegistroMovimiento>)context.ConvertTo<BE_CheckListRegistroMovimiento>(reader);
                            //response = context.Convert<BE_CheckListRegistroMovimiento>(reader);
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
                        value.ide_comentariochecklist = oParam.Value is DBNull ? 0 : (int)oParam.Value; 

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

        public async Task<ResultadoTransaccion<BE_Hospital>> GetHospitalDetalle(string buscar, int key, int numerolineas, int orden, string tipoatencion, string filtrolocal)
        {
            ResultadoTransaccion<BE_Hospital> vResultadoTransaccion = new ResultadoTransaccion<BE_Hospital>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnxClinica))
                {
                    var response = new List<BE_Hospital>();

                    var hospital = new BE_Hospital();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_HOSPITAL_DETALLE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));
                        cmd.Parameters.Add(new SqlParameter("@tipoatencion", tipoatencion));
                        cmd.Parameters.Add(new SqlParameter("@filtrolocal", filtrolocal));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                hospital.nombres = ((reader["nombres"]) is DBNull) ? string.Empty : reader["nombres"].ToString().Trim();
                                hospital.cama = ((reader["cama"]) is DBNull) ? string.Empty : reader["cama"].ToString().Trim();
                                hospital.nombrediagnostico = ((reader["nombrediagnostico"]) is DBNull) ? string.Empty : reader["nombrediagnostico"].ToString().Trim();
                            }

                            response.Add(hospital);     

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

      }
}
