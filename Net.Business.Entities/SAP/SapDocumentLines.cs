using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class SapDocumentLines
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string TaxCode { get; set; }
        public string AccountCode { get; set; }
        public string WarehouseCode { get; set; }
        public string CostingCode { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string U_SYP_EXT_LINEA { get; set; }
        public List<SapBatchNumbers> BatchNumbers { get; set; }
        public List<SapBinAllocations> DocumentLinesBinAllocations { get; set; }
    }

    public class SapDocumentLinesBase
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string TaxCode { get; set; }
        public string AccountCode { get; set; }
        public string WarehouseCode { get; set; }
        public string CostingCode { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        // Documento Base => Asociar Documentos
        public int? BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string U_SYP_EXT_LINEA { get; set; }
        public List<SapBatchNumbers> BatchNumbers { get; set; }
        public List<SapBinAllocations> DocumentLinesBinAllocations { get; set; }
    }
}
