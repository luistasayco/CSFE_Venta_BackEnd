using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ICiasRepository
    {
        Task<ResultadoTransaccion<BE_Cias>> GetCias();
        Task<ResultadoTransaccion<BE_Cias>> GetCiasFiltros(string nombre);

    }
}
