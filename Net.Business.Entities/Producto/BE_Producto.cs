namespace Net.Business.Entities
{
    public class BE_Producto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal QuantityOnStock { get; set; }
        //public decimal AvgStdPrice { get; set; }
        public decimal AvgStdPrice { get => 20; }
        //public string U_SYP_CS_LABORATORIO { get; set; }
        public string U_SYP_CS_LABORATORIO { get => "GLAXO OTC"; }
        public string U_SYP_CS_FAMILIA { get; set; }
        public string codproducto { get => ItemCode.Substring(4).PadLeft(8, '0'); }
    }
}
