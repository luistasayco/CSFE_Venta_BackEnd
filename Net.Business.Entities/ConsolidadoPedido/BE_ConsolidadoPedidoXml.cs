using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_ConsolidadoPedidoXml : EntityBase
    {
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int Opcion { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int idconsolidado { get; set; }
        [DBParameter(SqlDbType.Xml, 0, ActionType.Everything)]
        public string XmlData { get; set; }
    }
}
