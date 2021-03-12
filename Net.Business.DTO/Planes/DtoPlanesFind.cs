using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoPlanesFind
    {
        public int? IdPlan { get; set; }
        public string Nombre { get; set; }

        public BE_Planes RetornaPlanes()
        {
            return new BE_Planes
            {
                IdPlan = this.IdPlan,
                Nombre = this.Nombre
            };
        }
    }
}
