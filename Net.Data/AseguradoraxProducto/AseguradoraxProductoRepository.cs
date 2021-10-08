using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Net.CrossCotting;

namespace Net.Data
{
    public class AseguradoraxProductoRepository : RepositoryBase<BE_AseguradoraxProducto>, IAseguradoraxProductoRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "VEN_AseguradoraxProductoGNCPorFiltroGet";
        const string SP_ASEGURADORA_X_PRODUCTO_INSERT = DB_ESQUEMA + "VEN_AseguradoraxProductoGNC_InsertV2";//"VEN_AseguradoraxProductoGNCPorFiltroGet";
        const string SP_ASEGURADORA_X_PRODUCTO_DELETE = DB_ESQUEMA + "VEN_AseguradoraxProductoDelete";//Sp_AseguradoraxProductoGNC_DeleteV2
        const string SP_GET_EXISTE_PRODUCTO = DB_ESQUEMA + "VEN_ExisteAseguradoraxProductoGet";
        const string SP_GET_EXISTE_CONVENIO = DB_ESQUEMA + "VEN_VerificaProductoConvenioGNC";//"VEN_ExisteConvenioAseguradoraxProductoGet";

        const string SP_GET_ASEGURADORA_X_PRODUCTO = DB_ESQUEMA + "VEN_AseguradoraxProductoGet";


        public AseguradoraxProductoRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }
        public async Task<ResultadoTransaccion<BE_AseguradoraxProducto>> GetListAseguradoraxProductoPorFiltros(string codaseguradora, string codproducto, int tipoatencion)
        {
            ResultadoTransaccion<BE_AseguradoraxProducto> vResultadoTransaccion = new ResultadoTransaccion<BE_AseguradoraxProducto>();
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
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));
                        cmd.Parameters.Add(new SqlParameter("@cod_tipoatencion_mae", tipoatencion));

                        var response = new List<BE_AseguradoraxProducto>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_AseguradoraxProducto>)context.ConvertTo<BE_AseguradoraxProducto>(reader);
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


        public async Task<ResultadoTransaccion<int>> AseguradoraxProductoInsert(BE_AseguradoraxProducto value, int orden)
        {
            ResultadoTransaccion<int> vResultadoTransaccion = new ResultadoTransaccion<int>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_ASEGURADORA_X_PRODUCTO_INSERT, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", value.codproducto));
                        cmd.Parameters.Add(new SqlParameter("@codtipoatencionmae", value.cod_tipoatencion_mae));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 1;
                        vResultadoTransaccion.ResultadoDescripcion = "SE REGISTRO CORRECTAMENTE!";
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

        public async Task<ResultadoTransaccion<int>> AseguradoraxProductoDelete(BE_AseguradoraxProducto value, int orden)
        {
            ResultadoTransaccion<int> vResultadoTransaccion = new ResultadoTransaccion<int>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_ASEGURADORA_X_PRODUCTO_DELETE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", value.codproducto));
                        cmd.Parameters.Add(new SqlParameter("@cod_tipoatencion_mae", value.cod_tipoatencion_mae));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "ELIMINADO CORRECTAMENTE";
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


        public async Task<ResultadoTransaccion<bool>> GetExisteAseguradoraxProducto(string codaseguradora, string codproducto)
        {
            ResultadoTransaccion<bool> vResultadoTransaccion = new ResultadoTransaccion<bool>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_EXISTE_PRODUCTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                        await conn.OpenAsync();
                        int existe = 0;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync()) { 
                            existe = Convert.ToInt32(reader["Existe"]);
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = (existe > 0) ? "Ya Existe..." : "No Existe..."; //"Producto existe";
                        vResultadoTransaccion.data = (existe>0)? true:false;

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

        public async Task<ResultadoTransaccion<string>> GetExisteConvenioAseguradoraxProducto(string codaseguradora, string codproducto)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_EXISTE_CONVENIO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));

                        await conn.OpenAsync();
                        string convenio = string.Empty;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                convenio = reader["convenio"].ToString();
                            }
                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = convenio;
                        vResultadoTransaccion.data = convenio;

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


        public async Task<ResultadoTransaccion<BE_AseguradoraxProducto>> GetListAseguradoraxProducto()
        {
            ResultadoTransaccion<BE_AseguradoraxProducto> vResultadoTransaccion = new ResultadoTransaccion<BE_AseguradoraxProducto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_AseguradoraxProducto>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ASEGURADORA_X_PRODUCTO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_AseguradoraxProducto>)context.ConvertTo<BE_AseguradoraxProducto>(reader);
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


        public async Task<ResultadoTransaccion<MemoryStream>> GenerarAseguradoraxProductoPrint()
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
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 9.5f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                ResultadoTransaccion<BE_AseguradoraxProducto> resultadoTransaccion = await GetListAseguradoraxProducto();

                List<BE_AseguradoraxProducto> response = (List<BE_AseguradoraxProducto>)resultadoTransaccion.dataList;

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var title = string.Format("HOJA DE ASEGURADORA POR PRODUCTO", titulo);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                
                doc.Add(new Phrase(" "));

                
                // Generamos el detalle 

                tbl = new PdfPTable(new float[] { 10f, 10f, 20f, 10f, 20f, 10f, 20f }) { WidthPercentage = 100 };
                c1 = new PdfPCell(new Phrase("Item", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Código Aseguradora", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nombre Aseguradora", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Código Producto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nombre producto", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha registro", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Tipo Atención", parrafoNegroNegrita));
                c1.BorderWidth = 1;
                c1.DisableBorderSide(Rectangle.LEFT_BORDER);
                c1.DisableBorderSide(Rectangle.RIGHT_BORDER);
                c1.DisableBorderSide(Rectangle.TOP_BORDER);
                tbl.AddCell(c1);


                int lina = 0;
                foreach (BE_AseguradoraxProducto item in response)
                {
                    lina++;

                    c1 = new PdfPCell(new Phrase(lina.ToString(), parrafoNegro)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.codaseguradora, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.nomseguradora, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.codproducto, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.nomproducto, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.fec_registro.ToString("dd/MM/yyyy"), parrafoNegro)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.dsctipoatencionmae, parrafoNegro)) { Border = 0 };
                    tbl.AddCell(c1);
                }
                doc.Add(tbl);

                doc.Add(new Phrase(" "));


                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente el PDF.";
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
    }
}