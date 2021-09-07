using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class SapIncomingPayments
    {
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public decimal CashSum { get; set; }

        //I-Cheque
        public string CheckAccount { get; set; }
        //F-Cheque

        //I-Transferencia
        public string TransferAccount { get; set; }
        public decimal TransferSum { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferReference { get; set; }
        //F-Transferencia

        public string CounterReference { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocObjectCode { get; set; }
        public DateTime DueDate { get; set; }
        public SapPaymentInvoices PaymentInvoices { get; set; }
        public SapPaymentChecks PaymentChecks { get; set; }
    }
}
