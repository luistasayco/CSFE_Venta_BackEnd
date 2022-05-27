using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoRecetaObservacionModificar: EntityBase
    {
        public int idobs { get; set; }
        public int idusuarioanulacion { get; set; }

        public BE_RecetaObservacion RetornaModelo()
        {
            return new BE_RecetaObservacion
            {
                idobs = idobs,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }

    public class DtoRecetaEstadoModificar : EntityBase
    {
        public int ide_receta { get; set; }
        public string estadovd { get; set; }

        public BE_RecetaEstadoModificar RetornaModelo()
        {
            return new BE_RecetaEstadoModificar
            {
                ide_receta = ide_receta,
                estadovd = estadovd,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
