using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoConsolidadoCerrar : EntityBase
    {
        public int idconsolidado { get; set; }

        public BE_Consolidado RetornaConsolidado()
        {
            return new BE_Consolidado
            {
                idconsolidado = idconsolidado,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}

