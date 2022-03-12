namespace Net.Business.Entities
{
    public class SapReserveStock
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string U_ITEMCODE { get; set; }
        public int? U_BINABSENTRY { get; set; }
        public decimal? U_QUANTITY { get; set; }
        public string U_IDEXTERNO { get; set; }
        public string U_BATCHNUM { get; set; }
        public string U_WHSCODE { get; set; }
    }

    public class SapReserveStockUpdate
    {
        public decimal U_QUANTITY { get; set; }
    }

    public class SapReserveStockNew
    {
        public string Name { get; set; }
        public string U_ITEMCODE { get; set; }
        public int U_BINABSENTRY { get; set; }
        public decimal U_QUANTITY { get; set; }
        public string U_IDEXTERNO { get; set; }
        public string U_BATCHNUM { get; set; }
        public string U_WHSCODE { get; set; }
    }
}
