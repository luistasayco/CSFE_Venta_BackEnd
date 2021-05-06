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
    [ApiExplorerSettings(GroupName = "ApiApu")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ApuController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ApuController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Listara los pedidos por rango de fechas
        /// </summary>
        /// <param name="fechainicio"> Fecha de Inicio</param>
        /// <param name="fechafin">Fecha Fin</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListPedidosPorRangoFechas([FromQuery] DateTime fechainicio, DateTime fechafin)
        {

            var objectGetAll = await _repository.Pedido.GetListPedidosPorFiltro(fechainicio, fechafin, string.Empty, string.Empty);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoPedidoApuListarResponse().RetornarListaPedido(objectGetAll.dataList).ListaPedido;

            return Ok(obj);
        }

        /// <summary>
        /// Lista un pedido por un codigo de pedido
        /// </summary>
        /// <param name="codpedido">Codigo de Pedido</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListPedidosPorPedido([FromQuery] string codpedido)
        {

            var objectGetAll = await _repository.Pedido.GetListPedidosPorPedido(codpedido);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoPedidoApuListarResponse().RetornarListaPedido(objectGetAll.dataList);

            return Ok(obj.ListaPedido);
        }

        /// <summary>
        /// Listara el detalle del pedido
        /// </summary>
        /// <param name="codpedido">Codigo de Pedido</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListDetallePedidoPorPedido([FromQuery] string codpedido)
        {

            var objectGetAll = await _repository.Pedido.GetListPedidoDetallePorPedido(codpedido);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        /// <summary>
        /// Listara las recetas por rango de fechas
        /// </summary>
        /// <param name="fechainicio">Fecha Inicio</param>
        /// <param name="fechafin">Fecha Fin</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListRecetasPorFiltro([FromQuery] DateTime fechainicio, DateTime fechafin)
        {

            var objectGetAll = await _repository.Receta.GetListRecetasPorFiltro(fechainicio, fechafin, string.Empty, 0, string.Empty, string.Empty);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoRecetaApuListarResponse().RetornarListaReceta(objectGetAll.dataList);

            return Ok(obj.ListaReceta);
        }

        /// <summary>
        /// Lista la receta por un id de receta
        /// </summary>
        /// <param name="id">Id de Receta</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListRecetasPorReceta([FromQuery] int id)
        {

            var objectGetAll = await _repository.Receta.GetListRecetasPorReceta(id);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            var obj = new DtoRecetaApuListarResponse().RetornarListaReceta(objectGetAll.dataList);

            return Ok(obj.ListaReceta);
        }

        /// <summary>
        /// Listara el detalle de la receta
        /// </summary>
        /// <param name="id">Id Receta</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListDetalleRecetaPorReceta([FromQuery] int id)
        {

            var objectGetAll = await _repository.Receta.GetListRecetaDetallePorReceta(id);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

    }
}
