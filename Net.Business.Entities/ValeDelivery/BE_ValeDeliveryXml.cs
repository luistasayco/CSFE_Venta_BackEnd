namespace Net.Business.Entities
{
    public class BE_ValeDeliveryXml: EntityBase
    {
        public int idvaledelivery { get; set; }
        public string XmlData { get; set; }
    }

    public class BE_ValeDeliveryUpdEstado : EntityBase
    {
        public int ide_receta { get; set; }
        public string estadovd { get; set; }
    }
}
