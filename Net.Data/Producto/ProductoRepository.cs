using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Net.Connection;

namespace Net.Data
{
    public class ProductoRepository : RepositoryBase<BE_Producto>, IProductoRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        const string DB_ESQUEMA = "";
        const string SP_GET_ALTERNATIVO_POR_FILTRO = DB_ESQUEMA + "VEN_ListaAlternativoxProductoPorFiltroGet";

        public ProductoRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IConnectionSQL context)
             : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        //public async Task<ResultadoTransaccion<BE_Producto>> GetListProductoPorFiltro(string codalmacen, string codigo, string nombre, string codaseguradora, string codcia, bool conStock = true)
        //{
        //    ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;
        //    try
        //    {
        //        codigo = codigo == null ? "" : codigo.ToUpper();
        //        nombre = nombre == null ? "" : nombre.ToUpper();

        //        var cadena = "Items";
        //        var filter = "";
        //        var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, QuantityOnStock, AvgStdPrice, U_SYP_CS_FAMILIA, U_SYP_CS_SIC, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART";
        //        var filterConStock = " and QuantityOnStock gt 0";

        //        if (!string.IsNullOrEmpty(codigo))
        //        {
        //            filter = "&$filter =U_SYP_CS_SIC eq '" + codigo + "'";
        //        }

        //        if (string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
        //        {
        //            filter = "&$filter = contains (ItemName,'" + nombre + "')";
        //        }

        //        if (conStock)
        //        {
        //            cadena = cadena + campos + filter + filterConStock;
        //        }
        //        else
        //        {
        //            cadena = cadena + campos + filter;
        //        }

        //        List<BE_Producto> data = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

        //        if (!string.IsNullOrEmpty(codaseguradora) && !string.IsNullOrEmpty(codcia))
        //        {
        //            for (int i = 0; i < data.Count; i++)
        //            {
        //                var dataProducto = data[i];
        //                ResultadoTransaccion<BE_Producto> dataAlternativo = await GetListProductoAlternativoPorCodigo(dataProducto.codproducto, codaseguradora, codcia);

        //                if (((List<BE_Producto>)dataAlternativo.dataList).Count > 0)
        //                {
        //                    data[i].flgrestringido = true;
        //                }
        //            }
        //        }

        //        vResultadoTransaccion.IdRegistro = 0;
        //        vResultadoTransaccion.ResultadoCodigo = 0;
        //        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
        //        vResultadoTransaccion.dataList = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}
        //public async Task<ResultadoTransaccion<BE_Producto>> GetListProductoGenericoPorCodigo(string codigo, bool conStock = true)
        //{
        //    ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
        //    _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

        //    vResultadoTransaccion.NombreMetodo = _metodoName;
        //    vResultadoTransaccion.NombreAplicacion = _aplicacionName;
        //    try
        //    {
        //        codigo = codigo == null ? "" : codigo.ToUpper();

        //        var cadena = "Items";
        //        var filter = "";
        //        var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, QuantityOnStock, AvgStdPrice, U_SYP_CS_FAMILIA, U_SYP_CS_SIC, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART";
        //        var filterConStock = " and QuantityOnStock gt 0";

        //        if (!string.IsNullOrEmpty(codigo))
        //        {
        //            filter = "&$filter = U_SYP_CS_FAMILIA eq '" + codigo + "'";
        //        }

        //        if (conStock)
        //        {
        //            cadena = cadena + campos + filter + filterConStock;
        //        }
        //        else
        //        {
        //            cadena = cadena + campos + filter;
        //        }

        //        List<BE_Producto> data = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

        //        vResultadoTransaccion.IdRegistro = 0;
        //        vResultadoTransaccion.ResultadoCodigo = 0;
        //        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
        //        vResultadoTransaccion.dataList = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        vResultadoTransaccion.IdRegistro = -1;
        //        vResultadoTransaccion.ResultadoCodigo = -1;
        //        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
        //    }

        //    return vResultadoTransaccion;

        //}

        public async Task<ResultadoTransaccion<BE_Producto>> GetListProductoAlternativoPorCodigo(string codproducto, string codaseguradora, string codcia)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Producto>();

                    using (SqlCommand cmd = new SqlCommand(SP_GET_ALTERNATIVO_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codcia", codcia));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Producto>)context.ConvertTo<BE_Producto>(reader);
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

        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoPorCodigo(string codalmacen, string codproducto, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var cadena = "Items";
                var filter = "&$filter=ItemCode eq '" + codproducto + "'";
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, U_SYP_FAMILIA, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART, ItemsGroupCode, ManageBatchNumbers, ArTaxCode, Properties1 ";

                cadena = cadena + campos + filter;

                BE_Producto data = await _connectServiceLayer.GetAsyncTo<BE_Producto>(cadena);

                if (!string.IsNullOrEmpty(codaseguradora) && !string.IsNullOrEmpty(codcia))
                {
                    ResultadoTransaccion<BE_Producto> dataAlternativo = await GetListProductoAlternativoPorCodigo(codproducto, codaseguradora, codcia);

                    if (dataAlternativo.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = dataAlternativo.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<BE_Producto>)dataAlternativo.dataList).Count > 0)
                    {
                        data.flgrestringido = true;
                    }
                }

                if (data.ManageBatchNumbers.Equals("tYes"))
                {
                    StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_StockLote> resultadoTransaccionStockLote = await stockRepository.GetListStockLotePorFiltro(codalmacen, codproducto, true);

                    if (resultadoTransaccionStockLote.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStockLote.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    data.ListStockLote = (List<BE_StockLote>)resultadoTransaccionStockLote.dataList;
                }

                IGVRepository iGVRepository = new IGVRepository(_clientFactory, _configuration);
                ResultadoTransaccion<BE_IGV> resultadoTransaccionIGV = await iGVRepository.GetIGVPorCodigo(data.ArTaxCode);

                if (resultadoTransaccionIGV.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionIGV.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (((List<BE_IGV>)resultadoTransaccionIGV.dataList).Count > 0)
                {
                    data.valorIGV = ((List<BE_IGV>)resultadoTransaccionIGV.dataList)[0].Rate;
                } else
                {
                    data.valorIGV = 0;
                }

                ConveniosRepository conveniosRepository = new ConveniosRepository(_clientFactory, context, _configuration);

                ResultadoTransaccion<BE_Convenios> resultadoTransaccionConvenios = new ResultadoTransaccion<BE_Convenios>();

                if (codtipocliente.Equals("01"))
                {
                    resultadoTransaccionConvenios = await conveniosRepository.GetConveniosPorFiltros(null, tipomovimiento, codtipocliente, null, codpaciente, codaseguradora, codcia, codproducto);
                } else
                {
                    resultadoTransaccionConvenios = await conveniosRepository.GetConveniosPorFiltros(null, tipomovimiento, codtipocliente, null, null, null, null, codproducto);
                }

                if (resultadoTransaccionConvenios.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionConvenios.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (((List<BE_Convenios>)resultadoTransaccionConvenios.dataList).Count == 0)
                {
                    ListaPrecioRepository listaPrecioRepository = new ListaPrecioRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_ListaPrecio> resultadoTransaccionListaPrecio = await listaPrecioRepository.GetPrecioPorCodigo(codproducto, 1);

                    if (resultadoTransaccionListaPrecio.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListaPrecio.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList).Count > 0)
                    {
                        data.valorVVP = ((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList)[0].Price;
                    }
                    else
                    {
                        data.valorVVP = 0;
                    }

                } else
                {
                    if (((List<BE_Convenios>)resultadoTransaccionConvenios.dataList)[0].tipomonto.Equals("M"))
                    {
                        data.valorVVP = decimal.Parse(((List<BE_Convenios>)resultadoTransaccionConvenios.dataList)[0].monto.ToString());
                        data.FlgConvenio = true;
                    }
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.data = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoyStockAlmacenesPorCodigo(string codproducto)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var cadena = "Items";
                var filter = "&$filter=ItemCode eq '" + codproducto + "'";
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, U_SYP_FAMILIA, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART, ItemsGroupCode ";

                cadena = cadena + campos + filter;

                BE_Producto data = await _connectServiceLayer.GetAsyncTo<BE_Producto>(cadena);

                StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
                ResultadoTransaccion<BE_Stock> resultadoTransaccionStock = await stockRepository.GetListStockPorProducto(codproducto, true);

                if (resultadoTransaccionStock.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStock.ResultadoDescripcion;

                    return vResultadoTransaccion;
                }

                data.ListStockAlmacen = (List<BE_Stock>)resultadoTransaccionStock.dataList;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.data = data;
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
