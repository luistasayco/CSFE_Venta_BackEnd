using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ITipoConceptoRepository
    {
        Task<ResultadoTransaccion<BE_TipoConcepto>> GetTipoConcepto();
    }
}
