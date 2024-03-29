﻿using System;

namespace Net.Business.Entities
{
    public class BE_ConsolidadoPedidoPicking: EntityBase
    {
        public int idconsolidadopicking { get; set; }
        public int idconsolidado { get; set; }
        public string codpedido { get; set; }
        public string codproducto { get; set; }
        public decimal cantidad { get; set; }
        public decimal cantidadpicking { get; set; }
        public string lote { get; set; }
        public DateTime? fechavencimiento { get; set; }
        public string codalmacen { get; set; }
        public int? ubicacion { get; set; }
        public string codusuarioapu { get; set; }
        public string nombreubicacion { get; set; }
        public string nombreproducto { get; set; }
        public int estado { get; set; }
        public int idreserva { get; set; }
    }
}
