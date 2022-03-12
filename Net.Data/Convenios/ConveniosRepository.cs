using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Data;

namespace Net.Data
{
    class ConveniosRepository : RepositoryBase<BE_ConveniosListaPrecio>, IConveniosRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        const string DB_ESQUEMA = "";
        //const string SP_GET = DB_ESQUEMA + "VEN_ConveniosPorFiltroGet";

        //const string SP_GET = DB_ESQUEMA + "VEN_ConvenioslistaprecioGet";

        const string SP_GET = DB_ESQUEMA + "VEN_ConvenioslistaprecioGet";
        const string SP_GET_FILTRO = DB_ESQUEMA + "VEN_ConveniosListaPrecioPorFiltroGet";
        const string SP_INSERT = DB_ESQUEMA + "VEN_ConvenioslistaprecioIns";
        const string SP_UPDATE = DB_ESQUEMA + "VEN_ConvenioslistaprecioUpd";
        const string SP_DELETE = DB_ESQUEMA + "VEN_ConvenioslistaprecioDel";

        public ConveniosRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConveniosPorFiltros(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                var response = new List<BE_ConveniosListaPrecio>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codalmacen", codalmacen == null ? string.Empty : codalmacen));
                        cmd.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento == null ? string.Empty : tipomovimiento));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente == null ? string.Empty : codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente == null ? string.Empty : codcliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente == null ? string.Empty : codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora == null ? string.Empty : codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", codcia == null ? string.Empty : codcia));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConveniosListaPrecio>)context.ConvertTo<BE_ConveniosListaPrecio>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.dataList = response;
                    }
                }

                if (response.Count > 0)
                {
                    ListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_ListaPrecio> resultadoTransaccionListaPrecio = await listaPrecioRepository.GetPrecioPorCodigo(codproducto, response[0].pricelist);

                    if (resultadoTransaccionListaPrecio.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListaPrecio.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList).Count > 0)
                    {
                        BE_ListaPrecio bE_ListaPrecio = ((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList)[0];

                        response[0].monto = bE_ListaPrecio.Price == null ? 0 : double.Parse(bE_ListaPrecio.Price.ToString());
                    } else
                    {
                        response[0].monto = 0;
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

        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConvenioslistaprecio(int idconvenio, int pricelist, string codtipocliente, string codpaciente, string codaseguradora, string codcliente,string fechareg, string tmovimiento)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var response = new List<BE_ConveniosListaPrecio>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idconvenio", idconvenio == 0 ? 0 : idconvenio));
                        cmd.Parameters.Add(new SqlParameter("@pricelist", pricelist == 0 ? 0 : pricelist));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente == null ? string.Empty : codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente == null ? string.Empty : codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora == null ? string.Empty : codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente == null ? string.Empty : codcliente));
                        cmd.Parameters.Add(new SqlParameter("@fechareg", fechareg == null ? string.Empty : fechareg));
                        cmd.Parameters.Add(new SqlParameter("@tmovimiento", tmovimiento == null ? string.Empty : tmovimiento));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConveniosListaPrecio>)context.ConvertTo<BE_ConveniosListaPrecio>(reader);
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
        //public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConveniosPorFiltros(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto)
        //{
        //    ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;

        //    try
        //    {

        //        var response = new List<BE_ConveniosListaPrecio>();

        //        using (SqlConnection conn = new SqlConnection(_cnx))
        //        {
        //            using (SqlCommand cmd = new SqlCommand(SP_GET_FILTRO, conn))
        //            {
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add(new SqlParameter("@codalmacen", codalmacen == null ? string.Empty : codalmacen));
        //                cmd.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento == null ? string.Empty : tipomovimiento));
        //                cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente == null ? string.Empty : codtipocliente));
        //                cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente == null ? string.Empty : codcliente));
        //                cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente == null ? string.Empty : codpaciente));
        //                cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora == null ? string.Empty : codaseguradora));
        //                cmd.Parameters.Add(new SqlParameter("@codcia", codcia == null ? string.Empty : codcia));

        //                conn.Open();

        //                using (var reader = await cmd.ExecuteReaderAsync())
        //                {
        //                    response = (List<BE_ConveniosListaPrecio>)context.ConvertTo<BE_ConveniosListaPrecio>(reader);
        //                }

        //                conn.Close();

        //                vResultadoTransaccion.IdRegistro = 0;
        //                vResultadoTransaccion.ResultadoCodigo = 0;
        //                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
        //                vResultadoTransaccion.dataList = response;
        //            }
        //        }

        //        if (response.Count > 0)
        //        {
        //            ListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository(_clientFactory, _configuration);
        //            ResultadoTransaccion<BE_ListaPrecio> resultadoTransaccionListaPrecio = await listaPrecioRepository.GetPrecioPorCodigo(codproducto, response[0].pricelist);

        //            if (resultadoTransaccionListaPrecio.ResultadoCodigo == -1)
        //            {
        //                vResultadoTransaccion.IdRegistro = -1;
        //                vResultadoTransaccion.ResultadoCodigo = -1;
        //                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListaPrecio.ResultadoDescripcion;
        //                return vResultadoTransaccion;
        //            }

        //            if (((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList).Count > 0)
        //            {
        //                response[0].monto = double.Parse(((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList)[0].Price.ToString());
        //            }
        //            else
        //            {
        //                response[0].monto = 0;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;
        //}


        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Registrar(BE_ConveniosListaPrecio value)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@codalmacen", value.codalmacen));
                        cmd.Parameters.Add(new SqlParameter("@tipomovimiento", value.tipomovimiento));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", value.codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", value.codcliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", value.codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", value.codcia));
                        cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
                        cmd.Parameters.Add(new SqlParameter("@estado", 1));
                        cmd.Parameters.Add(new SqlParameter("@pricelist", value.pricelist));
                        cmd.Parameters.Add(new SqlParameter("@regcreateidusuario", value.regcreateidusuario));
                        cmd.Parameters.Add(new SqlParameter("@regcreate", DateTime.Now));

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

                        vResultadoTransaccion.IdRegistro = 0;
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

        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Modificar(BE_ConveniosListaPrecio value)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
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

                        cmd.Parameters.Add(new SqlParameter("@idconvenio", value.idconvenio));
                        cmd.Parameters.Add(new SqlParameter("@codalmacen", value.codalmacen));
                        cmd.Parameters.Add(new SqlParameter("@tipomovimiento", value.tipomovimiento));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", value.codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", value.codcliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", value.codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", value.codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", value.codcia));
                        cmd.Parameters.Add(new SqlParameter("@moneda", value.moneda));
                        cmd.Parameters.Add(new SqlParameter("@pricelist", value.pricelist));
                        cmd.Parameters.Add(new SqlParameter("@regupdateidusuario", value.regcreateidusuario));
                        cmd.Parameters.Add(new SqlParameter("@regupdate", DateTime.Now));

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

                        vResultadoTransaccion.IdRegistro = 0;
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

        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Eliminar(int idconvenio, int idusuario)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
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

                        cmd.Parameters.Add(new SqlParameter("@idconvenio", idconvenio));
                        cmd.Parameters.Add(new SqlParameter("@idusuario", idusuario));

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

                        vResultadoTransaccion.IdRegistro = 0;
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

        public async Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConveniosPorID(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto)
        {
            ResultadoTransaccion<BE_ConveniosListaPrecio> vResultadoTransaccion = new ResultadoTransaccion<BE_ConveniosListaPrecio>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                var response = new List<BE_ConveniosListaPrecio>();

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codalmacen", codalmacen == null ? string.Empty : codalmacen));
                        cmd.Parameters.Add(new SqlParameter("@tipomovimiento", tipomovimiento == null ? string.Empty : tipomovimiento));
                        cmd.Parameters.Add(new SqlParameter("@codtipocliente", codtipocliente == null ? string.Empty : codtipocliente));
                        cmd.Parameters.Add(new SqlParameter("@codcliente", codcliente == null ? string.Empty : codcliente));
                        cmd.Parameters.Add(new SqlParameter("@codpaciente", codpaciente == null ? string.Empty : codpaciente));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora == null ? string.Empty : codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", codcia == null ? string.Empty : codcia));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ConveniosListaPrecio>)context.ConvertTo<BE_ConveniosListaPrecio>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.dataList = response;
                    }
                }

                if (response.Count > 0)
                {
                    ListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_ListaPrecio> resultadoTransaccionListaPrecio = await listaPrecioRepository.GetPrecioPorCodigo(codproducto, response[0].pricelist);

                    if (resultadoTransaccionListaPrecio.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListaPrecio.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList).Count > 0)
                    {
                        response[0].monto = double.Parse(((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList)[0].Price.ToString());
                    }
                    else
                    {
                        response[0].monto = 0;
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
