using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class BusinessPartners
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public int GroupCode { get; set; }
        public string MailAddress { get; set; }
        public string MailZipCode { get; set; }
        public string Phone1 { get; set; }
        public string FederalTaxID { get; set; }
        public string EmailAddress { get; set; }
        public string FreeText { get; set; }
        public string U_SYP_BPAP { get; set; }
        public string U_SYP_BPAM { get; set; }
        public string U_SYP_BPNO { get; set; }
        public string U_SYP_BPN2 { get; set; }
        public string U_SYP_BPTP { get; set; }
        public string U_SYP_BPTD { get; set; }
        public List<Addresses> BPAddresses { get; set; }
    }

    public class Addresses
    {
        public string AddressName { get; set; }
        public string Street { get; set; }
        public string Block { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
    }
}
