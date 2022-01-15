using Net.Business.Entities;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IProductoRepository
    {
        //Task<ResultadoTransaccion<BE_Producto>> GetListProductoPorFiltro(string codalmacen, string codigo, string nombre, string codaseguradora, string codcia, bool conStock = true);
        //Task<ResultadoTransaccion<BE_Producto>> GetListProductoGenericoPorCodigo(string codigo, bool conStock = true);
        Task<ResultadoTransaccion<BE_Producto>> GetListProductoAlternativoPorCodigo(string codproducto, string codaseguradora, string codcia);
        Task<ResultadoTransaccion<BE_Producto>> GetProductoPorCodigo(string codalmacen, string codproducto, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion, bool constock);
        Task<ResultadoTransaccion<BE_Producto>> GetProductoyStockAlmacenesPorCodigo(string codproducto);

        Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorPedido(string codpedido, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion);
        Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorReceta(int idreceta, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion);
        Task<ResultadoTransaccion<BE_Producto>> GetListDetalleProductoPorIdBorrador(int idborrador, string codalmacen, string codaseguradora, string codcia, string tipomovimiento, string codtipocliente, string codcliente, string codpaciente, int tipoatencion);
        Task<ResultadoTransaccion<BE_AseguradoraxProducto>> GetProductoPorCodigoAseguradora(string codaseguradora, string codproducto, int codtipoatencion_mae, int orden);
        Task<ResultadoTransaccion<BE_Producto>> GetProductoPorFiltro(string nombreProducto, string nombreFamilia, string nombreLaboratorio);
        Task<ResultadoTransaccion<BE_Stock>> GetProductoPorCodigoBarra(string codalmacen, string codigoBarra);
    }
}
