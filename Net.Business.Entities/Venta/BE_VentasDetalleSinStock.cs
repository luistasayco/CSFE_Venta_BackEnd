using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("VentasDetalle")]
    public class BE_VentasDetalleSinStock
    {
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalleLote", Type = typeof(List<BE_VentasDetalleLote>))]
        public List<BE_VentasDetalleLote> listVentasDetalleLotes { get; set; }
    }
}
