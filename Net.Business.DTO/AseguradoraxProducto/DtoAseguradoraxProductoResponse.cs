using System;

namespace Net.Business.DTO.AseguradoraxProducto
{
    public class DtoAseguradoraxProductoResponse
    {
        public string codaseguradora { get; set; }
        public string nomseguradora { get; set; }
        public string codproducto { get; set; }
        public string nomproducto { get; set; }
        public DateTime fec_registro { get; set; }
        public string dsctipoatencionmae { get; set; }
    }
}
