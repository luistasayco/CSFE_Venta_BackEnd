using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ICheckListRegistroMovimientoRepository
    {
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoInsert(BE_CheckListRegistroMovimiento value);
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoUpdatet(BE_CheckListRegistroMovimiento value);
        Task<ResultadoTransaccion<string>> CheckListRegistroMovimientoAnular(BE_CheckListRegistroMovimiento value);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimiento(string cod_atencion, int ide_tarea, int orden);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCkeckListRegistroMovimientoEnviarCorreo(string cod_atencion, int ide_tarea, int orden);
        Task<ResultadoTransaccion<BE_CheckListRegistroMovimiento>> GetCheckListRegistroMovimientoVerificar(string cod_atencion, int ide_tarea, int orden);
        Task<ResultadoTransaccion<string>> ChecklistComentarioErrorInsert(BE_CheckListComentarioError value);
        Task<ResultadoTransaccion<BE_HospitalDocumento>> GetHospitalDocumento(string pcod_atencion, int porden);
        Task<ResultadoTransaccion<BE_Hospital>> GetHospitalDetalle(string buscar, int key, int numerolineas, int tipoatencion, int filtrolocal);
        
    }
}
