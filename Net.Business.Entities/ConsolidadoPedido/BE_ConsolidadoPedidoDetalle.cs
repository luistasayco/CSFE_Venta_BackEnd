using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("BE_ConsolidadoPedidoDetalle")]
    public class BE_ConsolidadoPedidoDetalle
    {
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codpedido { get; set; }
    }
}
