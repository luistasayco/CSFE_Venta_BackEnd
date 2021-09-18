using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Data;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiProducto")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ConsolidadoSolicitudController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ConsolidadoSolicitudController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet(Name = "GenerarConsolidadoSolicitudPrint")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GenerarConsolidadoSolicitudPrint([FromQuery]DateTime fechainicio, DateTime fechafin)
        {
            var objectGetById = await _repository.Consolidado.GenerarConsolidadoSolicitudPrint(fechainicio, fechafin);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", "Consolidado" + ".pdf");

            return pdf;
        }
    }
}
