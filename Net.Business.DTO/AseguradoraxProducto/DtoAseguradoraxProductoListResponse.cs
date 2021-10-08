using Net.Business.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Net.Business.DTO.AseguradoraxProducto
{
    public class DtoAseguradoraxProductoListResponse
    {
        public IEnumerable<DtoAseguradoraxProductoResponse> ListaAseguradoraxProducto { get; set; }

        public DtoAseguradoraxProductoListResponse RetornarCheckListRegistroMovimientoListar(IEnumerable<BE_AseguradoraxProducto> listaAseguradoraxProducto)
        {
            IEnumerable<DtoAseguradoraxProductoResponse> lista = (
                from value in listaAseguradoraxProducto
                select new DtoAseguradoraxProductoResponse
                {
                    codaseguradora = value.codaseguradora,
                    nomseguradora = value.nomseguradora,
                    codproducto = value.codproducto,
                    nomproducto = value.nomproducto,
                    fec_registro = value.fec_registro,
                    dsctipoatencionmae = value.dsctipoatencionmae
                }
            );

            return new DtoAseguradoraxProductoListResponse() { ListaAseguradoraxProducto = lista };
        }
    }
}
