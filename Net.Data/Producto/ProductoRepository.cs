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
using System.Linq;

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
        const string SP_GET_AseguradoraxProductoGNC_ConsultaV2 = DB_ESQUEMA + "VEN_AseguradoraxProductoGNC_ConsultaV2";
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
        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoPorCodigo(string codalmacen, string codproducto, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion, bool constock)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var cadena = "Items";
                var filter = "&$filter=ItemCode eq '" + codproducto + "' InventoryItem eq 'tYES' and SalesItem eq 'tYES' and Valid eq 'tYES'";
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, U_SYP_FAMILIA, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART, ItemsGroupCode, ManageBatchNumbers, ArTaxCode, Properties1, U_SYP_CS_DCTO, U_SYP_CS_FRVTA, U_SYP_CS_PRODCI ";

                cadena = cadena + campos + filter;

                List<BE_Producto> listProductos = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

                if (listProductos.Any())
                {

                    BE_Producto data = listProductos[0];

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
                    }
                    else
                    {
                        data.valorIGV = 0;
                    }

                    StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
                    ResultadoTransaccion<BE_Stock> resultadoTransaccionStock = await stockRepository.GetListStockPorProductoAlmacen(codalmacen, codproducto, constock);

                    if (resultadoTransaccionStock.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStock.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    if (resultadoTransaccionStock.dataList.Any())
                    {
                        BE_Stock stockProducto = ((List<BE_Stock>)resultadoTransaccionStock.dataList)[0];

                        data.ProductoStock = stockProducto.OnHandALM == null ? 0 : (decimal)stockProducto.OnHandALM;
                        data.valorVVP = stockProducto.Price == null ? 0 : (decimal)stockProducto.Price;
                    }

                    ConveniosRepository conveniosRepository = new ConveniosRepository(_clientFactory, context, _configuration);

                    ResultadoTransaccion<BE_ConveniosListaPrecio> resultadoTransaccionConvenios = new ResultadoTransaccion<BE_ConveniosListaPrecio>();

                    if (codtipocliente.Equals("01"))
                    {
                        resultadoTransaccionConvenios = await conveniosRepository.GetConveniosPorFiltros(null, tipomovimiento, codtipocliente, null, codpaciente, codaseguradora, codcia, codproducto);
                    }
                    else
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

                    if (((List<BE_ConveniosListaPrecio>)resultadoTransaccionConvenios.dataList).Count == 0)
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

                        BE_ListaPrecio precio = ((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList)[0];

                        if (((List<BE_ListaPrecio>)resultadoTransaccionListaPrecio.dataList).Count > 0)
                        {
                            var datap = precio.Price == null ? 0 : precio.Price;

                            data.valorVVP = (decimal)datap;
                        }
                        else
                        {
                            data.valorVVP = 0;
                        }

                    }
                    else
                    {
                        BE_ConveniosListaPrecio conveniosListaPrecio = ((List<BE_ConveniosListaPrecio>)resultadoTransaccionConvenios.dataList)[0];

                        if (!conveniosListaPrecio.monto.Equals(0))
                        {
                            data.valorVVP = decimal.Parse(conveniosListaPrecio.monto.ToString());
                        }
                        data.FlgConvenio = true;
                    }

                    VentaRepository ventaRepository = new VentaRepository(_clientFactory, context, _configuration);
                    ResultadoTransaccion<bool> resultadoTransaccionGastoCubierto = await ventaRepository.GetGastoCubiertoPorFiltro(codaseguradora, codproducto, tipoatencion);

                    data.GastoCubierto = false;

                    if (resultadoTransaccionGastoCubierto.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionGastoCubierto.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    data.GastoCubierto = resultadoTransaccionGastoCubierto.data;

                    ResultadoTransaccion<BE_Producto> resultadoTransaccionListProductoAlternativo = await GetListProductoAlternativoPorCodigo(codproducto, codaseguradora, codcia);

                    if (resultadoTransaccionListProductoAlternativo.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionListProductoAlternativo.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    data.ProductoRestringido = false;

                    if (resultadoTransaccionListProductoAlternativo.dataList.Any())
                    {
                        data.ProductoRestringido = true;
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = data;
                    vResultadoTransaccion.dataList = listProductos;
                }
                else
                {
                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Producto no existe", 0);
                    vResultadoTransaccion.data = null;
                    vResultadoTransaccion.dataList = listProductos;
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
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, U_SYP_FAMILIA, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART, ItemsGroupCode, Properties1, ManageBatchNumbers, U_SYP_CS_PRODCI ";

                cadena = cadena + campos + filter;

                List<BE_Producto> listproducto = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

                BE_Producto data = new BE_Producto();

                if (listproducto.Count > 0)
                {

                    data = listproducto[0];

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
                } else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Producto no existe en SAP HANA";
                    return vResultadoTransaccion;
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
        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoByCodigo(string codproducto)
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
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, U_SYP_FAMILIA, U_SYP_CS_EABAS, U_SYP_CS_CLASIF, U_SYP_MONART, ItemsGroupCode, Properties1, ManageBatchNumbers, U_SYP_CS_PRODCI ";

                cadena = cadena + campos + filter;

                List<BE_Producto> listproducto = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

                BE_Producto data = new BE_Producto();
                data = listproducto[0];

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
        public async Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorPedido(string codpedido, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {

                List<BE_Producto> listProductos = new List<BE_Producto>();

                PedidoRepository pedidoRepository = new PedidoRepository(context, _configuration, _clientFactory);
                ResultadoTransaccion<BE_PedidoDetalle> resultadoTransaccionDetallePedido = await pedidoRepository.GetListPedidoDetallePorPedido(codpedido);

                if (resultadoTransaccionDetallePedido.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetallePedido.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionDetallePedido.dataList.Any())
                {

                    List<BE_PedidoDetalle> listPedidoDetalle = (List<BE_PedidoDetalle>)resultadoTransaccionDetallePedido.dataList;

                    string mensajeProductosSinStock = string.Empty;
                    decimal cantidadPicking = 0;

                    foreach (BE_PedidoDetalle item in listPedidoDetalle)
                    {
                        ResultadoTransaccion<BE_Producto> resultadoTransaccionProducto = await GetProductoPorCodigo(codalmacen, item.codproducto, codaseguradora, codcia, tipomovimiento, codtipocliente, codcliente, codpaciente, tipoatencion, true);

                        if (resultadoTransaccionProducto.ResultadoCodigo == -1 && resultadoTransaccionProducto.IdRegistro == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProducto.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        if (resultadoTransaccionProducto.IdRegistro == 0 && resultadoTransaccionProducto.ResultadoCodigo == -1)
                        {

                        } else
                        {

                            ResultadoTransaccion<BE_Pedido> resultadoTransaccionDatosPedido = await pedidoRepository.GetDatosPedidoPorPedido(codpedido);

                            if (resultadoTransaccionDatosPedido.ResultadoCodigo == -1)
                            {
                                vResultadoTransaccion.IdRegistro = -1;
                                vResultadoTransaccion.ResultadoCodigo = -1;
                                vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDatosPedido.ResultadoDescripcion;
                                return vResultadoTransaccion;
                            }

                            BE_Pedido bE_PedidoDatos = ((List< BE_Pedido>)resultadoTransaccionDatosPedido.dataList)[0];
                            resultadoTransaccionProducto.data.ListStockLote = new List<BE_StockLote>();

                            if (bE_PedidoDatos.indicadorpicking == 2)
                            {
                                ConsolidadoRepository consolidadoRepository = new ConsolidadoRepository(_clientFactory, context, _configuration);
                                ResultadoTransaccion<BE_ConsolidadoPedidoPicking> resultadoTransaccionPicking = await consolidadoRepository.GetListConsolidadoIndividualPicking(codpedido, item.codproducto);

                                cantidadPicking = 0;

                                if (resultadoTransaccionPicking.ResultadoCodigo == -1)
                                {
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPicking.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }

                                resultadoTransaccionProducto.data.ListStockLote = new List<BE_StockLote>();

                                if (resultadoTransaccionPicking.dataList.Any())
                                {

                                    foreach (BE_ConsolidadoPedidoPicking itemPicking in resultadoTransaccionPicking.dataList)
                                    {
                                        var stock = new BE_StockLote
                                        {
                                            ItemCode = item.codproducto,
                                            ItemName = item.nombreproducto,
                                            BatchNum = itemPicking.lote,
                                            QuantityLote = 0,
                                            Quantityinput = itemPicking.cantidadpicking,
                                            ExpDate = itemPicking.fechavencimiento,
                                            BinAbs = itemPicking.ubicacion,
                                            BinCode = itemPicking.nombreubicacion
                                        };

                                        cantidadPicking += itemPicking.cantidadpicking;

                                        resultadoTransaccionProducto.data.ListStockLote.Add(stock);
                                    }

                                    resultadoTransaccionProducto.data.CantidadPedido = cantidadPicking;

                                    resultadoTransaccionProducto.data.CodPedido = item.codpedido;
                                    resultadoTransaccionProducto.data.binActivat = item.binactivat;

                                    listProductos.Add(resultadoTransaccionProducto.data);

                                    if ((decimal)item.cantidad != cantidadPicking)
                                    {
                                        mensajeProductosSinStock += string.Format("Producto {0} - {1}, Cantidad de pedido {2} y Cantidad picking {3} <br>", item.codproducto, resultadoTransaccionProducto.data.ItemName, item.cantidad, cantidadPicking);
                                    }

                                } else
                                {
                                    mensajeProductosSinStock += string.Format("Producto {0} - {1} no se ha realizado picking en el almacen: {2} <br>", item.codproducto, resultadoTransaccionProducto.data.ItemName, codalmacen);
                                    resultadoTransaccionProducto.data.CantidadPedido = resultadoTransaccionProducto.data.ProductoStock;
                                }
                            } else
                            {
                                if (resultadoTransaccionProducto.data.ProductoStock < (decimal)item.cantidad)
                                {
                                    mensajeProductosSinStock += string.Format("Producto {0} - {1} no tiene stock en el almacen: {2} <br>", item.codproducto, resultadoTransaccionProducto.data.ItemName, codalmacen);
                                    resultadoTransaccionProducto.data.CantidadPedido = resultadoTransaccionProducto.data.ProductoStock;
                                }
                                else
                                {
                                    resultadoTransaccionProducto.data.CantidadPedido = (decimal)item.cantidad;
                                }

                                resultadoTransaccionProducto.data.CantidadPedido = (decimal)item.cantidad;
                                resultadoTransaccionProducto.data.CodPedido = item.codpedido;
                                resultadoTransaccionProducto.data.binActivat = item.binactivat;

                                listProductos.Add(resultadoTransaccionProducto.data);
                            }
                        }
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = mensajeProductosSinStock;
                    vResultadoTransaccion.dataList = listProductos;

                } 
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "No existe detalle para el pedido nro. " + codpedido;
                    return vResultadoTransaccion;
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
        public async Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorReceta(int idereceta, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {

                List<BE_Producto> listProductos = new List<BE_Producto>();

                RecetaRepository recetaRepository = new RecetaRepository(context, _configuration);
                ResultadoTransaccion<BE_RecetaDetalle> resultadoTransaccionDetalleReceta = await recetaRepository.GetListRecetaDetallePorReceta(idereceta);

                if (resultadoTransaccionDetalleReceta.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetalleReceta.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                WarehousesRepository warehousesRepository = new WarehousesRepository(_clientFactory, _configuration);
                BE_Warehouses resultadoTransaccionWarehouse = await warehousesRepository.GetWarehousesPorCodigo(codalmacen);

                var binactivat = resultadoTransaccionWarehouse.EnableBinLocations == "tYES" ? true : false;

                if (resultadoTransaccionDetalleReceta.dataList.Any())
                {

                    PickingRepository pickingRepository = new PickingRepository(_clientFactory, context, _configuration);
                    ResultadoTransaccion<BE_Picking> resultadoTransaccionPickingReceta = await pickingRepository.GetListPickingPorReceta(idereceta);
                    if (resultadoTransaccionPickingReceta.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPickingReceta.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    List<BE_Picking> listPicking = (List<BE_Picking>)resultadoTransaccionPickingReceta.dataList;

                    int countPicking = listPicking.FindAll(xFila => xFila.estado == 1).Count;

                    List<BE_RecetaDetalle> lisDetalleReceta = (List<BE_RecetaDetalle>)resultadoTransaccionDetalleReceta.dataList;
                    decimal cantidadPicking = 0;

                    foreach (BE_RecetaDetalle item in lisDetalleReceta)
                    {
                        ResultadoTransaccion<BE_Producto> resultadoTransaccionProducto = await GetProductoPorCodigo(codalmacen, item.codproducto, codaseguradora, codcia, tipomovimiento, codtipocliente, codcliente, codpaciente, tipoatencion, true);

                        if (resultadoTransaccionProducto.ResultadoCodigo == -1 && resultadoTransaccionProducto.IdRegistro == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProducto.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        if (resultadoTransaccionProducto.IdRegistro == 0 && resultadoTransaccionProducto.ResultadoCodigo == -1)
                        {

                        }
                        else
                        {

                            resultadoTransaccionProducto.data.ListStockLote = new List<BE_StockLote>();

                            if (countPicking > 0)
                            {
                                resultadoTransaccionProducto.data.pickingreceta = true;

                                ResultadoTransaccion<BE_Picking> resultadoTransaccionPicking = await pickingRepository.GetListPickingPorRecetaProductoAlmacen(idereceta, item.codproducto, codalmacen);

                                if (resultadoTransaccionPicking.ResultadoCodigo == -1)
                                {
                                    vResultadoTransaccion.IdRegistro = -1;
                                    vResultadoTransaccion.ResultadoCodigo = -1;
                                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionPicking.ResultadoDescripcion;
                                    return vResultadoTransaccion;
                                }
                                cantidadPicking = 0;
                                // Si existe picking
                                if (resultadoTransaccionPicking.dataList.Any())
                                {
                                    foreach (BE_Picking itemPicking in resultadoTransaccionPicking.dataList)
                                    {
                                        var stock = new BE_StockLote
                                        {
                                            ItemCode = item.codproducto,
                                            ItemName = item.nombreproducto,
                                            BatchNum = itemPicking.lote,
                                            QuantityLote = 0,
                                            Quantityinput = itemPicking.cantidadpicking,
                                            ExpDate = itemPicking.fechavencimiento,
                                            BinAbs = itemPicking.ubicacion,
                                            BinCode = itemPicking.nombreubicacion
                                        };
                                        cantidadPicking += itemPicking.cantidadpicking;
                                        resultadoTransaccionProducto.data.ListStockLote.Add(stock);
                                    }

                                    resultadoTransaccionProducto.data.CantidadPedido = cantidadPicking;
                                    resultadoTransaccionProducto.data.binActivat = binactivat;
                                    listProductos.Add(resultadoTransaccionProducto.data);
                                }
                            } else
                            {
                                resultadoTransaccionProducto.data.CantidadPedido = (decimal)item.cantidad;
                                resultadoTransaccionProducto.data.binActivat = binactivat;
                                resultadoTransaccionProducto.data.pickingreceta = false;
                                listProductos.Add(resultadoTransaccionProducto.data);
                            }
                        }
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", listProductos.Count);
                    vResultadoTransaccion.dataList = listProductos;

                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "No existe detalle para la receta nro. " + idereceta.ToString();
                    return vResultadoTransaccion;
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

        public async Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorIdBorrador(int idborrador, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {

                List<BE_Producto> listProductos = new List<BE_Producto>();

                SalaOperacionRepository salaOperacionRepository = new SalaOperacionRepository(_clientFactory,context, _configuration);
                ResultadoTransaccion<BE_SalaOperacionDetalle> resultadoTransaccionDetalle = await salaOperacionRepository.GetListSalaOperacionDetallePorId(idborrador);

                if (resultadoTransaccionDetalle.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionDetalle.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionDetalle.dataList.Any())
                {

                    List<BE_SalaOperacionDetalle> lisDetalle = (List<BE_SalaOperacionDetalle>)resultadoTransaccionDetalle.dataList;


                    foreach (BE_SalaOperacionDetalle item in lisDetalle)
                    {
                        ResultadoTransaccion<BE_Producto> resultadoTransaccionProducto = await GetProductoPorCodigo(codalmacen, item.codproducto, codaseguradora, codcia, tipomovimiento, codtipocliente, codcliente, codpaciente, tipoatencion, true);

                        if (resultadoTransaccionProducto.ResultadoCodigo == -1 && resultadoTransaccionProducto.IdRegistro == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProducto.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        ResultadoTransaccion<BE_StockLote> resultadoTransaccionStockLote = await salaOperacionRepository.GetListSalaOperacionDetalleLoteUbiPorId(item.idborradordetalle);

                        if (resultadoTransaccionStockLote.ResultadoCodigo == -1)
                        {
                            vResultadoTransaccion.IdRegistro = -1;
                            vResultadoTransaccion.ResultadoCodigo = -1;
                            vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStockLote.ResultadoDescripcion;
                            return vResultadoTransaccion;
                        }

                        resultadoTransaccionProducto.data.ListStockLote = (List<BE_StockLote>)resultadoTransaccionStockLote.dataList;

                        if (resultadoTransaccionProducto.IdRegistro == 0 && resultadoTransaccionProducto.ResultadoCodigo == -1)
                        {

                        }
                        else
                        {
                            resultadoTransaccionProducto.data.CantidadPedido = (decimal)item.cantidad;
                            //resultadoTransaccionProducto.data.CodPedido = item.codpedido;

                            listProductos.Add(resultadoTransaccionProducto.data);
                        }

                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", listProductos.Count);
                    vResultadoTransaccion.dataList = listProductos;

                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "No existe detalle para el borrador nro. " + idborrador.ToString();
                    return vResultadoTransaccion;
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

        public async Task<ResultadoTransaccion<BE_AseguradoraxProducto>> GetProductoPorCodigoAseguradora(string codaseguradora, string codproducto, int codtipoatencion_mae, int orden)
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

                    using (SqlCommand cmd = new SqlCommand(SP_GET_AseguradoraxProductoGNC_ConsultaV2, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codaseguradora", codaseguradora));
                        cmd.Parameters.Add(new SqlParameter("@codproducto", codproducto));
                        cmd.Parameters.Add(new SqlParameter("@codtipoatencion_mae", codtipoatencion_mae));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

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

        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoPorFiltro(string nombreProducto, string nombreFamilia, string nombreLaboratorio)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                string filter = string.Empty;
                nombreProducto = nombreProducto == null ? "" : nombreProducto.Trim().ToUpper();
                nombreFamilia = nombreFamilia == null ? "" : nombreFamilia.Trim().ToUpper();
                nombreLaboratorio = nombreLaboratorio == null ? "" : nombreLaboratorio.Trim().ToUpper();

                var cadena = "Items?$select=ItemCode, ItemName,ManageBatchNumbers,Properties1 &$filter= InventoryItem eq 'tYES' and SalesItem eq 'tYES' and Valid eq 'tYES' ";

                if (nombreFamilia != string.Empty)
                {
                    cadena += " and U_SYP_FAMILIA eq '" + nombreFamilia + "'";
                }

                if (nombreLaboratorio != string.Empty)
                {
                    cadena += " and U_SYP_CS_LABORATORIO eq '" + nombreLaboratorio + "'";
                }

                if (nombreProducto != string.Empty)
                {
                    cadena += " and startswith(ItemName,'" + nombreProducto + "')";
                }

                //if (tipo.Equals("FAMILIA"))
                //{
                //    filter = "&$filter=U_SYP_FAMILIA eq '" + filtro;
                //}
                //else if (tipo.Equals("LABORATORIO"))
                //{
                //    filter = "&$filter=U_SYP_CS_LABORATORIO eq '" + filtro;
                //}
                //else if (tipo.Equals("PRODUCTO"))
                //{
                //    cadena += "&$filter=startswith(ItemName,'" + filtro + "')";
                //}

                //U_SYP_FAMILIA
                //U_SYP_CS_LABORATORIO
                List<BE_Producto> listproducto = await _connectServiceLayer.GetBaseAsync<BE_Producto>(cadena, 200);

                BE_Producto data = new BE_Producto();

                if (listproducto.Count < 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "El producto no existe en SAP HANA";
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.dataList = listproducto;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_Stock>> GetProductoPorCodigoBarra(string codalmacen, string codigoBarra)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codalmacen = codalmacen == null ? "" : codalmacen.Trim().ToUpper();
                codigoBarra = codigoBarra == null ? "" : codigoBarra.Trim().ToUpper();

                string query = string.Empty;
                string vista = string.Empty;
                string Select = string.Empty;
                string filter = string.Empty;

                string codProducto = codigoBarra.Substring(0, 8).Trim();

                ResultadoTransaccion< BE_Producto> resultadoTransaccionProducto = await GetProductoByCodigo(codProducto);

                if (resultadoTransaccionProducto.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionProducto.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                if (resultadoTransaccionProducto.data.manbtchnum)
                {
                    if (codigoBarra.Length == 8)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Producto tiene identificador de Lote, código de barra incorrecto";
                        return vResultadoTransaccion;
                    }
                }

                StockRepository stockRepository = new StockRepository(_clientFactory, _configuration);
                List<BE_Stock> listproducto = new List<BE_Stock>();

                if (codigoBarra.Length <= 8)
                {

                    ResultadoTransaccion<BE_Stock> resultadoTransaccionStockProducto = await stockRepository.GetListStockPorProductoAlmacen(codalmacen, codProducto, true);

                    if (resultadoTransaccionStockProducto.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStockProducto.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    listproducto = (List<BE_Stock>)resultadoTransaccionStockProducto.dataList;
                }
                else
                {
                    string codLote = Microsoft.VisualBasic.Strings.Right(codigoBarra, (codigoBarra.Length) - (codProducto.Length + 1)).Trim();

                    ResultadoTransaccion<BE_Stock> resultadoTransaccionStockProductoLote = await stockRepository.GetListStockLotePorFiltro(codalmacen, codProducto, codLote, true);

                    if (resultadoTransaccionStockProductoLote.ResultadoCodigo == -1)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccionStockProductoLote.ResultadoDescripcion;
                        return vResultadoTransaccion;
                    }

                    listproducto = (List<BE_Stock>)resultadoTransaccionStockProductoLote.dataList;
                }

                if (listproducto.Count < 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "El producto no existe en SAP HANA";
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.dataList = listproducto;
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
