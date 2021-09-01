﻿using Net.Connection.Attributes;
using System;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_VentasDevolucion
    {
        /// <summary>
        /// Filtro
        /// </summary>
        [DBParameter(SqlDbType.Char, 1, ActionType.Everything)]
        public string  opcion { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codatencion { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventa { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DBParameter(SqlDbType.VarChar, 50, ActionType.Everything)]
        public string nombrealmacen { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechaemision { get; set; }
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DBParameter(SqlDbType.VarChar, 200, ActionType.Everything)]
        public string nombreproducto { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int cantidad { get; set; }
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int cnt_dev { get; set; }
        [DBParameter(SqlDbType.VarChar, 200, ActionType.Everything)]
        public string nombrelaboratorio { get; set; }
        [DBParameter(SqlDbType.Char, 2, ActionType.Everything)]
        public string tipomovimiento { get; set; }
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool manbtchnum { get; set; }
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool binactivat { get; set; }
    }
}
