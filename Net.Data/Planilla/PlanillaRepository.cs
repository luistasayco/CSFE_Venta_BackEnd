using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.CrossCotting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class PlanillaRepository : RepositoryBase<BE_Planilla>, IPlanillaRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";

        const string SP_GET_REPORTE_PLANILLA = DB_ESQUEMA + "VEN_Reporte_CuadreCaja";
        const string SP_GET_REPORTE_PLANILLA_RESUMEN = DB_ESQUEMA + "VEN_Reporte_CuadreCajaResumen";
        const string SP_GET_REPORTE_PLANILLA_DETALLE = DB_ESQUEMA + "VEN_Reporte_CuadreCajaDetallado";

        const string SP_INSERTXUSUER = DB_ESQUEMA + "VEN_PlanillaxUserIns";
        const string SP_UPDATE = DB_ESQUEMA + "VEN_PlanillasUpd";
        const string SP_DELETE = DB_ESQUEMA + "VEN_PlanillasDel";
        const string SP_PLANILLA_POR_FILTRO = DB_ESQUEMA + "VEN_ListaPlanillasPorFiltroGet";

        public PlanillaRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<string>> RegistrarPorUsuario(BE_Planilla value)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();
            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            using (SqlConnection conn = new SqlConnection(_cnx))
            {

                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERTXUSUER, conn, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idusuario", value.idusuario));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", value.codcentro));
                        cmd.Parameters.Add(new SqlParameter("@monto", value.montodolares));
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", string.Empty));//value.codcomprobante
                        cmd.Parameters.Add(new SqlParameter("@ingresoegreso", string.Empty));// value.ingresoegreso
                        cmd.Parameters.Add(new SqlParameter("@accion", "1"));

                        SqlParameter oParam = new SqlParameter("@numeroplanilla", SqlDbType.Char,8)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(oParam);

                        var result = await cmd.ExecuteNonQueryAsync();
                        value.numeroplanilla = (string)oParam.Value;

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = $"PLANILLA PROCESADA {value.numeroplanilla}";
                        vResultadoTransaccion.data = value.numeroplanilla;

                    }

                    if (value.numeroplanilla.Length>0)
                    {

                        foreach (var item in value.planilladetalle)
                        {
                            using (SqlCommand cmd = new SqlCommand(SP_INSERTXUSUER, conn, transaction))
                            {
                                cmd.Parameters.Clear();
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@idusuario", value.idusuario));
                                cmd.Parameters.Add(new SqlParameter("@codcentro", string.Empty));
                                cmd.Parameters.Add(new SqlParameter("@monto", item.monto));
                                cmd.Parameters.Add(new SqlParameter("@codcomprobante", item.codcomprobante));//value.codcomprobante
                                cmd.Parameters.Add(new SqlParameter("@ingresoegreso", item.ingresoegreso));// value.ingresoegreso
                                cmd.Parameters.Add(new SqlParameter("@accion", "2"));
                                cmd.Parameters.Add(new SqlParameter("@strnumeroplanilla", value.numeroplanilla));

                                SqlParameter oParam = new SqlParameter("@numeroplanilla", SqlDbType.Char, 8)
                                {
                                    Direction = ParameterDirection.Output,
                                };
                                cmd.Parameters.Add(oParam);

                                var result = await cmd.ExecuteNonQueryAsync();

                                vResultadoTransaccion.IdRegistro = 0;
                                vResultadoTransaccion.ResultadoCodigo = 0;
                                vResultadoTransaccion.ResultadoDescripcion = "PLANILLA PROCESADA.";
                                vResultadoTransaccion.data = value.numeroplanilla;
                            }
                        }
                    }

                    transaction.Commit();
                    transaction.Dispose();

                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex3)
                    {
                        vResultadoTransaccion.ResultadoDescripcion = ex3.Message.ToString();
                    }

                }
            }


            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Planilla>> Modificar(BE_Planilla values)
        {
            ResultadoTransaccion<BE_Planilla> vResultadoTransaccion = new ResultadoTransaccion<BE_Planilla>();
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
                        cmd.Parameters.Add(new SqlParameter("@campo", values.campo));
                        cmd.Parameters.Add(new SqlParameter("@codigo", values.numeroplanilla));
                        cmd.Parameters.Add(new SqlParameter("@nuevovalor", values.fecha));

                        await conn.OpenAsync();
                        int result = await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = result;
                        vResultadoTransaccion.ResultadoCodigo = result;
                        vResultadoTransaccion.ResultadoDescripcion = "Grabación exitosa.";
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

        public async Task<ResultadoTransaccion<BE_Planilla>> Elminar(string numeroplanilla)
        {
            ResultadoTransaccion<BE_Planilla> vResultadoTransaccion = new ResultadoTransaccion<BE_Planilla>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_DELETE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroplanilla));

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "La planilla fue eliminada.";
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

        public async Task<ResultadoTransaccion<BE_Planilla>> GetPlanillasPorFiltro(string buscar, int key, int numerolineas, int orden, string serie)
        {
            ResultadoTransaccion<BE_Planilla> vResultadoTransaccion = new ResultadoTransaccion<BE_Planilla>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new BE_Planilla();
                    using (SqlCommand cmd = new SqlCommand(SP_PLANILLA_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));
                        cmd.Parameters.Add(new SqlParameter("@serie", serie));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = context.Convert<BE_Planilla>(reader);

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

        public async Task<ResultadoTransaccion<BE_Planilla>> GetListaPlanillasPorFiltro(string buscar, int key, int numerolineas, int orden, string serie, string codcentro, string idusuario, string numeroPlanilla, string fechaInicio, string fechaFin)
        {
            ResultadoTransaccion<BE_Planilla> vResultadoTransaccion = new ResultadoTransaccion<BE_Planilla>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Planilla>();
                    using (SqlCommand cmd = new SqlCommand(SP_PLANILLA_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));
                        cmd.Parameters.Add(new SqlParameter("@serie", serie));
                        cmd.Parameters.Add(new SqlParameter("@numeroPlanilla", (numeroPlanilla == string.Empty) ? null : numeroPlanilla));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", ( Convert.ToInt32(idusuario)==0)? null: idusuario));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", (fechaInicio == string.Empty) ? null : fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", (fechaFin == string.Empty) ? null : fechaFin));

                        //cmd.Parameters.Add(new SqlParameter("@codcentro", (codcentro == string.Empty) ? null : codcentro));


                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Planilla>)context.ConvertTo<BE_Planilla>(reader);
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

        public async Task<ResultadoTransaccion<BE_ReportePlanilla>> GetReportePlanillaPorNumero(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares)
        {
            ResultadoTransaccion<BE_ReportePlanilla> vResultadoTransaccion = new ResultadoTransaccion<BE_ReportePlanilla>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            if (fechaInicio != null)
            {
                fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            }

            if (fechaFin != null)
            {
                fechaFin = Utilidades.GetFechaHoraFinActual(fechaFin);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ReportePlanilla>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_REPORTE_PLANILLA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechaFin));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", idusuario));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroPlanilla));
                        cmd.Parameters.Add(new SqlParameter("@dolares", dolares));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ReportePlanilla>)context.ConvertTo<BE_ReportePlanilla>(reader);

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

        public async Task<ResultadoTransaccion<BE_ReportePlanillaDetalle>> GetReporteDetallePlanillaPorNumero(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden)
        {
            ResultadoTransaccion<BE_ReportePlanillaDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_ReportePlanillaDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            if (fechaInicio != null)
            {
                fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            }

            if (fechaFin != null)
            {
                fechaFin = Utilidades.GetFechaHoraFinActual(fechaFin);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ReportePlanillaDetalle>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_REPORTE_PLANILLA_DETALLE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechaFin));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", idusuario));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroPlanilla));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ReportePlanillaDetalle>)context.ConvertTo<BE_ReportePlanillaDetalle>(reader);

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

        public async Task<ResultadoTransaccion<BE_ReportePlanillaResumen>> GetReporteResumenPlanillaPorNumero(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares)
        {
            ResultadoTransaccion<BE_ReportePlanillaResumen> vResultadoTransaccion = new ResultadoTransaccion<BE_ReportePlanillaResumen>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            if (fechaInicio != null)
            {
                fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            }

            if (fechaFin != null)
            {
                fechaFin = Utilidades.GetFechaHoraFinActual(fechaFin);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ReportePlanillaResumen>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_REPORTE_PLANILLA_RESUMEN, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechaFin));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", idusuario));
                        cmd.Parameters.Add(new SqlParameter("@codcentro", codcentro));
                        cmd.Parameters.Add(new SqlParameter("@numeroplanilla", numeroPlanilla));
                        cmd.Parameters.Add(new SqlParameter("@dolares", dolares));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ReportePlanillaResumen>)context.ConvertTo<BE_ReportePlanillaResumen>(reader);

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

        public async Task<ResultadoTransaccion<MemoryStream>> GenerarReportePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                Document doc = new Document();
                var pgSize = new iTextSharp.text.Rectangle(PageSize.Letter);
                doc.SetPageSize(pgSize.Rotate());
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

                ResultadoTransaccion<BE_ReportePlanilla> resultadoTransaccionPlanilla = await GetReportePlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, dolares);

                if (resultadoTransaccionPlanilla.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanilla.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<BE_ReportePlanillaResumen> resultadoTransaccionPlanillaResumen = await GetReporteResumenPlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, dolares);

                if (resultadoTransaccionPlanillaResumen.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanillaResumen.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionPlanilla.dataList.Any())
                {
                    var lista = (List<BE_ReportePlanilla>)resultadoTransaccionPlanilla.dataList;
                    var listaResumen = (List<BE_ReportePlanillaResumen>)resultadoTransaccionPlanillaResumen.dataList;


                    var tbl = new PdfPTable(new float[] { 40f, 30f, 30f }) { WidthPercentage = 100 };

                    #region "Reporte"

                    var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, titulo)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    var texto = string.Format("Planilla de Ingreso de: {0}", lista[0].area);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    texto = string.Format("Planilla : {0}", numeroPlanilla);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    texto = string.Format("Del: {0}", lista[0].fechaplanilla);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    texto = string.Format("T.C : {0}", lista[0].tc);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    texto = string.Format("Usuario: {0}", lista[0].usuario);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    texto = string.Format("Emisión : {0}", DateTime.Now.ToString());

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new Phrase(" "));
                    //Obtenemos los datos de la venta

                    tbl = new PdfPTable(new float[] { 15f, 25f, 10f, 10f, 10f, 10f, 10f, 10f }) { WidthPercentage = 100 };

                    c1 = new PdfPCell(new Phrase("Documento", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Rowspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Nombre Paciente", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Rowspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("INGRESOS", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Colspan = 3;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("DEDUCCIONES", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Colspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Total Neto", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Rowspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Importe", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Garantias", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Total Ing.", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Documento", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Importe", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    double ingresos = 0;
                    double totalIngresos = 0;
                    double egresos = 0;
                    double totalNeto = 0;

                    foreach (BE_ReportePlanilla item in lista)
                    {
                        ingresos += item.ingresos;
                        totalIngresos += item.ingresos;
                        egresos += item.egresos;
                        totalNeto += item.ingresos;

                        c1 = new PdfPCell(new Phrase(item.documentoe, parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.paciente, parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.ingresos,2).ToString(), parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.ingresos, 2).ToString(), parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);

                        string docreferencia = item.docreferencia == item.documentoe ? string.Empty : item.docreferencia;

                        c1 = new PdfPCell(new Phrase(docreferencia, parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.egresos,2).ToString(), parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round((item.ingresos - item.egresos),2).ToString(), parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);
                    }

                    // Totales
                    c1 = new PdfPCell(new Phrase("TOTAL INGRESOS", parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.Colspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(ingresos,2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalIngresos,2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(egresos,2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalNeto,2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    #endregion
                    doc.Add(new Chunk(string.Empty));

                    tbl = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
                    c1 = new PdfPCell(new Phrase("RESUMEN:", parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);
                    doc.Add(tbl);

                    #region "Resumen"
                    tbl = new PdfPTable(new float[] { 20f, 10f, 10f, 10f, 50f }) { WidthPercentage = 100 };

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Colspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("DOLARES AL CAMBIO", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("SOLES", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    List<BE_ReportePlanillaResumenFind> reportePlanillaResumenFinds = new List<BE_ReportePlanillaResumenFind>();

                    foreach (BE_ReportePlanillaResumen item in listaResumen)
                    {

                        var existeRegistros = reportePlanillaResumenFinds.FindAll(xFila => xFila.tipopago == item.tipopago.Trim() && xFila.entidad == item.entidad.Trim()).Count;

                        if (existeRegistros == 0)
                        {
                            reportePlanillaResumenFinds.Add(new BE_ReportePlanillaResumenFind
                            {
                                tipopago = item.tipopago.Trim(),
                                entidad = item.entidad.Trim(),
                                dolares = item.moneda.Trim() == "DOLARES AL CAMBIO" ? item.monto : 0,
                                soles = item.moneda.Trim() == "SOLES" ? item.monto : 0,
                                totallinea = item.totallinea
                            });
                        } else
                        {
                            reportePlanillaResumenFinds.Find(xFila => xFila.tipopago == item.tipopago.Trim() && xFila.entidad == item.entidad.Trim()).dolares += item.moneda.Trim() == "DOLARES AL CAMBIO" ? item.monto : 0;
                            reportePlanillaResumenFinds.Find(xFila => xFila.tipopago == item.tipopago.Trim() && xFila.entidad == item.entidad.Trim()).soles += item.moneda.Trim() == "SOLES" ? item.monto : 0;
                        }
                    }

                    string tipopago = string.Empty;
                    double totaldolares = 0;
                    double totalsoles = 0;

                    double totalglobaldolares = 0;
                    double totalglobalsoles = 0;

                    foreach (BE_ReportePlanillaResumenFind item in reportePlanillaResumenFinds)
                    {

                        if (tipopago.Trim() != item.tipopago.Trim() && tipopago != string.Empty)
                        {
                            //c1 = new PdfPCell(new Phrase(tipopago, parrafoNegro)) { BorderWidth = 1 };
                            //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            //c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            //tbl.AddCell(c1);

                            c1 = new PdfPCell(new Phrase("TOTAL", parrafoNegro)) { BorderWidth = 1 };
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            tbl.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Math.Round(totaldolares, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tbl.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(Math.Round(totalsoles, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tbl.AddCell(c1);

                            c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            tbl.AddCell(c1);

                            totaldolares = 0;
                            totalsoles = 0;
                        }

                        if (tipopago.Trim() != item.tipopago.Trim() )
                        {
                            c1 = new PdfPCell(new Phrase(item.tipopago, parrafoNegro)) { BorderWidth = 1 };
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            c1.HorizontalAlignment = Element.ALIGN_LEFT;
                            c1.Rowspan = item.totallinea;
                            tbl.AddCell(c1);
                        } 

                        c1 = new PdfPCell(new Phrase(item.entidad, parrafoNegro)) { BorderWidth = 1 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_LEFT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.dolares,2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.soles,2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        tbl.AddCell(c1);

                        totaldolares += item.dolares;
                        totalsoles += item.soles;

                        totalglobaldolares += item.dolares;
                        totalglobalsoles += item.soles;

                        tipopago = item.tipopago;
                    }

                    //c1 = new PdfPCell(new Phrase(tipopago, parrafoNegro)) { BorderWidth = 1 };
                    //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("TOTAL", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totaldolares, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalsoles, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("TOTAL", parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.Colspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalglobaldolares, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalglobalsoles, 2).ToString(), parrafoNegro)) { BorderWidth = 1 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    tbl.AddCell(c1);


                    doc.Add(tbl);
                    #endregion

                }

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

        public async Task<ResultadoTransaccion<MemoryStream>> GenerarReporteDetallePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.Letter.Rotate());
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

                ResultadoTransaccion<BE_ReportePlanillaDetalle> resultadoTransaccionPlanilla = await GetReporteDetallePlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, orden);

                if (resultadoTransaccionPlanilla.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanilla.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionPlanilla.dataList.Any())
                {
                    var lista = (List<BE_ReportePlanillaDetalle>)resultadoTransaccionPlanilla.dataList;


                    var tbl = new PdfPTable(new float[] { 20f, 60f, 20f }) { WidthPercentage = 100 };

                    var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    var texto = string.Format("DETALLE PLANILLA DEL {0}", ((DateTime)lista[0].fechaplanilla).ToShortDateString());

                    c1 = new PdfPCell(new Phrase(texto, titulo)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    tbl.AddCell(c1);

                    texto = string.Format("PLANILLA : {0}", numeroPlanilla);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.Colspan = 3;
                    tbl.AddCell(c1);

                    texto = string.Format("AREA : {0}", lista[0].nombrecentro);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.Colspan = 3;
                    tbl.AddCell(c1);

                    texto = string.Format("USUARIO : {0}", lista[0].nombreusuario);

                    c1 = new PdfPCell(new Phrase(texto, parrafoNegroNegrita)) { Border = 0 };
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.Colspan = 3;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                    doc.Add(new Phrase(" "));
                    //Obtenemos los datos de la venta

                    tbl = new PdfPTable(new float[] { 15f, 10f, 30f, 10f, 10f, 7f, 10f, 8f }) { WidthPercentage = 100 };

                    c1 = new PdfPCell(new Phrase("Comprobante", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("T. Cli.", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Anombre de", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Neto", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Monto Parcial TC", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("T.Pago", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Entidad", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase("Moneda", parrafoNegro)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    decimal totalParcial = 0;
                    string codcomprobante = string.Empty;

                    foreach (BE_ReportePlanillaDetalle item in lista)
                    {

                        totalParcial += item.montoparcial;

                        c1 = new PdfPCell(new Phrase(item.codcomprobante, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.nombretipocliente, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.anombrede, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        if (codcomprobante == item.codcomprobante && !string.IsNullOrEmpty(codcomprobante))
                        {
                            c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegro)) { Border = 0 };
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tbl.AddCell(c1);
                        } else
                        {
                            c1 = new PdfPCell(new Phrase(Math.Round(item.monto, 2).ToString(), parrafoNegro)) { Border = 0 };
                            c1.HorizontalAlignment = Element.ALIGN_CENTER;
                            c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                            tbl.AddCell(c1);
                        }

                        //c1 = new PdfPCell(new Phrase(Math.Round(item.monto,2).ToString(), parrafoNegro)) { Border = 0 };
                        //c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        //c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(Math.Round(item.montoparcial,2).ToString(), parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.tipopago, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.nombreentidad, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        c1 = new PdfPCell(new Phrase(item.moneda, parrafoNegro)) { Border = 0 };
                        c1.HorizontalAlignment = Element.ALIGN_CENTER;
                        c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tbl.AddCell(c1);

                        codcomprobante = item.codcomprobante;
                    }

                    c1 = new PdfPCell(new Phrase("TOTAL DE REGISTROS:", parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.Colspan = 2;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(lista.Count.ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_LEFT;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(lista[0].m_sumagrupo,2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(Math.Round(totalParcial, 2).ToString(), parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tbl.AddCell(c1);

                    c1 = new PdfPCell(new Phrase(string.Empty, parrafoNegroNegrita)) { Border = 0 };
                    c1.HorizontalAlignment = Element.ALIGN_CENTER;
                    c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    c1.Colspan = 3;
                    tbl.AddCell(c1);

                    doc.Add(tbl);

                }

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

        public async Task<ResultadoTransaccion<MemoryStream>> GenerarExcelPlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, decimal dolares)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                MemoryStream stream = new MemoryStream();

                ResultadoTransaccion<BE_ReportePlanilla> resultadoTransaccionPlanilla = await GetReportePlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, dolares);

                if (resultadoTransaccionPlanilla.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanilla.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                ResultadoTransaccion<BE_ReportePlanillaResumen> resultadoTransaccionPlanillaResumen = await GetReporteResumenPlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, dolares);

                if (resultadoTransaccionPlanillaResumen.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanillaResumen.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                using (SpreadsheetDocument doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());
                    //Sheets sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = doc.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Planilla"
                    };
                    sheets.Append(sheet);
                    //worksheetPart.Worksheet.Save();
                    //workbookPart.Workbook.Save();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();


                    SheetData sheetData = (SheetData)worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.RowIndex = 1;
                    row.Append(
                        ConstructCellHeader("Documento", CellValues.String),
                        ConstructCellHeader("Nombre Paciente", CellValues.String),
                        ConstructCellHeader("Importe", CellValues.String),
                        ConstructCellHeader("Garantias", CellValues.String),
                        ConstructCellHeader("Total Ing.", CellValues.String),
                        ConstructCellHeader("Documento", CellValues.String),
                        ConstructCellHeader("Importe", CellValues.String),
                        ConstructCellHeader("Total Neto", CellValues.String)
                        );
                    sheetData.AppendChild(row);
                    workbookPart.Workbook.Save();

                    
                    DocumentFormat.OpenXml.Spreadsheet.Row rowIte;
                    foreach (BE_ReportePlanilla item in resultadoTransaccionPlanilla.dataList)
                    {
                        string docreferencia = item.docreferencia == item.documentoe ? string.Empty : item.docreferencia;

                        rowIte = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        rowIte.Append(
                            ConstructCell(item.documentoe, CellValues.String, 6),
                            ConstructCell(item.paciente, CellValues.String, 6),
                            ConstructCell(Math.Round(item.ingresos,2).ToString(), CellValues.String, 6),
                            ConstructCell(string.Empty, CellValues.String, 6),
                            ConstructCell(Math.Round(item.ingresos, 2).ToString(), CellValues.String, 6),
                            ConstructCell(docreferencia, CellValues.String, 6),
                            ConstructCell(Math.Round(item.egresos, 2).ToString(), CellValues.String, 6),
                            ConstructCell(Math.Round(item.ingresos- item.egresos, 2).ToString(), CellValues.String, 6)
                            );
                        sheetData.AppendChild(rowIte);
                    }

                    worksheetPart.Worksheet.Save();
                    workbookPart.Workbook.Save();
                    doc.Close();
                }

                stream.Seek(0, SeekOrigin.Begin);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = stream;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<MemoryStream>> GenerarExcelDetallePlanillaPrint(DateTime? fechaInicio, DateTime? fechaFin, int idusuario, string codcentro, string numeroPlanilla, string orden)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                MemoryStream stream = new MemoryStream();

                ResultadoTransaccion<BE_ReportePlanillaDetalle> resultadoTransaccionPlanilla = await GetReporteDetallePlanillaPorNumero(fechaInicio, fechaFin, idusuario, codcentro, numeroPlanilla, orden);

                if (resultadoTransaccionPlanilla.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPlanilla.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                using (SpreadsheetDocument doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());
                    //Sheets sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = doc.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Planilla"
                    };
                    sheets.Append(sheet);
                    //worksheetPart.Worksheet.Save();
                    //workbookPart.Workbook.Save();

                    WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();


                    SheetData sheetData = (SheetData)worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    row.RowIndex = 1;
                    row.Append(
                        ConstructCellHeader("Comprobante", CellValues.String),
                        ConstructCellHeader("T.Cli.", CellValues.String),
                        ConstructCellHeader("Anombre de", CellValues.String),
                        ConstructCellHeader("Neto", CellValues.String),
                        ConstructCellHeader("Monto Parcial TC", CellValues.String),
                        ConstructCellHeader("T.Pago", CellValues.String),
                        ConstructCellHeader("Entidad", CellValues.String),
                        ConstructCellHeader("Moneda", CellValues.String)
                        );
                    sheetData.AppendChild(row);
                    workbookPart.Workbook.Save();


                    DocumentFormat.OpenXml.Spreadsheet.Row rowIte;

                    string codcomprobante = string.Empty;
                    decimal monto = 0;

                    foreach (BE_ReportePlanillaDetalle item in resultadoTransaccionPlanilla.dataList)
                    {
                        if (codcomprobante == item.codcomprobante && !string.IsNullOrEmpty(codcomprobante))
                        {
                            monto = 0;
                        }
                        else
                        {
                            monto = Math.Round(item.monto, 2);
                        }

                        rowIte = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        rowIte.Append(
                            ConstructCell(item.codcomprobante, CellValues.String, 6),
                            ConstructCell(item.nombretipocliente, CellValues.String, 6),
                            ConstructCell(item.anombrede, CellValues.String, 6),
                            ConstructCell(Math.Round(monto, 2).ToString(), CellValues.Number, 6),
                            ConstructCell(Math.Round(item.montoparcial, 2).ToString(), CellValues.Number, 6),
                            ConstructCell(item.tipopago, CellValues.String, 6),
                            ConstructCell(item.nombreentidad, CellValues.String, 6),
                            ConstructCell(item.moneda, CellValues.String, 6)
                            );
                        sheetData.AppendChild(rowIte);

                        codcomprobante = item.codcomprobante;
                    }

                    worksheetPart.Worksheet.Save();
                    workbookPart.Workbook.Save();
                    doc.Close();
                }

                stream.Seek(0, SeekOrigin.Begin);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = stream;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }
        private Stylesheet GenerateStylesheet()
        {

            Stylesheet styleSheet = null;
            Fonts fonts = new Fonts(
                new DocumentFormat.OpenXml.Spreadsheet.Font(
                    new FontSize() { Val = 10 }
                ),
                new DocumentFormat.OpenXml.Spreadsheet.Font(
                    new FontSize() { Val = 10 },
                    new Bold()
                ),
                new DocumentFormat.OpenXml.Spreadsheet.Font(
                    new FontSize() { Val = 10 },
                    new Color() { Rgb = "FEFEFF" }
                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "DEEBF6" } }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "F8CBAC" } }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FFF3CB" } }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "C5E0B2" } }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FBE4D5" } }) { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "2F5497" } }) { PatternType = PatternValues.Solid })
                );

            Borders borders = new Borders(
                    new Border(),
                    new Border(
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Dashed },//Dashed, Dotted
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Dashed },
                        new TopBorder(new Color() { Rgb = "000000" }) { Style = BorderStyleValues.Dashed },//Dotted
                        new BottomBorder(new Color() { Rgb = "000000" }) { Style = BorderStyleValues.Dashed },
                        new DiagonalBorder()),
                    new Border( // index 2 blue
                        new LeftBorder(new Color() { Rgb = "2F5497" }) { Style = BorderStyleValues.Dashed },
                        new RightBorder(new Color() { Rgb = "2F5497" }) { Style = BorderStyleValues.Dashed },
                        new TopBorder(new Color() { Rgb = "2F5497" }) { Style = BorderStyleValues.Dashed },
                        new BottomBorder(new Color() { Rgb = "2F5497" }) { Style = BorderStyleValues.Dashed },
                        new DiagonalBorder()
                        )
                );


            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // 1 body
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }, // 2 header
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true }, // 3 header
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 4, BorderId = 1, ApplyFill = true }, // 4header
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, FillId = 4, BorderId = 1, ApplyFill = true }, // 5 sin negrita
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, FillId = 0, BorderId = 2, ApplyFill = true }, // 6 border blue
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 5, BorderId = 1, ApplyFill = true }, // 7 green header
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 6, BorderId = 1, ApplyFill = true }, // 8 rosa header
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 7, BorderId = 1, ApplyFill = true } // 9 rosa header
                );



            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);


            return styleSheet;
        }

        private DocumentFormat.OpenXml.Spreadsheet.Cell ConstructCellHeader(string value, CellValues dataType)
        {
            return new DocumentFormat.OpenXml.Spreadsheet.Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = 1,
            };
        }

        private DocumentFormat.OpenXml.Spreadsheet.Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new DocumentFormat.OpenXml.Spreadsheet.Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }
    }
}
