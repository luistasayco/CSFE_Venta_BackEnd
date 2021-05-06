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
    [ApiExplorerSettings(GroupName = "ApiTipoCambio")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class TipoCambioController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public TipoCambioController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtieneTipoCambio()
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var ObjectNew = await _repository.TipoCambio.ObtieneTipoCambio();

            if (ObjectNew == null)
            {
                return StatusCode(500, "");
            }

            return Ok(ObjectNew);
        }
    }
}
