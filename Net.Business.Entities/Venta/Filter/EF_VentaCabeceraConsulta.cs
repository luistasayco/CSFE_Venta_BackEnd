using Net.Connection.Attributes;
using System.Data;

namespace Net.Business.Entities
{
    public class EF_VentaCabeceraConsulta
    {
        [DBParameter(SqlDbType.Char, 100, ActionType.Everything)]
        public string buscar { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int key { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int numerolineas { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int orden { get; set; }
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string fecha { get; set; }

    }
}
