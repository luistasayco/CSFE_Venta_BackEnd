using System;
using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class SapDocument
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapDocumentLines> DocumentLines { get; set; }
        /// <summary>
        /// Tipo de documento
        /// </summary>
        public string U_SYP_MDTD { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string Reference2 { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }
    }

    public class SapDocumentNota
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapDocumentLinesNota> DocumentLines { get; set; }
        /// <summary>
        /// Tipo de documento
        /// </summary>
        public string U_SYP_MDTD { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        //public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }
        public string U_SYP_CS_RUC_ASEG { get; set; }
        public string U_SYP_CS_NOM_ASEG { get; set; }
        public string U_SYP_MDSD { get; set; }
        public string U_SYP_MDCD { get; set; }
        public string U_SYP_STATUS { get; set; }
        public string NumAtCard { get; set; }
        public string U_SYP_MDTO { get; set; }
        public string U_SYP_MDSO { get; set; }
        public string U_SYP_MDCO { get; set; }
        public string U_SYP_FECHA_REF { get; set; }
        public string U_SYP_FECHAREF { get; set; }
        public string FederalTaxID { get; set; }
        public string JournalMemo { get; set; }
        public string U_SYP_CS_USUARIO { get; set; }
        public string ControlAccount { get; set; }
        public string U_SYP_CS_FINI_ATEN { get; set; }
        public string U_SYP_CS_FFIN_ATEN { get; set; }
        public string U_SBA_TIPONC { get; set; }
    }

    public class SapDocumentReserva
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public string WareHouseUpdateType { get; set; }
        public string ReserveInvoice { get; set; }
        public List<SapDocumentLinesReserva> DocumentLines { get; set; }
        public string NumAtCard { get; set; }
        public decimal DocRate { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string U_SYP_CS_SEDE { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        //public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        public string U_SYP_MDTD { get; set; }
        public string U_SYP_MDSD { get; set; }
        public string U_SYP_MDCD { get; set; }
        public string U_SYP_STATUS { get; set; }
        public string JournalMemo { get; set; }
        public string U_SYP_CS_FINI_ATEN { get; set; }
        public string U_SYP_CS_USUARIO { get; set; }
        public string U_SYP_CS_OA { get; set; }
        public string U_SYP_CS_PAC_HC { get; set; }
        public string ControlAccount { get; set; }
        public string U_SYP_CS_FFIN_ATEN { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        public string U_SYP_CS_RUC_ASEG { get; set; }
        public string U_SYP_CS_NOM_ASEG { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }
        public string U_SYP_CS_PRESOTOR { get; set; }
    }

    public class SapDocumentBase
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public int PaymentGroupCode { get; set; }
        public List<SapDocumentLinesBase> DocumentLines { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string Reference2 { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }

    }

    public class SapDocumentFacturaBase
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public int PaymentGroupCode { get; set; }
        public List<SapDocumentFacturaLinesBase> DocumentLines { get; set; }
        public string NumAtCard { get; set; }
        public decimal DocRate { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string U_SYP_CS_SEDE { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        //public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        public string U_SYP_MDTD { get; set; }
        public string U_SYP_MDSD { get; set; }
        public string U_SYP_MDCD { get; set; }
        public string U_SYP_STATUS { get; set; }
        public string JournalMemo { get; set; }
        public string U_SYP_CS_FINI_ATEN { get; set; }
        public string U_SYP_CS_USUARIO { get; set; }
        public string U_SYP_CS_OA_CAB { get; set; }
        public string U_SYP_CS_PAC_HC { get; set; }
        public string ControlAccount { get; set; }
        public string U_SYP_CS_FFIN_ATEN { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        public string U_SYP_CS_RUC_ASEG { get; set; }
        public string U_SYP_CS_NOM_ASEG { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }
    }

    public class SapDocumentUpdateDeliveryBase
    {
        
        public string U_SYP_MDMT { get; set; }

    }

    public class SapDocumentReturnBase
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapDocumentLinesBase> DocumentLines { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string Reference2 { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }

    }

    public class SapDocumentReturn
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapDocumentLines> DocumentLines { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string Reference2 { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_TVENTA { get; set; }
    }

    public class SapDocumentReservaBase
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public DateTime TaxDate { get; set; }
        public string Comments { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapDocumentLinesBase> DocumentLines { get; set; }
        /// <summary>
        /// Num. identificador externo
        /// </summary>
        public string U_SYP_EXTERNO { get; set; }
        /// <summary>
        /// Codigo Externo + Tipo de transferencia
        /// </summary>
        public string Reference2 { get; set; }
        /// <summary>
        /// Tipo Salida
        /// </summary>
        public string U_SYP_MDTS { get; set; }
        /// <summary>
        /// Motivo de traslado
        /// </summary>
        public string U_SYP_MDMT { get; set; }
        /// <summary>
        /// Timo de salida de mercancias
        /// </summary>
        public string U_SYP_TPOSALME { get; set; }
        /// <summary>
        /// Almacenero
        /// </summary>
        public string U_SYP_ALMACENERO { get; set; }
        /// <summary>
        /// Solicitante
        /// </summary>
        public string U_SYP_SOLICITANTE { get; set; }
        /// <summary>
        /// Código Presotor
        /// </summary>
        public string U_SYP_CS_PRESOTOR { get; set; }
        /// <summary>
        /// Código Orden Atención
        /// </summary>
        public string U_SYP_CS_OA { get; set; }
        /// <summary>
        /// Dni - Paciente
        /// </summary>
        public string U_SYP_CS_DNI_PAC { get; set; }
        /// <summary>
        /// Nombre - Paciente
        /// </summary>
        public string U_SYP_CS_NOM_PAC { get; set; }
        /// <summary>
        /// Código Paciente - Historia Clínica
        /// </summary>
        public string U_SYP_CS_PAC_HC { get; set; }
        /// <summary>
        /// Movimiento origen en sistema CSFE
        /// </summary>
        public string U_SYP_CS_MOV_ORIGEN { get; set; }
        /// <summary>
        /// Registro de Origen
        /// </summary>
        public string U_SBA_ORIG { get; set; }
        public string U_SYP_MDTD { get; set; }
        public string U_SYP_TVENTA { get; set; }
    }

    public class SapSelectDocument
    {
        public long DocEntry { get; set; }
    }

    public class SapDocumentFacturaAnular
    {
        public string U_SYP_STATUS { get; set; }
        public string U_SYP_CS_USUANU { get; set; }
        public string U_SYP_CS_MOTANU { get; set; }
        public string U_SYP_CS_FECANU { get; set; }
    }

    public class SapProject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ValidFrom { get; set; }
        //public string ValidTo { get; set; }
        public string Active { get; set; }
        public string U_SYP_CS_ORIGEN { get; set; }
    }
}
