using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoConsolidadoPedidoEliminar : EntityBase
    {
        public int idconsolidado { get; set; }
        public string codpedido { get; set; }

        public BE_ConsolidadoPedido RetornaConsolidadoPedido()
        {
            return new BE_ConsolidadoPedido
            {
                idconsolidado = idconsolidado,
                codpedido = codpedido,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}

