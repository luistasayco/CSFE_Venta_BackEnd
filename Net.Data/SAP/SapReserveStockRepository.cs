using Net.Business.Entities;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Net.Connection;
using System.Collections.Generic;

namespace Net.Data
{
    public class SapReserveStockRepository : RepositoryBase<SapReserveStock>, ISapReserveStockRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public SapReserveStockRepository(IHttpClientFactory clientFactory, IConfiguration configuration, IConnectionSQL context)
             : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }

        public async Task<ResultadoTransaccion<SapBaseResponse<SapReserveStock>>> SetCreateReserve(SapReserveStockNew value)
        {
            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapReserveStock>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = "U_SBA_AJSTINV";
                SapBaseResponse<SapReserveStock> data = await _connectServiceLayer.PostAsyncSBA<SapBaseResponse<SapReserveStock>>(cadena, value);

                if (data.code == 0)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje;
                    return vResultadoTransaccion;
                }

                vResultadoTransaccion.IdRegistro = data.code;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
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

        public async Task<ResultadoTransaccion<SapBaseResponse<SapReserveStock>>> SetDeleteReserve(int value)
        {
            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapReserveStock>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("U_SBA_AJSTINV({0})", value);
                SapBaseResponse<SapReserveStock> data = await _connectServiceLayer.DeleteAsyncSBA<SapBaseResponse<SapReserveStock>>(cadena);

                if (data == null)
                {
                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";
                }else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = data.Mensaje.ToString();
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

        public async Task<ResultadoTransaccion<SapBaseResponse<SapReserveStock>>> SetUpdateReserve(int id, SapReserveStockUpdate value)
        {
            ResultadoTransaccion<SapBaseResponse<SapReserveStock>> vResultadoTransaccion = new ResultadoTransaccion<SapBaseResponse<SapReserveStock>>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = string.Format("U_SBA_AJSTINV({0})", id);
                SapBaseResponse<SapReserveStock> data = await _connectServiceLayer.PatchAsyncSBA<SapBaseResponse<SapReserveStock>>(cadena, value);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "DATOS DE SAP ACTUALIZADO CORRECTAMENTE";

            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<SapReserveStock>> GetListReservaPorIdExterno(string u_idexterno)
        {
            ResultadoTransaccion<SapReserveStock> vResultadoTransaccion = new ResultadoTransaccion<SapReserveStock>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                
                var modelo = "U_SBA_AJSTINV";
                var campos = "?$select=* ";

                var filter = "&$filter = U_IDEXTERNO eq '"+ u_idexterno + "' ";

                modelo = modelo + campos + filter;

                List<SapReserveStock> data = await _connectServiceLayer.GetAsync<SapReserveStock>(modelo);

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
