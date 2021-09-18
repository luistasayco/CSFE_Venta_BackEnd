using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot(ElementName= "SeperacionCuentaGenerado")]
    public class BE_SeperacionCuentaGenerado
    {
        [DataMember]
        [XmlElement(ElementName = "ListSeperacionCuentaGenerado")]
        public List<BE_SeperacionCuenta> ListSeperacionCuentaGenerado { get; set; }
    }

    [XmlRoot(ElementName = "ListSeperacionCuentaGenerado")]
    public class BE_SeperacionCuenta
    {
        [XmlElement(ElementName = "tipomovimiento")]
        public string tipomovimiento { get; set; }
        [XmlElement(ElementName = "codventa")]
        public string codventa { get; set; }
    }
}
