using Net.Business.Entities;


namespace Net.Business.DTO
{
    public class DtoComprobanteDarBaja
    {

        public string codComprobante { get; set; }
        public string motivoDarBaja { get; set; }
        public int idUsuario { get; set; }
       
        public BE_ComprobantesBaja RetornaComprobanteDarBaja() {

            var obj = new BE_ComprobantesBaja();
            obj.cod_comprobante = codComprobante;
            obj.dsc_motivobaja = motivoDarBaja;
            return obj;
        }

    }
}
