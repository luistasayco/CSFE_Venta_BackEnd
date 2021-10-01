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
        public List<SapDocumentLinesBase> DocumentLines { get; set; }

    }
}
