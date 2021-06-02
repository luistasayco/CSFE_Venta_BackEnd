using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_VentasDetalle
    {
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventa { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipomovimiento { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int cantidad { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int cantidad_fraccion { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal preciounidadcondcto { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal precioventaPVP { get; set; }
        //[DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        //public decimal valorVVF { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal valorVVP { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int stockalmacen { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int stockalm_fraccion { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoproducto { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montototal { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montopaciente { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoaseguradora { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechagenera { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaemision { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int stockfraccion { get; set; }
        //[DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        //public decimal costocompra { get; set; }
        //[DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        //public decimal promedio { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string estado { get; set; }
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string gnc { get; set; }
        [DBParameter(SqlDbType.Char, 14, ActionType.Everything)]
        public string codpedido { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int cant_traentcon { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int cant_deventcon { get; set; }
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int cant_tramencon { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public decimal totalconigv { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal totalsinigv { get; set; }
        [DBParameter(SqlDbType.VarChar, 200, ActionType.Everything)]
        public string nombreproducto { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoplan { get; set; }
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajecoaseguro { get; set; }
        [DBParameter(SqlDbType.Float, 0, ActionType.Everything)]
        public double valor_dscto { get; set; }
        public bool narcotico { get; set; }
        public decimal igvproducto { get; set; }
        public string codtipoproducto { get; set; }
        public string manBtchNum { get; set; }
        public bool flgbtchnum { get; set; }
        public BE_VentasDetalleDatos VentasDetalleDatos { get; set; }
        public List<BE_StockLote> listStockLote { get; set; }
    }
}
