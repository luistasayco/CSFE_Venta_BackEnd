﻿using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("VentasDetalle")]
    public class BE_VentasDetalle
    {
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventa { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipomovimiento { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int cantidad { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal preciounidadcondcto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal precioventaPVP { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal valorVVP { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int stockalmacen { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montototal { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montopaciente { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoaseguradora { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechagenera { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaemision { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int stockfraccion { get; set; }
        //[DataMember, XmlAttribute]
        //[DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        //public string estado { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string gnc { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 14, ActionType.Everything)]
        public string codpedido { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public decimal totalconigv { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal totalsinigv { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.VarChar, 200, ActionType.Everything)]
        public string nombreproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoplan { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajecoaseguro { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Float, 0, ActionType.Everything)]
        public double valor_dscto { get; set; }
        [DataMember, XmlAttribute]
        public bool narcotico { get; set; }
        [DataMember, XmlAttribute]
        public decimal igvproducto { get; set; }
        [DataMember, XmlAttribute]
        public string codtipoproducto { get; set; }
        [DataMember, XmlAttribute]
        public bool manBtchNum { get; set; }
        [DataMember, XmlAttribute]
        public bool flgbtchnum { get; set; }
        [DataMember, XmlAttribute]
        public bool flgnarcotico { get; set; }
        [DataMember, XmlAttribute]
        public decimal preciounidad { get; set; }
        [DataMember, XmlAttribute]
        public int cnt_dev { get; set; }
        [DataMember, XmlAttribute]
        public int cntxdev { get; set; }
        [DataMember]
        [XmlElement(ElementName = "VentasDetalleDatos", Type = typeof(BE_VentasDetalleDatos))]
        public BE_VentasDetalleDatos VentasDetalleDatos { get; set; }
        [DataMember, XmlIgnore]
        public List<BE_StockLote> listStockLote { get; set; }
        [DataMember, XmlAttribute]
        public bool binactivat { get; set; }
        [DataMember, XmlAttribute]
        public bool flgbin { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalleLote", Type = typeof(List<BE_VentasDetalleLote>))]
        public List<BE_VentasDetalleLote> listVentasDetalleLotes { get; set; }
        [DataMember, XmlIgnore]
        public int cantidad_fraccion { get; set; }
        [DataMember, XmlIgnore]
        public int cnt_unitario { get; set; }
        [DataMember, XmlIgnore]
        public decimal prc_unitario { get; set; }
        [DataMember, XmlIgnore]
        public decimal cantsunat { get; set; }
        [DataMember, XmlIgnore]
        public string destributo { get; set; }
        [DataMember, XmlIgnore]
        public decimal d_ventaunitario_sinigv { get; set; }
        [DataMember, XmlIgnore]
        public int baseentry { get; set; }
        [DataMember, XmlIgnore]
        public int baseline { get; set; }

        // Datos para SAP
        [DataMember, XmlIgnore]
        public string AccountCode { get; set; }
        [DataMember, XmlIgnore]
        public string CostingCode { get; set; }
        [DataMember, XmlIgnore]
        public string CostingCode2 { get; set; }
        [DataMember, XmlIgnore]
        public string CostingCode3 { get; set; }
        [DataMember, XmlIgnore]
        public string CostingCode4 { get; set; }
        [DataMember, XmlIgnore]
        public int baseentrydevolucion { get; set; }
        [DataMember, XmlIgnore]
        public int baselinedevolucion { get; set; }
        [DataMember, XmlIgnore]
        public string nombrelaboratorio { get; set; }
        [DataMember, XmlIgnore]
        public string TaxCode { get; set; }
        [DataMember, XmlIgnore]
        public string TaxOnly { get; set; }
        [DataMember, XmlIgnore]
        public string Project { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_OA { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_DNI_MED { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_NOM_MED { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_RUC_MED { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_PROYECTO { get; set; }
    }

    [DataContract]
    [Serializable]
    [XmlRoot("VentasDetalleLoteLecturado")]
    public class VentasDetalleLoteLecturado
    {
        [DataMember, XmlAttribute]
        public string codproducto { get; set; }
        [DataMember, XmlAttribute]
        public string lote { get; set; }
        [DataMember, XmlAttribute]
        public string nombreproducto { get; set; }
        [DataMember, XmlAttribute]
        public decimal cantidad { get; set; }
    }
}
