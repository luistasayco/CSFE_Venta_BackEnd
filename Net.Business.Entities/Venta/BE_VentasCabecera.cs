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
    [XmlRoot("Cabecera")]
    public class BE_VentasCabecera: EntityBase
    {
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, ActionType.Everything, true)]
        public string codventa { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipomovimiento { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 11, ActionType.Everything)]
        public string codcomprobante { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string codempresa { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string codtipocliente { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codcliente { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codpaciente { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombre { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 4, ActionType.Everything)]
        public string cama { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codmedico { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codatencion { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 12, ActionType.Everything)]
        public string codpresotor { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 16, ActionType.Everything)]
        public string codpoliza { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 5, ActionType.Everything)]
        public string planpoliza { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public double deducible { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 4, ActionType.Everything)]
        public string codaseguradora { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 7, ActionType.Everything)]
        public string codcia { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajecoaseguro { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajeimpuesto { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechagenera { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaemision { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime? fechacancelacion { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime? fechaanulacion { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montodctoplan { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal porcentajedctoplan { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string moneda { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montototal { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoigv { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoneto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codplan { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montognc { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montopaciente { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal montoaseguradora { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string observacion { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 3, ActionType.Everything)]
        public string codcentro { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string coduser { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string estado { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombremedico { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombreaseguradora { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string nombrecia { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventadevolucion { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal tipocambio { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 14, ActionType.Everything)]
        public string codpedido { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 8, ActionType.Everything)]
        public string usuarioanulacion { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.VarChar, 60, ActionType.Everything)]
        public string nombrediagnostico { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string flagpaquete { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime? fecha_envio { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime? fecha_entrega { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public Boolean flg_gratuito { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public Boolean flg_enviosap { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fec_enviosap { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int ide_docentrysap { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fec_docentrysap { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public int ide_tablaintersap { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombreestado { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombretipocliente { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombrealmacen { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string nombreplan { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 50, ActionType.Everything)]
        public string autorizado { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 60, ActionType.Everything)]
        public string codcomprobantee { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool tienedevolucion { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string ruccliente { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 60, ActionType.Everything)]
        public string dircliente { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string tipdocidentidad { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 15, ActionType.Everything)]
        public string docidentidad { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string nombretipdocidentidad { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string correocliente { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string moneda_comprobantes { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string numeroplanilla { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalle", Type = typeof(List<BE_VentasDetalle>))]
        public List<BE_VentasDetalle> listaVentaDetalle { get; set; }
        [DataMember, XmlIgnore]
        [DBParameter(SqlDbType.VarChar, 20, ActionType.Everything)]
        public string nombremaquina { get; set; }
        [DataMember, XmlAttribute]
        public string usuario { get; set; }
        [DataMember, XmlIgnore]
        public string motivoanulacion { get; set; }
        [DataMember, XmlAttribute]
        public bool flgsinstock { get; set; }
        [DataMember, XmlIgnore]
        public List<BE_VentasGenerado> listVentasGenerado { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalleUbicacion", Type = typeof(List<BE_VentasDetalleUbicacion>))]
        public List<BE_VentasDetalleUbicacion> listVentasDetalleUbicacion { get; set; }
        [DataMember, XmlIgnore]
        public string cardcode { get; set; }
        [DataMember, XmlIgnore]
        public string strTienedevolucion { get; set; }
        [DataMember, XmlAttribute]
        public int idborrador { get; set; }
        [DataMember, XmlIgnore]
        public string CuentaEfectivoPago { get; set; }
        [DataMember, XmlAttribute]
        public int idparaquien { get; set; }
        [DataMember, XmlAttribute]
        public string codclienteparaquien { get; set; }
        [DataMember, XmlAttribute]
        public string cardcodeparaquien { get; set; }
        [DataMember, XmlIgnore]
        public int ide_receta { get; set; }

        //Datos de Factura
        [DataMember, XmlIgnore]
        public string anombredecomprobante { get; set; }
        [DataMember, XmlIgnore]
        public string ruccomprobante { get; set; }
        [DataMember, XmlIgnore]
        public string direccioncomprobante { get; set; }
        [DataMember, XmlIgnore]
        public string docidentidadcomprobante { get; set; }
        [DataMember, XmlIgnore]
        public string nombretipdocidentidadcomprobante { get; set; }

        // Campos de usuario para SAP
        [DataMember, XmlIgnore]
        public string U_SYP_CS_DNI_PAC { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_NOM_PAC { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_RUC_ASEG { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_NOM_ASEG { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDTD { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDSD { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDCD { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_STATUS { get; set; }
        [DataMember, XmlIgnore]
        public string NumAtCard { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDTO { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDSO { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDCO { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_FECHA_REF { get; set; }
        [DataMember, XmlIgnore]
        public string FederalTaxID { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_MDMT { get; set; }
        [DataMember, XmlIgnore]
        public string Comments { get; set; }
        [DataMember, XmlIgnore]
        public string JournalMemo { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_USUARIO { get; set; }
        [DataMember, XmlIgnore]
        public string ControlAccount { get; set; }
            [DataMember, XmlIgnore]
            public string U_SYP_CS_OA_CAB { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_PAC_HC { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_FINI_ATEN { get; set; }
        [DataMember, XmlIgnore]
        public string U_SYP_CS_FFIN_ATEN { get; set; }
        [DataMember, XmlIgnore]
        public string U_SBA_TIPONC { get; set; }
    }
}
