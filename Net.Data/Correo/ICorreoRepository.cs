using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ICorreoRepository
    {
        Task<ResultadoTransaccion<string>> GetCorreoDestinatario(string codLista, string dscTipo);
        Task<ResultadoTransaccion<string>> Registrar(BE_Correo value);

    }
}
