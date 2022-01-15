using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Data;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiCliente")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ConveniosController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ConveniosController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConveniosPorFiltros([FromQuery] string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto)
        {

            var objectGetAll = await _repository.Convenios.GetConveniosPorFiltros(codalmacen, tipomovimiento, codtipocliente, codcliente, codpaciente, codaseguradora, codcia, codproducto);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);
        }
        /// <summary>
        /// Obtiene la lista de contatantes de la tabla Cia
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConvenioslistaprecio([FromQuery] int idconvenio, int pricelist, string codtipocliente, string codpaciente, string codaseguradora, string codcliente,string fechareg,string tmovimiento)
        {

            var objectGetAll = await _repository.Convenios.GetConvenioslistaprecio(idconvenio,pricelist, codtipocliente, codpaciente, codaseguradora, codcliente, fechareg, tmovimiento);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);

        }


        /// <summary>
        /// Obtiene la lista de "Precio lista o lista de precio"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListaPrecio()
        {

            var objectGetAll = await _repository.ListaPrecio.GetListaPrecio();

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarListaPrecio([FromBody] DtoConveniosListaPrecioRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.Convenios.Registrar(value.RetornaConveniosListaPrecio());

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
                return BadRequest($"Hubo un error al grabar: { ex.Message.ToString() }");
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModificarListaPrecio([FromBody] DtoConveniosListaPrecioModificar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.Convenios.Modificar(value.RetornaConveniosListaPrecio());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.dataList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error al modificar : { ex.Message.ToString() }");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EliminarListaPrecio([FromBody] DtoConvenioAnular value)
        {
            try
            {
                if (value.idconvenio == 0)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.Convenios.Eliminar(value.idconvenio, value.idUsuario);

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
                return BadRequest($"Hubo un error al eliminar : { ex.Message.ToString() }");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Modificar([FromBody] DtoConveniosListaPrecioModificar value)
        {
            try
            {
                if (value.idconvenio == 0)
                {
                    return BadRequest("");
                }

                var response = await _repository.Convenios.Modificar(value.RetornaConveniosListaPrecio());

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
                return BadRequest($"Hubo un error al eliminar : { ex.Message.ToString() }");
            }
        }



    }
}
