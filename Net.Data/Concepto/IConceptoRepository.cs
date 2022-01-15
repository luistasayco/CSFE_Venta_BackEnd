using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IConceptoRepository
    {
        Task<ResultadoTransaccion<BE_Concepto>> GetById(BE_Concepto value);
        Task<ResultadoTransaccion<BE_Concepto>> GetByDescription(BE_Concepto value);
        Task<ResultadoTransaccion<BE_Concepto>> Registrar(BE_Concepto item);
        Task<ResultadoTransaccion<BE_Concepto>> Modificar(BE_Concepto item);
        Task<ResultadoTransaccion<BE_Concepto>> Eliminar(BE_Concepto item);
    }
}
