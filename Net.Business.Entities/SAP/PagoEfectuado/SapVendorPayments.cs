using System;
using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class SapVendorPayments
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public string CashAccount { get; set; }
        public decimal CashSum { get; set; }
        public string CounterReference { get; set; }
        public DateTime TaxDate { get; set; }
        public DateTime DueDate { get; set; }
        public string DocObjectCode { get; set; }
        public List<SapPaymentInvoices> PaymentInvoices { get; set; }

    }
}
