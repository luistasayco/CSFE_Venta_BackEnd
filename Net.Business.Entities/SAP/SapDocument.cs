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
        public List<SapDocumentLines> DocumentLines { get; set; }

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

    }

    public class SapSelectDocument
    {
        public long DocEntry { get; set; }
    }
}
