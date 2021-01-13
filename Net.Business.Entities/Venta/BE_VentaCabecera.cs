using Net.Connection.Attributes;
using System;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_VentaCabecera
    {
        [DBParameter(SqlDbType.Char, ActionType.Everything, true)]
        public string CodVenta { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipomovimiento { get; set; }
        [DBParameter(SqlDbType.Char, 11, ActionType.Everything)]
        public string codcomprobante { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string codempresa { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string codtipocliente { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codcliente { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codpaciente { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombre { get; set; }
        [DBParameter(SqlDbType.Char, 4, ActionType.Everything)]
        public string cama { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codmedico { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codatencion { get; set; }
        [DBParameter(SqlDbType.Char, 12, ActionType.Everything)]
        public string codpresotor { get; set; }
        [DBParameter(SqlDbType.Char, 16, ActionType.Everything)]
        public string codpoliza { get; set; }
        [DBParameter(SqlDbType.Char, 5, ActionType.Everything)]
        public string planpoliza { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public double deducible { get; set; }
        [DBParameter(SqlDbType.Char, 4, ActionType.Everything)]
        public string codaseguradora { get; set; }
        [DBParameter(SqlDbType.Char, 7, ActionType.Everything)]
        public string codcia { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajecoaseguro { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajeimpuesto { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechagenera { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaemision { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechacancelacion { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaanulacion { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montodctoplan { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoplan { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string moneda { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montototal { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoigv { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoneto { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codplan { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montognc { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montopaciente { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoaseguradora { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string observacion { get; set; }
        [DBParameter(SqlDbType.Char, 3, ActionType.Everything)]
        public string codcentro { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string coduser { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string estado { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombremedico { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombreaseguradora { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombrecia { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventadevolucion { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal tipocambio { get; set; }
        [DBParameter(SqlDbType.Char, 14, ActionType.Everything)]
        public string codpedido { get; set; }
        [DBParameter(SqlDbType.VarChar, 8, ActionType.Everything)]
        public string usuarioanulacion { get; set; }
        [DBParameter(SqlDbType.VarChar, 60, ActionType.Everything)]
        public string nombrediagnostico { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string flagpaquete { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public string fecha_envio { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fecha_entrega { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public Boolean flg_gratuito { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public Boolean flg_enviosap { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fec_enviosap { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int ide_docentrysap { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fec_docentrysap { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public int ide_tablaintersap { get; set; }
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombreestado { get; set; }
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombretipocliente { get; set; }
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombrealmacen { get; set; }
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombreplan { get; set; }
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string autorizado { get; set; }
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string codcomprobantee { get; set; }
    }
}
