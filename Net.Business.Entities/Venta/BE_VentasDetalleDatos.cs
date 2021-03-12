using Net.Connection.Attributes;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_VentasDetalleDatos
    {
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventa { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipodocumentoautorizacion { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string numerodocumentoautorizacion { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal valor_dscto { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porc_dscto { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string obs_dscto { get; set; }
        [DBParameter(SqlDbType.VarChar, 4, ActionType.Everything)]
        public string tipo_dscto { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int num_sec { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string estado { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int flg_asi { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal val_venta { get; set; }
        [DBParameter(SqlDbType.Char, 12, ActionType.Everything)]
        public string codpresotor { get; set; }
    }
}
