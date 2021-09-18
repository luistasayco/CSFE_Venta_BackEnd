using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoCheckListComentarioError
    {
        public int ide_comentariochecklist { get; set; }
        public string cod_atencion { get; set; }
        public string dsc_comentario { get; set; }
        public string flg_estado { get; set; }
        public string cod_presupuesto { get; set; }
        public int ide_tarea { get; set; }

        public BE_CheckListComentarioError RetornarCheckListComentarioError()
        {
            return new BE_CheckListComentarioError
            {
                ide_comentariochecklist = this.ide_comentariochecklist,
                cod_atencion = this.cod_atencion,
                dsc_comentario = this.dsc_comentario,
                flg_estado = this.flg_estado,
                cod_presupuesto = this.cod_presupuesto,
                ide_tarea = this.ide_tarea
            };
        }
    }
}
