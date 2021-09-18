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
    [XmlRoot("ConsolidadoPedido")]
    public class DtoConsolidadoPedidoRegistrar: EntityBase
    {
        [DataMember, XmlAttribute]
        public int Opcion { get; set; }
        [DataMember, XmlAttribute]
        public int IdConsolidado { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListaConsolidadoPedidos", Type = typeof(List<BE_ConsolidadoPedidoDetalle>))]
        public List<BE_ConsolidadoPedidoDetalle> listaConsolidadoPedidos { get; set; }

        public BE_ConsolidadoPedidoXml RetornaModelo()
        {

            var entiDom = new BE_ConsolidadoPedidoXml();
            var ser = new Serializador();
            var ms = new MemoryStream();
            ser.SerializarXml(this, ms);
            entiDom.XmlData = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();

            return new BE_ConsolidadoPedidoXml
            {
                Opcion = this.Opcion,
                idconsolidado = this.IdConsolidado,
                XmlData = entiDom.XmlData,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
