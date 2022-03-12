using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ICheckListRegistroMovimientoRepository
    {
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoInsert(BE_CheckListRegistroMovimiento value);
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoUpdatet(string value, string comentario);
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoAnular(BE_CheckListRegistroMovimiento value);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimiento(string cod_atencion, int ide_tarea, int orden);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCkeckListRegistroMovimientoEnviarCorreo(string cod_atencion, int ide_tarea);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimientoVerificar(string cod_atencion, int ide_tarea);
        Task<ResultadoTransaccion<string>> ChecklistComentarioErrorInsert(BE_CheckListComentarioError value);
        Task<ResultadoTransaccion<BE_HospitalDocumento>> GetHospitalDocumento(string pcod_atencion, int porden);
        Task<ResultadoTransaccion<BE_Hospital>> GetHospitalDetalle(string buscar, int key, int numerolineas, int orden, string tipoatencion, string filtrolocal);
        Task<ResultadoTransaccion<MemoryStream>> GenerarCkeckListReportePrint(string codatencion);


    }
}
