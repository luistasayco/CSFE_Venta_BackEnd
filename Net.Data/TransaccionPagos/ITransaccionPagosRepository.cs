using Net.Business.Entities;
using Net.Connection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ITransaccionPagosRepository
    {
        //Task<ResultadoTransaccion<long>> RegistrarTransaccionPagos(BE_TransaccionPagos value);
        Task<ResultadoTransaccion<object>> ProcesarTransaccion(BE_ProcesarTransaccionPagoRequest value,string codventa,int regcreateusuario);
        Task<ResultadoTransaccion<object>> AnularTransaccion(BE_ProcesarTransaccionAnularRequest value, string codventa, int regcreateusuario);

    }
}
