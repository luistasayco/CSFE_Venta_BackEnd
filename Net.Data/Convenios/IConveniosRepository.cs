using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IConveniosRepository : IRepositoryBase<BE_ConveniosListaPrecio>
    {
        
        Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConvenioslistaprecio(int idconvenio, int pricelist, string codtipocliente, string codpaciente, string codaseguradora, string codcliente, string fechareg, string tmovimiento);
        Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> GetConveniosPorFiltros(string codalmacen, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, string codaseguradora, string codcia, string codproducto);

        Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Registrar(BE_ConveniosListaPrecio value);
        Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Modificar(BE_ConveniosListaPrecio value);
        Task<ResultadoTransaccion<BE_ConveniosListaPrecio>> Eliminar(int idconvenio, int idusuario);



    }
}
