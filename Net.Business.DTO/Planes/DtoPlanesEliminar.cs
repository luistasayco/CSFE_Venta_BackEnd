using Net.Business.Entities;

namespace Net.Business.DTO
{

    public class DtoPlanesEliminar: EntityBase
    {
        public int IdPlan { get; set; }

        public BE_Planes RetornaPlanes()
        {
            return new BE_Planes
            {
                IdPlan = this.IdPlan,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
