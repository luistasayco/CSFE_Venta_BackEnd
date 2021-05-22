using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;

namespace Net.Data
{
    public class StockRepository: IStockRepository
    {
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public StockRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<BE_Stock>> GetListStockPorFiltro(string codalmacen, string nombre, string codproducto, bool constock)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                nombre = nombre == null ? "" : nombre.ToUpper();
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var modelo = "sml.svc/SBASTKG";
                var campos = "?$select=* ";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' ";
                var filterConStock = "and OnHand_1 gt 0";


                if (!string.IsNullOrEmpty(codproducto))
                {
                    filter = filter + " and ItemCode eq '" + codproducto + "'";
                }

                if (string.IsNullOrEmpty(codproducto) && !string.IsNullOrEmpty(nombre))
                {
                    filter = filter + " and contains (ItemName,'" + nombre + "')";
                }

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock;
                }
                else
                {
                    modelo = modelo + campos + filter;
                }

                List<BE_Stock> data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_Stock>> GetListStockPorProductoAlmacen(string codalmacen, string codproducto)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var modelo = "sml.svc/SBASTKG";
                var campos = "?$select=* ";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' and ItemCode eq '" + codproducto + "'";
                var filterConStock = "and OnHand_1 gt 0";

                modelo = modelo + campos + filter + filterConStock;

                BE_Stock data = await _connectServiceLayer.GetAsyncTo<BE_Stock>(modelo);

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

        public async Task<ResultadoTransaccion<BE_StockLote>> GetListStockLotePorFiltro(string codalmacen, string codproducto, bool constock)
        {
            ResultadoTransaccion<BE_StockLote> vResultadoTransaccion = new ResultadoTransaccion<BE_StockLote>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var modelo = "sml.svc/SBASTCK";
                var campos = "?$select= ItemCode, ItemName, BatchNum, Quantity, IsCommited_2, OnOrder_2 ";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' and ItemCode eq '" + codproducto + "' ";
                var filterConStock = "and Quantity gt 0";

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock;
                }
                else
                {
                    modelo = modelo + campos + filter;
                }

                List<BE_StockLote> data = await _connectServiceLayer.GetAsync<BE_StockLote>(modelo);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Stock>> GetListStockPorProducto(string codproducto, bool constock)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var modelo = "sml.svc/SBASTKG";
                var campos = "?$select=* ";
                var filter = "&$filter = ItemCode eq '" + codproducto + "' ";
                var filterConStock = "and OnHand_1 gt 0";

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock;
                }
                else
                {
                    modelo = modelo + campos + filter;
                }

                List<BE_Stock> data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
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
