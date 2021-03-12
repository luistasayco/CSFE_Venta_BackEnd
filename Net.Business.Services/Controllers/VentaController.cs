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
    [ApiExplorerSettings(GroupName = "ApiVenta")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class VentaController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public VentaController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="codcomprobante"></param>
       /// <param name="codventa"></param>
       /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] string codcomprobante, string codventa)
        {

            var objectGetAll = await _repository.VentaCabecera.GetAll(codcomprobante, codventa);

            var obj = new DtoVentaCabeceraListarResponse().RetornarListaVentaCabecera(objectGetAll);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj.ListaVentaCabecera);
        }
    }
}
