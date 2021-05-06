using Microsoft.Data.SqlClient;
using Net.Business.Entities;
using Net.Connection;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Net.Data
{
    public class TipoCambioRepository : RepositoryBase<BE_TipoCambio>, ITipoCambioRepository
    {
        private string _metodoName;
        private string _aplicacionName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_TIPO_CAMBIO = DB_ESQUEMA + "VEN_GetVentasCabeceraPorFiltros";

        public TipoCambioRepository(IConnectionSQL context)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
        }
        public async Task<ResultadoTransaccion<BE_TipoCambio>> ObtieneTipoCambio()
        {
            ResultadoTransaccion<BE_TipoCambio> vResultadoTransaccion = new ResultadoTransaccion<BE_TipoCambio>();

            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            BE_TipoCambio tipoCambio = new BE_TipoCambio();
            tipoCambio.bancarioventa = 3.5990M;

            vResultadoTransaccion.ResultadoCodigo = 0;
            vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
            //vResultadoTransaccion.Data = tipoCambio;

            //using (SqlConnection conn = new SqlConnection(context.DevuelveConnectionSQL()))
            //{
            //    using (CommittableTransaction transaction = new CommittableTransaction())
            //    {
            //        await conn.OpenAsync();
            //        conn.EnlistTransaction(transaction);

            //        try
            //        {
            //            using (SqlCommand cmd = new SqlCommand(SP_GET_TIPO_CAMBIO, conn))
            //            {
            //                cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //                SqlParameter oParam = new SqlParameter("@tipodecambio", 0);
            //                oParam.SqlDbType = SqlDbType.Int;
            //                oParam.Direction = ParameterDirection.Output;
            //                cmd.Parameters.Add(oParam);

            //                await cmd.ExecuteNonQueryAsync();

            //                tipoCambio.bancarioventa = (int)cmd.Parameters["@tipodecambio"].Value;

            //                vResultadoTransaccion.IdRegistro = (int)cmd.Parameters["@tipodecambio"].Value;
            //                vResultadoTransaccion.ResultadoCodigo = 0;
            //                vResultadoTransaccion.ResultadoDescripcion = "Se realizo correctamente";
            //                vResultadoTransaccion.Data = tipoCambio;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            vResultadoTransaccion.IdRegistro = -1;
            //            vResultadoTransaccion.ResultadoCodigo = -1;
            //            vResultadoTransaccion.ResultadoDescripcion += ex.Message.ToString();
            //            transaction.Rollback();
            //        }
            //    }
            //}

            return vResultadoTransaccion;
        }
    }
}
