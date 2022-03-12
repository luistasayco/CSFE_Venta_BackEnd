using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiProducto")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CheckListController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public CheckListController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckListRegistroMovimientoInsert([FromBody] DtoCheckListRegistroMovimientoInsert value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.CheckListRegistroMovimiento.CheckListRegistroMovimientoInsert(value.RetornaCheckListRegistroMovimientoInsert());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en el proceso de grabado : { ex.Message.ToString() }");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckListRegistroMovimientoUpdatet([FromBody] DtoCheckListRegistroMovimientoUpdate data)
        {
            try
            {

                //List<DtoCheckListRegistroMovimientoUpdate> value = JsonConvert.DeserializeObject<List<DtoCheckListRegistroMovimientoUpdate>>(data);

                if (string.IsNullOrEmpty(data.data))
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.CheckListRegistroMovimiento.CheckListRegistroMovimientoUpdatet(data.data, data.comentario);

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en el proceso de actualización : { ex.Message.ToString() }");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckListRegistroMovimientoAnular([FromBody] DtoCheckListRegistroMovimientoAnular value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.CheckListRegistroMovimiento.CheckListRegistroMovimientoAnular(value.RetornaCheckListRegistroMovimientoAnular());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en el proceso de anulación : { ex.Message.ToString() }");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCheckListRegistroMovimiento([FromQuery] string cod_atencion, int ide_tarea, int orden)
        {
            var objectGetAll = await _repository.CheckListRegistroMovimiento.GetCheckListRegistroMovimiento(cod_atencion, ide_tarea, orden);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoCheckListRegistroMovimientoListarResponse().RetornarCheckListRegistroMovimientoListar(objectGetAll.dataList);

            return Ok(obj.ListaCheckListRegistroMovimiento);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCkeckListRegistroMovimientoEnviarCorreo([FromQuery] string cod_atencion, int ide_tarea)
        {
            var objectGetAll = await _repository.CheckListRegistroMovimiento.GetCkeckListRegistroMovimientoEnviarCorreo(cod_atencion, ide_tarea);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoCkeckListRegistroMovimientoEnviarCorreoListarResponse().RetornarCkeckListRegistroMovimientoEnviarCorreoListar(objectGetAll.dataList);

            return Ok(obj.ListarCkeckListRegistroMovimientoEnviarCorreo);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCheckListRegistroMovimientoVerificar([FromQuery] string cod_atencion, int ide_tarea)
        {
            var objectGetAll = await _repository.CheckListRegistroMovimiento.GetCheckListRegistroMovimientoVerificar(cod_atencion, ide_tarea);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoCheckListRegistroMovimientoVerificarResponse().RetornarCheckListRegistroMovimientoVerificar(objectGetAll.dataList);

            return Ok(obj.ListaCheckListRegistroMovimientoVerificar);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChecklistComentarioErrorInsert([FromBody] DtoCheckListComentarioError value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.CheckListRegistroMovimiento.ChecklistComentarioErrorInsert(value.RetornarCheckListComentarioError());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en el proceso : { ex.Message.ToString() }");
            }
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHospitalDocumento([FromQuery] string pcod_atencion, int porden)
        {
            var objectGetAll = await _repository.CheckListRegistroMovimiento.GetHospitalDocumento(pcod_atencion, porden);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHospitalDetalle([FromQuery] string buscar, int key, int numerolineas, int orden, string tipoatencion, string filtrolocal)
        {
            var objectGetAll = await _repository.CheckListRegistroMovimiento.GetHospitalDetalle(buscar, key, numerolineas, orden, tipoatencion, filtrolocal);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpGet("{codatencion}", Name = "GenerarCkeckListReportePrint")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GenerarCkeckListReportePrint(string codatencion)
        {
            var objectGetById = await _repository.CheckListRegistroMovimiento.GenerarCkeckListReportePrint(codatencion);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", codatencion + ".pdf");

            return pdf;
        }
    }
}
