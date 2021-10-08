using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Data;
using System.Threading.Tasks;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiCliente")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AseguradoraxProductoBuscarController : Controller
    {
        private readonly IRepositoryWrapper _repository;
        public AseguradoraxProductoBuscarController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }


        /// <summary>
        /// Busqueda por aseguradora
        /// </summary>
        /// <param name="buscar"></param>
        /// <param name="estado"></param>
        /// <param name="orden"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAseguradora([FromQuery] string buscar,string estado, string orden)
        {

            var objectGetAll = await _repository.Aseguradora.GetAseguradora(buscar, estado, orden);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        /// <summary>
        /// Busqueda por familia
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFamilia([FromQuery] string nombre)
        {

            if (nombre != null) nombre = nombre.ToUpper();

             //var objectGetAll = await _repository.Familia.GetFamilia(buscar, orden);
             var objectGetAll = await _repository.Familia.GetFamiliaLista(nombre);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        /// <summary>
        /// Busqueda por Laboratorio
        /// </summary>
        /// <param name="buscar"></param>
        /// <param name="orden"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLaboratorio([FromQuery] string buscar, string orden)
        {

            var objectGetAll = await _repository.Laboratorio.GetLaboratorio(buscar, orden);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }
    }
}
