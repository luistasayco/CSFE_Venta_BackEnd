﻿using System;

namespace Net.Business.Entities
{
    public class BE_ComprobanteElectronico
    {
        public string generarnota { get; set; }
        public string mensaje { get; set; }
        public string codcomprobante { get; set; }
        public string codcomprobantee { get; set; }
        public string tipoplantilla { get; set; }
        public string tipocomp_sunat { get; set; }
        public string tipocomp_tci { get; set; }
        public string estado { get; set; }
        public string nombreestado { get; set; }
        public string codtipocliente { get; set; }
        public string tipomovimiento { get; set; }
        public string codventa { get; set; }
        public DateTime fechaemision { get; set; }
        public string anombrede { get; set; }
        public string direccion { get; set; }
        public string ruc { get; set; }
        //public string tipodocidentidad_sunat { get; set; }
        public string tipdocidentidad_sunat { get; set; }

        public string ruc_sunat { get; set; }
        public string receptor_correo { get; set; }
        public string empresa_ruc { get; set; }
        public string empresa_tipodocidentidad { get; set; }
        public string empresa_nombre { get; set; }
        public string empresa_dicreccion { get; set; }
        public string empresa_telefono { get; set; }
        public string concepto { get; set; }
        public string codatencion { get; set; }
        public string codpaciente { get; set; }
        public string nombrepaciente { get; set; }
        public string nombretitular { get; set; }
        public string nombrecia { get; set; }
        public string codpoliza { get; set; }
        public string observaciones { get; set; }
        public string texto1 { get; set; }
        public string suc_direccion { get; set; }
        public string tipopagotxt { get; set; }


        public string ref_electronico { get; set; }
        public string ref_codcomprobantee { get; set; }
        public string ref_tipocomp_sunat { get; set; }
        public DateTime? ref_fechaemision { get; set; }
        public string c_codmotivo_sunat { get; set; }
        public string c_motivonota { get; set; }


        public string c_moneda { get; set; }
        public string c_nombremoneda { get; set; }
        public string c_simbolomoneda { get; set; }
        public decimal c_montodsctoplan { get; set; }
        public decimal c_montoafecto { get; set; }
        public decimal c_montoinafecto { get; set; }
        public decimal c_montoexonerado { get; set; }
        public decimal c_montogratuito { get; set; }
        public decimal c_montoigv { get; set; }
        public decimal c_montoneto { get; set; }
        public decimal porcentajeimpuesto { get; set; }
        public decimal porcentajecoaseguro { get; set; }
        public string flg_gratuito { get; set; }
        public string d_orden { get; set; }
        public string d_unidad { get; set; }
        public string d_codproducto { get; set; }
        public string nombreproducto { get; set; }
        public decimal d_cant_sunat { get; set; }
        public int d_stockfraccion { get; set; }
        public int d_cantidad { get; set; }
        public int d_cantidad_fraccion { get; set; }
        public decimal d_valorVVP { get; set; }
        public decimal d_precioventaPVP { get; set; }
        public decimal d_porcdctoproducto { get; set; }
        public decimal d_porcdctoplan { get; set; }
        public decimal d_preciounidadcondcto { get; set; }
        public decimal d_montototal { get; set; }
        public decimal d_montopaciente { get; set; }
        public decimal d_montoaseguradora { get; set; }
        public decimal d_ventaunitario_sinigv { get; set; }
        public decimal d_ventaunitario_conigv { get; set; }
        public decimal d_dscto_sinigv { get; set; }
        public decimal d_dscto_conigv { get; set; }
        public decimal d_total_sinigv { get; set; }
        public decimal d_total_conigv { get; set; }
        public decimal d_total_sinigv2 { get; set; }
        public decimal d_montoigv { get; set; }
        public string d_determinante { get; set; }
        public string d_codigotipoprecio { get; set; }
        public string d_afectacionigv { get; set; }
        public decimal d_montobase { get; set; }
        public string tipo_operacion { get; set; }
        public string hora_emision { get; set; }
        public decimal total_impuesto { get; set; }
        public decimal total_valorventa { get; set; }
        public decimal total_precioventa { get; set; }
        public string vs_ubl { get; set; }
        public decimal mt_total { get; set; }
        public decimal mtg_Base { get; set; }
        public decimal mtg_ValorImpuesto { get; set; }
        public decimal mtg_Porcentaje { get; set; }
        public decimal d_dscto_unitario { get; set; }
        public decimal d_total_condsctoigv { get; set; }
        public decimal d_dscto_montobase { get; set; }
        public string CodigoEstabSUNAT { get; set; }
        public string CodDistrito { get; set; }
        public string forma_pago { get; set; }
        public string cod_tributo { get; set; }
        public string cod_afectacionIGV { get; set; }
        public string des_tributo { get; set; }
        public string cod_UN { get; set; }
        public string CodProductoSUNAT { get; set; }
        public string cuadre { get; set; }
        public string cardcode { get; set; }
        public string cardcodeparaquien { get; set; }
        public string codalmacen { get; set; }
        public int baseentry { get; set; }
        public int baseline { get; set; }
        // Datos para SAP
        public string AccountCode { get; set; }
        public string CostingCode { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public bool manbtchnum { get; set; }
        public bool binactivat { get; set; }
        public bool flgsinstock { get; set; }
        //extra
        public string strXml { get; set; }
        public string nomtipocliente { get; set; }
        public string CuentaEfectivoPago { get; set; }
        public string codventadevolucion { get; set; }
        public string tipodocidentidad_sunat { get; set; }
        // Campos de usuario en SAP
        public string NumAtCard { get; set; }
        public decimal DocRate { get; set; }
        public string U_SYP_CS_SEDE { get; set; }
        public string U_SYP_CS_DNI_PAC { get; set; }
        public string U_SYP_CS_NOM_PAC { get; set; }
        public string U_SYP_CS_RUC_ASEG { get; set; }
        public string U_SYP_CS_NOM_ASEG { get; set; }
        public string U_SYP_MDTD { get; set; }
        public string U_SYP_MDTO { get; set; }
        public string U_SBA_TIPONC { get; set; }
        public string FederalTaxID { get; set; }
        public string U_SYP_FECHA_REF { get; set; }
        public string U_SYP_MDCO { get; set; }
        public string U_SYP_MDSO { get; set; }
        public string U_SYP_MDSD { get; set; }
        public string U_SYP_MDCD { get; set; }
        public string U_SYP_STATUS { get; set; }
        public string U_SYP_MDMT { get; set; }
        public string Comments { get; set; }
        public string JournalMemo { get; set; }
        public string U_SYP_CS_USUARIO { get; set; }
        public string Rounding { get; set; }
        public string RoundDif { get; set; }
        public decimal d_ventaunitario_sinigv_g { get; set; }
        public string ControlAccount { get; set; }
        public string U_SYP_CS_OA_CAB { get; set; }
        public string U_SYP_CS_PAC_HC { get; set; }
        public string U_SYP_CS_FINI_ATEN { get; set; }
        public string U_SYP_CS_FFIN_ATEN { get; set; }
        public string TaxCode { get; set; }
        public string codpresotor { get; set; }
        // DETALLE:
        public string TaxOnly { get; set; }
        public string Project { get; set; }
        public string U_SYP_CS_OA { get; set; }
        public string U_SYP_CS_DNI_MED { get; set; }
        public string U_SYP_CS_NOM_MED { get; set; }
        public string U_SYP_CS_RUC_MED { get; set; }
        
    }
}
