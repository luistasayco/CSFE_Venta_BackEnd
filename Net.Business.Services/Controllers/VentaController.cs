using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Data;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiVenta")]

    public class VentaController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public VentaController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Obtener Listado
        /// </summary>
        /// <param name="buscar">Codigo de Venta o Comprobante</param>
        /// <param name="key"></param>
        /// <param name="numeroLineas">numero de lineas que devolvera la consulta</param>
        /// <param name="orden"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(string buscar, int key, int numeroLineas, int orden, string fecha)
        {

            var objectGetAll = await _repository.VentaCabecera.GetAll(buscar,key, numeroLineas,orden, fecha);

            var obj = new DtoVentaCabeceraListarResponse().RetornarListaVentaCabecera(objectGetAll.ToList());

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }
    }
}
