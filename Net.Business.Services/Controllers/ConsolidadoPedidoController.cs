using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Data;
using Net.Business.DTO;
using System;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiAtencion")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ConsolidadoPedidoController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ConsolidadoPedidoController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opcion"></param>
        /// <param name="codpaciente"></param>
        /// <param name="nombres"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListPedidosParaConsolidado([FromQuery] string fechaini, string fechafin, string codigopedido)
        {
            var objectGetAll =  new List<DtoPedidoParaConsolidadoResponse>() {
             new DtoPedidoParaConsolidadoResponse()
             {
                 Id = 1,
                 cama ="cama 1",
                 codigoatencion ="1",
                 codigopedido ="p001",
                 nombrealmacen ="almacen1",
                 nombrepaciente = "paciente 1",
                 fechagenerada ="2021-09-07",
                 tipopedido ="1",
                 nombrecentro = "centro 1"
                
             },
            new DtoPedidoParaConsolidadoResponse()
             {
                 Id = 2,
                 cama ="cama 2",
                 codigoatencion ="2",
                 codigopedido ="p003",
                 nombrealmacen ="almacen2",
                 nombrepaciente = "paciente 2",
                 fechagenerada ="2021-09-07",
                 tipopedido ="1",
                 nombrecentro = "centro 1"
             },
             new DtoPedidoParaConsolidadoResponse()
             {
                 Id = 3,
                 cama ="cama 3",
                 codigoatencion ="3",
                 codigopedido ="p004",
                 nombrealmacen ="almacen3",
                 nombrepaciente = "paciente 3",
                 fechagenerada ="2021-09-07",
                 tipopedido ="1",
                 nombrecentro = "centro 3"
             }
            };

            //var objectGetAll = await _repository.Atencion.GetListPacientePorFiltros(opcion, codpaciente, nombres);

            //if (objectGetAll == null)
            //{
            //    return NotFound();
            //}

            return  Ok(objectGetAll);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListPedidosSinConsolidarPorFiltro([FromQuery] DateTime fechainicio, DateTime fechafin, string codtipopedido, string codpedido)
        {

            var objectGetAll = await _repository.ConsolidadoPedido.GetListPedidosSinConsolidarPorFiltro(fechainicio, fechafin, codtipopedido, codpedido);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoPedidoListarResponse().RetornarListaPedido(objectGetAll.dataList);


            return Ok(obj.ListaPedido);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListConsolidadoCabecera([FromQuery] DateTime fechainicio, DateTime fechafin, int idconsolidado)
        {

            var objectGetAll = await _repository.ConsolidadoPedido.GetListConsolidadoCabecera(fechainicio, fechafin, idconsolidado);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoConsolidadoPedidoCabeceraListarResponse().RetornarListaConsolidadoPedido(objectGetAll.dataList);

            return Ok(obj.ListaConsolidadoPedidoCabecera);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListConsolidadoPedido([FromQuery] int idconsolidado)
        {

            var objectGetAll = await _repository.ConsolidadoPedido.GetListConsolidadoPedido(idconsolidado);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoConsolidadoPedidoListarResponse().RetornarListaConsolidadoPedido(objectGetAll.dataList);

            return Ok(obj.ListaConsolidadoPedido);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListConsolidadoPedidoProducto([FromQuery] int idconsolidado, string codpedido)
        {

            var objectGetAll = await _repository.ConsolidadoPedido.GetListConsolidadoPedidoProducto(idconsolidado, codpedido);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoConsolidadoPedidoProductoListarResponse().RetornarListaConsolidadoPedido(objectGetAll.dataList);

            return Ok(obj.ListaConsolidadoPedidoProducto);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListConsolidadoPedidoPicking([FromQuery] int idconsolidado)
        {

            var objectGetAll = await _repository.ConsolidadoPedido.GetListConsolidadoPedidoPickingPorIdConsolidado(idconsolidado);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoConsolidadoPedidoPickingListarResponse().RetornarListaConsolidadoPedidoPicking(objectGetAll.dataList);

            return Ok(obj.ListaConsolidadoPedidoPicking);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateConsolidadoPedido([FromBody] DtoConsolidadoPedidoRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ConsolidadoPedido.CreateConsolidadoPedido(value.RetornaModelo());
                //var response = value.RetornaModelo();

                //return NoContent();

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateConsolidadoPedido([FromBody] DtoConsolidadoPedidoRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ConsolidadoPedido.UpdateConsolidadoPedido(value.RetornaModelo());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteConsolidadoPedidoPorPedido([FromBody] DtoConsolidadoPedidoEliminar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ConsolidadoPedido.DeleteConsolidadoPedidoPorPedido(value.RetornaConsolidadoPedido());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteConsolidado([FromBody] DtoConsolidadoEliminar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ConsolidadoPedido.DeleteConsolidado(value.RetornaConsolidado());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CloseConsolidado([FromBody] DtoConsolidadoCerrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ConsolidadoPedido.CloseConsolidado(value.RetornaConsolidado());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpGet("{idconsolidado}", Name = "GenerarConsolidadoPedidoPrint")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GenerarConsolidadoPedidoPrint(int idconsolidado)
        {
            var objectGetById = await _repository.ConsolidadoPedido.GenerarConsolidadoPedidoPrint(idconsolidado);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", idconsolidado + ".pdf");

            return pdf;
        }
    }
}
