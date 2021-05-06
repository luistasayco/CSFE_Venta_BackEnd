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
    public class ProductoRepository: IProductoRepository
    {

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public ProductoRepository(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<BE_Producto>> GetListProductoPorFiltro(string codigo, string nombre, bool conStock = true)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codigo = codigo == null ? "" : codigo.ToUpper();
                nombre = nombre == null ? "" : nombre.ToUpper();

                var cadena = "Items";
                var filter = "";
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, QuantityOnStock, AvgStdPrice, U_SYP_CS_FAMILIA";
                var filterConStock = " and QuantityOnStock gt 0";

                if (!string.IsNullOrEmpty(codigo))
                {
                    filter = "&$filter =U_SYP_CS_SIC eq '" + codigo + "'";
                }

                if (string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
                {
                    filter = "&$filter = contains (ItemName,'" + nombre + "')";
                }

                if (conStock)
                {
                    cadena = cadena + campos + filter + filterConStock;
                }
                else
                {
                    cadena = cadena + campos + filter;
                }

                List<BE_Producto> data = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

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
        public async Task<ResultadoTransaccion<BE_Producto>> GetListProductoGenericoPorCodigo(string codigo, bool conStock = true)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codigo = codigo == null ? "" : codigo.ToUpper();

                var cadena = "Items";
                var filter = "";
                var campos = "?$select=ItemCode, ItemName, U_SYP_CS_LABORATORIO, QuantityOnStock, AvgStdPrice, U_SYP_CS_FAMILIA";
                var filterConStock = " and QuantityOnStock gt 0";

                if (!string.IsNullOrEmpty(codigo))
                {
                    filter = "&$filter = U_SYP_CS_FAMILIA eq '" + codigo + "'";
                }

                if (conStock)
                {
                    cadena = cadena + campos + filter + filterConStock;
                }
                else
                {
                    cadena = cadena + campos + filter;
                }

                List<BE_Producto> data = await _connectServiceLayer.GetAsync<BE_Producto>(cadena);

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
        public async Task<ResultadoTransaccion<BE_Producto>> GetProductoInsertarVentaPorFiltro(string codatencion)
        {
            ResultadoTransaccion<BE_Producto> vResultadoTransaccion = new ResultadoTransaccion<BE_Producto>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
               


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
