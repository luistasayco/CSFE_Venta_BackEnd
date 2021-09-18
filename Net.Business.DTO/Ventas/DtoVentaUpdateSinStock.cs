using Net.Business.Entities;
using Net.CrossCotting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Net.Business.DTO
{
    [DataContract]
    [Serializable]
    [XmlRoot("Cabecera")]
    public class DtoVentaUpdateSinStock: EntityBase
    {
        [DataMember, XmlAttribute]
        public string codventa { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalle", Type = typeof(List<BE_VentasDetalleSinStock>))]
        public List<BE_VentasDetalleSinStock> listVentasDetalle { get; set; }

        public BE_VentaXml RetornaModelo()
        {

            var entiDom = new BE_VentaXml();
            var ser = new Serializador();
            var ms = new MemoryStream();
            ser.SerializarXml(this, ms);
            entiDom.XmlData = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();

            return new BE_VentaXml
            {
                codventa = this.codventa,
                XmlData = entiDom.XmlData,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
