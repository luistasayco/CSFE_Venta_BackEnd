using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_Planilla : EntityBase
    {
        public BE_Planilla()
        {
            this.planilladetalle = new List<BE_PlanillaDetalle>();
        }
        [DBParameter(SqlDbType.Char, ActionType.Everything)]
        public string numeroplanilla { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int NroPlanilla { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string numerogrupo { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string coduser { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fecha { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal monto { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montodolares { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal tc_oficial { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal tc { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string estado { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string coduserreceptor { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fecharecepcion { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int idproceso { get; set; }
        [DBParameter(SqlDbType.Char, 12, ActionType.Everything)]
        public string idasiento { get; set; }
        
        [DBParameter(SqlDbType.Char, 100, ActionType.Everything)]
        public string nombreusuario { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int regcreateidusuario { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime regcreate { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int regupdateidusuario { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime regupdate { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public string existe_anulacion { get; set; }
        public bool flg_enviosap { get; set; }
        //public int flg_anulado { get; set; }
        //public int flgeliminado { get; set; }
        public string codcentro { get; set; }
        public string codcomprobante { get; set; }
        public string ingresoegreso { get; set; }
        //public string accion { get; set; }

        public string campo { get; set; }
        public int idusuario { get; set; }
        public virtual IList<BE_PlanillaDetalle> planilladetalle { get; set; }
    }

    public class BE_ReportePlanilla
    {
        public string area { get; set; }
        public string usuario { get; set; }
        public string numeroplanilla { get; set; }
        public double tc { get; set; }
        public string documento { get; set; }
        public string paciente { get; set; }
        public double ingresos { get; set; }
        public double egresos { get; set; }
        public string docreferencia { get; set; }
        public string movimiento { get; set; }
        public string fechaplanilla { get; set; }
        public string documentoe { get; set; }
    }

    public class BE_ReportePlanillaDetalle
    {
        public string numeroplanilla { get; set; }
        public string numerogrupo { get; set; }
        public string codcomprobante { get; set; }
        public DateTime? fechacancelacion { get; set; }
        public string anombrede { get; set; }
        public string tipocliente { get; set; }
        public string nombretipocliente { get; set; }
        public decimal monto { get; set; }
        public decimal montoparcial { get; set; }
        public string tipopago { get; set; }
        public string nombreentidad { get; set; }
        public string moneda { get; set; }
        public DateTime? fechaplanilla { get; set; }
        public string coduser { get; set; }
        public string nombreusuario { get; set; }
        public string estado { get; set; }
        public decimal m_sumagrupo { get; set; }
        public decimal montodolares { get; set; }
        public string nombrecentro { get; set; }
    }

    public class BE_ReportePlanillaResumen
    {
        public string tipopago { get; set; }
        public string entidad { get; set; }
        public string moneda { get; set; }
        public double monto { get; set; }
        public int totallinea { get; set; }
    }

    public class BE_ReportePlanillaResumenFind
    {
        public string tipopago { get; set; }
        public string entidad { get; set; }
        public double dolares { get; set; }
        public double soles { get; set; }
        public int totallinea { get; set; }
    }
}
