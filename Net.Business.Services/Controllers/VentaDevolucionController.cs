using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Business.Entities;
using Net.Data;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiVenta")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class VentaDevolucionController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public VentaDevolucionController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Obtener lista de registros
        /// </summary>
        /// <param name="value">Este es el cuerpo para enviar los parametros</param>
        /// <returns>Retorna lista</returns>
        /// <response code="200">Devuelve el listado completo </response>
        /// <response code="404">Si no existen datos</response>   
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetVentasPorAtencion([FromQuery] DtoVentaDevolucionFind value)
        {

            var objectGetAll = await _repository.VentaDevolucion.GetVentasPorAtencion(value.RetornaModelo());

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNotaCabeceraPorFiltro([FromQuery] string codcomprobante, string codventa, DateTime fecinicio, DateTime fecfin, string codnota, string anombredequien)
        {

            var objectGetAll = await _repository.VentasNota.GetNotaCabeceraPorFiltro(codcomprobante, codventa, fecinicio, fecfin, codnota, anombredequien);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNotaCabeceraPorCodNota([FromQuery] string codnota)
        {

            var objectGetAll = await _repository.VentasNota.GetNotaCabeceraPorCodNota(codnota);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DarDeBaja([FromBody] DtoComprobanteDarBaja value)
        {

            ResultadoTransaccion<string> response = new ResultadoTransaccion<string>();
            response.IdRegistro = -1;
            string codComprobante = value.codComprobante;

            var obj = await _repository.VentasNota.GetNotaCabeceraPorCodNota(value.codComprobante);
            if (obj.dataList.Count() == 0) return BadRequest("No Existe Comprobanet");

            var objE = obj.dataList.FirstOrDefault();

            //objE.numeroplanilla
            //objE.fechaemision
            //objE.estado

            if (objE.estado.Equals("X"))
            {
                response.IdRegistro = -1;
                response.ResultadoDescripcion = "El Comprobante YA Fue Anulado Anteriormente";
                return Ok(response);
            }
            if (!string.IsNullOrEmpty(objE.numeroplanilla))
            {
                response.IdRegistro = -1;
                response.ResultadoDescripcion = "El comprobante esta incluido en la planilla";
                return Ok(response);
            }

            var objCP = await _repository.VentaCaja.GetComprobanteElectroncioVB("001", codComprobante, "", "L", "", 6);
            if (objCP.IdRegistro == -1) return BadRequest(objCP.ResultadoDescripcion);
            if (objCP.data.flg_electronico == "1")
            {
                if (objCP.data.dar_baja != "S")
                {
                    response.IdRegistro = -1;
                    response.ResultadoDescripcion = "No puede Dar de baja este comprobante <br>" + objCP.data.mensaje + "<br>Regla: Tener CDR-Aceptado, No estar Otorgado, No tener envío a baja, No tener Nota. Fecha de CDR(FT) y FechaEmisión(BV) no pasar los días permitidos";
                    return Ok(response);
                }
            }
            else
            {
                response.IdRegistro = -1;
                response.ResultadoDescripcion = "Solo se dan de baja comprobantes electrónicos";
                return Ok(response.ResultadoDescripcion);
            }

            var fechaHoy = DateTime.Now;
            //var fechaHoy = DateTime.Now.ToString("dd-MM-yyyy");

            if (objE.fechaemision.Month != fechaHoy.Month)
            {
                response.IdRegistro = -1;
                response.ResultadoDescripcion = "No se puede DAR DE BAJA porque es del mes pasado";
                return Ok(response.ResultadoDescripcion);
            }

            //var objCS = await _repository.Comprobante.GetComprobanteConsulta(codComprobante, 0, 0, 5);
            //var objResponse = objCS.dataList.FirstOrDefault();

            var objResponse = await _repository.VentaCaja.GetComprobanteElectroncioCodVenta(codComprobante, 0, 0, "", "", "", "", "", "", 5);
            if (objResponse.data.codcomprobante == null)
            {
                response.IdRegistro = -1;
                response.ResultadoDescripcion = "No se puede dar de baja al comprobante";
                return Ok(response.ResultadoDescripcion);
            }

            var objResultCPE = objResponse.data;

            //Comprobante_baja
            var request = value.RetornaComprobanteDarBaja();
            //request.cod_comprobante = objResultCPE.codcomprobante;
            request.cod_comprobantee = objResultCPE.codcomprobantee;

            string pSerieComprobanteE = objResultCPE.codcomprobantee.Substring(0, 4);
            //string pNumeroComprobanteE = objResultCPE.codcomprobantee.Substring(5, objResultCPE.codcomprobantee.Length);
            string pNumeroComprobanteE = objResultCPE.codcomprobantee.Substring(4, (objResultCPE.codcomprobantee.Length - 4));
            string pCodComprobanteEFact = objResultCPE.codcomprobantee;

            request.cod_tipocompsunat = objResultCPE.tipo_comprobante;
            request.fec_emisioncomp = objResultCPE.fecha_registro_sis;
            request.fec_baja = fechaHoy;
            request.cod_sistema = "L";
            request.cod_empresa = "001";

            //string  pENEnumeradosNoEmitidos = pENEnumeradosNoEmitidos + ENNumeradosNoEmitidos("1", pTipoCompSunat, pSerieComprobanteE, pNumeroComprobanteE, pMotivoBaja)
            //      pENErrorComunicacion = pENErrorComunicacion + ENErrorComunicacion

            //request.nu = objResultCPE.codcomprobantee.Substring(0,4);
            //codcomprobantee

            string xRucEmisor = string.Empty, xUrlWebService = string.Empty;

            var objConf = await _repository.Tabla.GetListTablaClinicaPorFiltros("EFACT_PARAMETROSWSTCI", "", 0, 0, 7);
            foreach (var item in objConf.dataList)
            {
                if (item.codigo.Trim().Equals("RUC")) xRucEmisor = item.nombre;
                if (item.codigo.Trim().Equals("WS")) xUrlWebService = item.nombre;
            }

            var objResultBaja = await _repository.VentaCaja.RegistrarComunicadoBajoComprobante(request, xUrlWebService, xRucEmisor);
            if (objResultBaja.IdRegistro == 0)
            {
                response.IdRegistro = 0;
                response.ResultadoDescripcion = "Su comprobante fue enviado De_Baja.<br>Favor consultar el estado de su solicitud (Aceptado o Rechazado).";
            }

            //else
            //{
            //    response.IdRegistro = -1;
            //    response.ResultadoDescripcion = "Su comprobante no pudo ser enviado De_Baja. Favor verificar los estados<br>Regla: Tener CDR-Rechazado, No tener envío a baja, No tener Nota.";
            //}


            //GetComprobanteElectroncioCodVenta

            //wErrorBaja = zEfact.RegistrarComunicadoBajoComprobante(Me.txtCodComprobante, wMotivoBaja, zFarmacia2)
            //        If wErrorBaja = True Then 'OK
            //            Me.MousePointer = vbDefault
            //            MsgBox "Su comprobante fue enviado De_Baja." & Chr(10) _
            //            & "Favor consultar el estado de su solicitud (Aceptado o Rechazado).", vbInformation, "Sunat - " & Me.txtCodComprobante
            //        Else
            //            Me.MousePointer = vbDefault
            //            MsgBox "Su comprobante no pudo ser enviado De_Baja. Favor verificar los estados" & Chr(10) _
            //            & wMensaje & Chr(10) _
            //            & "Regla: Tener CDR-Rechazado, No tener envío a baja, No tener Nota.", vbInformation, Me.txtCodComprobante
            //            Exit Sub
            //        End If

            return Ok(response); ;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EliminarNotaCredito([FromBody] DtoEliminarNotaCredito value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.VentasNota.EliminarNotaCredito(value.codnota, value.usuario, value.idusuario);

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }   
    }
}
