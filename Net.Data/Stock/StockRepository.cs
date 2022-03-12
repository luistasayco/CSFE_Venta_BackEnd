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
                var ceros = constock ? "N" : "Y";

                if (codalmacen.Equals("*"))
                {
                    ceros = "Y";
                }

                var codalmacenFind = codalmacen == "*" ? string.Empty : codalmacen;

                var modelo = "sml.svc/SBASTKGParameters(CODITEM='',CODALM='" + codalmacenFind + "',CEROS='" + ceros + "')/SBASTKG";
                var campos = "?$select=* ";
                var orderby = "&$orderby = ItemName";

                var filter = "&$filter = validFor eq 'Y' ";

                if (codalmacen.Equals("*"))
                {
                    filter += "and SellItem eq 'Y' and InvntItem eq 'Y' ";
                } else
                {
                    filter += "and WhsCode eq '" + codalmacenFind + "' and SellItem eq 'Y' and InvntItem eq 'Y' ";
                }

                var filterConStock = string.Empty;

                if (!codalmacen.Equals("*"))
                {
                    if (ceros.Equals("N"))
                    {
                        filterConStock = " and OnHandALM gt 0";
                    }
                    else
                    {
                        filterConStock = " and OnHandALM eq 0";
                    }
                }

                if (!string.IsNullOrEmpty(codproducto))
                {
                    filter = filter + " and ItemCode eq '" + codproducto + "'";
                }

                if (string.IsNullOrEmpty(codproducto) && !string.IsNullOrEmpty(nombre))
                {
                    filter = filter + " and contains (ItemName,'" + nombre + "')";
                }

                if (!codalmacen.Equals("*"))
                {
                    if (constock)
                    {
                        modelo = modelo + campos + filter + filterConStock + orderby;
                    }
                    else
                    {
                        modelo = modelo + campos + filter + filterConStock + orderby;
                    }
                } else
                {
                    modelo = modelo + campos + filter + orderby;
                }
                    

                List<BE_Stock> data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                List<BE_Stock> dataFilter = new List<BE_Stock>();

                if (data.Count > 0)
                {
                    if (!codalmacen.Equals("*"))
                    {
                        if (constock)
                        {
                            foreach (var item in data)
                            {
                                var reserva = item.ReserverdALM == null ? 0 : item.ReserverdALM;
                                var diferencia = item.OnHandALM - reserva;

                                item.Price = item.Price == null ? 0 : item.Price;

                                if (diferencia > 0)
                                {
                                    item.OnHandALM = item.OnHandALM - reserva;
                                    dataFilter.Add(item);
                                }
                            }
                        }
                        else
                        {
                            dataFilter = data;
                        }
                    }
                    else
                    {
                        foreach (BE_Stock item in data)
                        {
                            var isCountExiste = dataFilter.FindAll(xFila => xFila.ItemCode == item.ItemCode).Count;

                            if(isCountExiste == 0 )
                            {
                                item.OnHandALM = item.OnHand;

                                dataFilter.Add(item);
                            }
                        }
                    }
                        
                }
                else
                {
                    dataFilter = data;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Stock>> GetListStockPorProductoAlmacen(string codalmacen, string codproducto, bool constock)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();
                var ceros = constock ? "N" : "Y";

                var modelo = "sml.svc/SBASTKGParameters(CODITEM='" + codproducto + "',CODALM='" + codalmacen + "',CEROS='" + ceros + "')/SBASTKG";
                var campos = "?$select=* ";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' and ItemCode eq '" + codproducto + "'  and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                var filterConStock = string.Empty;

                if (ceros.Equals("N"))
                {
                    filterConStock = " and OnHandALM gt 0";
                }
                else
                {
                    filterConStock = " and OnHandALM eq 0";
                }

                modelo = modelo + campos + filter + filterConStock;

                List<BE_Stock> data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                List<BE_Stock> dataFilter = new List<BE_Stock>();

                if (data.Count > 0)
                {
                    if (constock)
                    {
                        foreach (var item in data)
                        {
                            var reserva = item.ReserverdALM == null ? 0 : item.ReserverdALM;
                            var diferencia = item.OnHandALM - reserva;
                            if (diferencia > 0)
                            {
                                item.OnHandALM = item.OnHandALM - reserva;
                                dataFilter.Add(item);
                            }
                        }
                    }
                    else
                    {
                        dataFilter = data;
                    }
                }
                else
                {
                    dataFilter = data;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                vResultadoTransaccion.dataList = dataFilter;
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

                var ceros = constock ? "N" : "Y";

                var modelo = "sml.svc/SBASTCKParameters(CODITEM='" + codproducto + "',CODALM='" + codalmacen + "',CEROS='"+ ceros + "')/SBASTCK";
                var campos = "?$select= ItemCode, ItemName, BatchNum, QuantityLote, IsCommitedLote, OnOrderLote , ExpDate";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' and ItemCode eq '" + codproducto + "'  and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                var filterConStock = " and QuantityLote gt 0 ";
                var orderby = " &$orderby = ExpDate";

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock + orderby;
                }
                else
                {
                    modelo = modelo + campos + filter + orderby;
                }

                List<BE_StockLote> data = await _connectServiceLayer.GetAsync<BE_StockLote>(modelo);

                List<BE_StockLote> dataFilter = new List<BE_StockLote>();

                if (data.Count > 0)
                {
                    if (constock)
                    {
                        foreach (var item in data)
                        {
                            var reserva = item.ReservedLote == null ? 0 : item.ReservedLote;
                            var diferenciaLote = item.QuantityLote - reserva;

                            if (diferenciaLote > 0)
                            {
                                item.QuantityLote = diferenciaLote;
                                dataFilter.Add(item);
                            }
                        }
                    } else
                    {
                        dataFilter = data;
                    }
                }
                else
                {
                    dataFilter = data;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_StockLote>> GetListStockUbicacionPorFiltro(string codalmacen, string codproducto, bool constock)
        {
            ResultadoTransaccion<BE_StockLote> vResultadoTransaccion = new ResultadoTransaccion<BE_StockLote>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                codproducto = codproducto == null ? "" : codproducto.ToUpper();

                var ceros = constock ? "N" : "Y";

                var modelo = "sml.svc/SBAVSFNParameters(CODITEM='" + codproducto + "',CODALM='" + codalmacen + "',CEROS='" + ceros + "', CODUBI=0)/SBAVSFN";
                var campos = "?$select= ItemCode, ItemName, BatchNum, QuantityLote, IsCommitedLote, OnOrderLote , ExpDate, BinAbs, BinCode, OnHandQty, ReserverdQty, ReservedLote";
                var filter = "&$filter = WhsCode eq '" + codalmacen + "' and ItemCode eq '" + codproducto + "'  and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                var filterConStock = " and OnHandQty gt 0 ";
                var orderby = " &$orderby = ExpDate";

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock + orderby;
                }
                else
                {
                    modelo = modelo + campos + filter + orderby;
                }

                List<BE_StockLote> data = await _connectServiceLayer.GetAsync<BE_StockLote>(modelo);

                List<BE_StockLote> dataFilter = new List<BE_StockLote>();

                if (data.Count > 0)
                {
                    if (constock)
                    {
                        foreach (var item in data)
                        {
                            var reserva = item.ReserverdQty == null ? 0 : item.ReserverdQty;

                            var diferenciaUbicacion = item.OnHandQty - reserva;
                            var diferenciaLote = item.OnHandQty - reserva;

                            if (diferenciaUbicacion > 0)
                            {
                                item.OnHandQty = diferenciaUbicacion;
                                item.QuantityLote = diferenciaLote;
                                dataFilter.Add(item);
                            }
                        }
                    }
                    else
                    {
                        dataFilter = data;
                    }
                }
                else
                {
                    dataFilter = data;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
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
                var ceros = constock ? "N" : "Y";
                var modelo = "sml.svc/SBASTKGParameters(CODITEM='" + codproducto.Trim() + "',CODALM='',CEROS='" + ceros + "')/SBASTKG";
                var campos = "?$select=* ";
                var filter = "&$filter = ItemCode eq '" + codproducto.Trim() + "' and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                var filterConStock = "and OnHandALM gt 0";

                if (constock)
                {
                    modelo = modelo + campos + filter + filterConStock;
                }
                else
                {
                    modelo = modelo + campos + filter;
                }

                List<BE_Stock> data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                List<BE_Stock> dataFilter = new List<BE_Stock>();

                if (data.Count > 0)
                {
                    if (constock)
                    {
                        foreach (var item in data)
                        {
                            var reserva = item.ReserverdALM == null ? 0 : item.ReserverdALM;
                            var diferencia = item.OnHandALM - reserva;
                            if (diferencia > 0)
                            {
                                item.OnHandALM = item.OnHandALM - reserva;
                                dataFilter.Add(item);
                            }
                        }
                    }
                    else
                    {
                        dataFilter = data;
                    }
                }
                else
                {
                    dataFilter = data;
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Stock>> GetListProductoGenericoPorCodigo(string codalmacen, string codprodci, bool constock)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {

                List<BE_Stock> listDCI = await _connectServiceLayer.GetAsync<BE_Stock>("U_SYP_CS_PRODCI?$select=U_SYP_CS_DCI&$filter = Code eq '" + codprodci + "'");

                List<BE_Stock> data = new List<BE_Stock>();
                List<BE_Stock> dataFilter = new List<BE_Stock>();

                if (listDCI.Count > 0)
                {
                    string vCodDCI = listDCI[0].U_SYP_CS_DCI == null ? string.Empty: listDCI[0].U_SYP_CS_DCI;

                    if (!string.IsNullOrEmpty(vCodDCI))
                    {
                        List<BE_Stock> listProdDCI = await _connectServiceLayer.GetAsync<BE_Stock>("U_SYP_CS_PRODCI?$select=Code&$filter = U_SYP_CS_DCI eq '" + vCodDCI + "'");

                        if (listProdDCI.Count > 0)
                        {
                            var filterProDci = string.Empty;

                            foreach (var item in listProdDCI)
                            {
                                if (!string.IsNullOrEmpty(item.Code))
                                {
                                    if (string.IsNullOrEmpty(filterProDci))
                                    {
                                        filterProDci += " U_SYP_CS_PRODCI eq '" + item.Code + "' ";
                                    }
                                    else
                                    {
                                        filterProDci += " or U_SYP_CS_PRODCI eq '" + item.Code + "' ";
                                    }
                                }
                            }

                            var ceros = constock ? "N" : "Y";
                            var modelo = "sml.svc/SBASTKGParameters(CODITEM='',CODALM='"+ codalmacen + "',CEROS='" + ceros + "')/SBASTKG";
                            var campos = "?$select=* ";
                            var filter = "&$filter = " + filterProDci + " and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                            var filterConStock = "and OnHandALM gt 0";

                            if (constock)
                            {
                                modelo = modelo + campos + filter + filterConStock;
                            }
                            else
                            {
                                modelo = modelo + campos + filter;
                            }

                            data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                            if (data.Count > 0)
                            {
                                if (constock)
                                {
                                    foreach (var item in data)
                                    {
                                        var reserva = item.ReserverdALM == null ? 0 : item.ReserverdALM;
                                        var diferencia = item.OnHandALM - reserva;
                                        if (diferencia > 0)
                                        {
                                            item.OnHandALM = item.OnHandALM - reserva;
                                            dataFilter.Add(item);
                                        }
                                    }
                                }
                                else
                                {
                                    dataFilter = data;
                                }
                            }
                            else
                            {
                                dataFilter = data;
                            }
                        }
                    }
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
        public async Task<ResultadoTransaccion<BE_Stock>> GetListProductoGenericoPorDCI(string codalmacen, string coddci, bool constock)
        {
            ResultadoTransaccion<BE_Stock> vResultadoTransaccion = new ResultadoTransaccion<BE_Stock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                List<BE_Stock> data = new List<BE_Stock>();
                List<BE_Stock> dataFilter = new List<BE_Stock>();
                List<BE_Stock> listProdDCI = await _connectServiceLayer.GetAsync<BE_Stock>("U_SYP_CS_PRODCI?$select=Code&$filter = U_SYP_CS_DCI eq '" + coddci + "'");

                if (listProdDCI.Count > 0)
                {
                    var filterProDci = string.Empty;

                    foreach (var item in listProdDCI)
                    {
                        if (!string.IsNullOrEmpty(item.Code))
                        {
                            if (string.IsNullOrEmpty(filterProDci))
                            {
                                filterProDci += " U_SYP_CS_PRODCI eq '" + item.Code + "' ";
                            }
                            else
                            {
                                filterProDci += " or U_SYP_CS_PRODCI eq '" + item.Code + "' ";
                            }
                        }
                    }

                    var ceros = constock ? "N" : "Y";
                    var modelo = "sml.svc/SBASTKGParameters(CODITEM='',CODALM='" + codalmacen + "',CEROS='" + ceros + "')/SBASTKG";
                    var campos = "?$select=* ";
                    var filter = "&$filter = " + filterProDci + " and SellItem eq 'Y' and InvntItem eq 'Y' and validFor eq 'Y' ";
                    var filterConStock = "and OnHandALM gt 0";

                    if (constock)
                    {
                        modelo = modelo + campos + filter + filterConStock;
                    }
                    else
                    {
                        modelo = modelo + campos + filter;
                    }

                    data = await _connectServiceLayer.GetAsync<BE_Stock>(modelo);

                    if (data.Count > 0)
                    {
                        if (constock)
                        {
                            foreach (var item in data)
                            {
                                var reserva = item.ReserverdALM == null ? 0 : item.ReserverdALM;
                                var diferencia = item.OnHandALM - reserva;
                                if (diferencia > 0)
                                {
                                    item.OnHandALM = item.OnHandALM - reserva;
                                    dataFilter.Add(item);
                                }
                            }
                        }
                        else
                        {
                            dataFilter = data;
                        }
                    }
                    else
                    {
                        dataFilter = data;
                    }
                }

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = dataFilter;
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
