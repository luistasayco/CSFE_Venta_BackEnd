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
    public class AseguradoraxProductoController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public AseguradoraxProductoController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListAseguradoraxProductoPorFiltros([FromQuery] string codaseguradora, string codproducto, int tipoatencion)
        {

            var objectGetAll = await _repository.AseguradoraxProducto.GetListAseguradoraxProductoPorFiltros(codaseguradora, codproducto, tipoatencion);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AseguradoraxProductoInsert(DtoAseguradoraxProductoInsert value)
        {


            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                //value.codaseguradora = "0257";
                //value.codproducto = "00084326";

                //value.codaseguradora = "0111";
                //value.codproducto = "00090712";

                var responseExiteProducto = await _repository.AseguradoraxProducto.GetExisteAseguradoraxProducto(value.codaseguradora, value.codproducto);
                if(responseExiteProducto.IdRegistro==-1) return BadRequest($"GENERO UN ERROR AL MOMENTO DE VERIFICAR SI EXISTE EL PRODUCTO CON LA ASEGURADORA.");

                //si el producto ya existe lo devolvemos
                if (responseExiteProducto.data)
                {
                    return Ok(responseExiteProducto);
                }

                var responseExiteConvenio = await _repository.AseguradoraxProducto.GetExisteConvenioAseguradoraxProducto(value.codaseguradora, value.codproducto);
                if (responseExiteProducto.IdRegistro == -1) return BadRequest($"GENERO UN ERROR AL MOMENTO DE VERIFICAR SI EXISTE EL CONVENIO CON EL PRODUCTO");

                if (responseExiteConvenio.data=="S")
                {
                    responseExiteConvenio.ResultadoDescripcion = "No puede agregar el producto porque tiene convenio de descuento!!!";
                    return Ok(responseExiteConvenio);
                }

                int orden = 1;
                var response = await _repository.AseguradoraxProducto.AseguradoraxProductoInsert(value.RetornaAseguradoraxProductoInsert(), orden);

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
                return BadRequest($"Hubo un error en el proceso de grabado : { ex.Message.ToString() }");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AseguradoraxProductoDelete(DtoAseguradoraxProductoDelete value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }


                var bInser = false;

                if (value.orden == 2)
                {
                    if (value.checkbox == 1) bInser = true;

                }

                if (bInser == false) {
                    //int orden = 1;
                    var response = await _repository.AseguradoraxProducto.AseguradoraxProductoDelete(value.RetornaAseguradoraxProductoDelete(), value.orden);
                    if (response.IdRegistro == -1) return BadRequest(response);

                        if (response.ResultadoCodigo == -1)
                        {
                            return BadRequest(response);
                        }

                    return Ok(response);
                }
                else
                {
                    int orden = 2;
                    var responseInsert = await _repository.AseguradoraxProducto.AseguradoraxProductoInsert(value.RetornaAseguradoraxProductoDelete(), orden);
                    if (responseInsert.IdRegistro == -1) return BadRequest(responseInsert);
                    return Ok(responseInsert);

                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en el proceso de eliminar : { ex.Message.ToString() }");
            }
        }


        /// <summary>
        /// Carga ltvAsignados: Al seleccionar aseguradora desde la ventana de busqueda
        /// </summary>
        /// <param name="codaseguradora"></param>
        /// <param name="codproducto"></param>
        /// <param name="codtipoatencion_mae"></param>
        /// <param name="orden"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductoPorCodigoAseguradora([FromQuery] string codaseguradora,string codproducto,int codtipoatencion_mae,int orden)
        {
            var objectGetAll = await _repository.Producto.GetProductoPorCodigoAseguradora(codaseguradora, codproducto, codtipoatencion_mae, orden);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }


        /// <summary>
        /// Al hacer clic a ltvAsignados
        /// </summary>
        /// <param name="codaseguradora"></param>
        /// <param name="codproducto"></param>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTipoAtencionPorFiltros([FromQuery] string codaseguradora, string codproducto)
        {
            var objectGetAll = await _repository.TipoAtencion.GetTipoAtencionPorFiltros(codaseguradora, codproducto);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);

        }


        /// <summary>
        /// Boton buscar 1: Por Familia, Laboratorio o producto --> ltvProductos
        /// </summary>
        /// <param name="nombreProducto"></param>
        /// <param name="nombreFamilia"></param>
        /// <param name="nombreLaboratoion"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductoPorFiltro([FromQuery] string nombreProducto, string nombreFamilia,string nombreLaboratoion)
        {
            var objectGetAll = await _repository.Producto.GetProductoPorFiltro(nombreProducto, nombreFamilia, nombreLaboratoion);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }        
    }
}
