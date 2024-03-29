﻿namespace Net.Business.Entities
{
    public class BE_PedidoDetalle
    {
        public string codproducto { get; set; }
        public string nombreproducto { get; set; }
        public double cantidad { get; set; }
        public string tipoproducto { get; set; }
        public string codpedido { get; set; }
        public decimal igv { get; set; }
        public decimal cantidadpicking { get; set; }
        public bool binactivat { get; set; }
        public string codalmacen { get; set; }
    }

    public class BE_PedidoDetalle_Mensaje
    {
        public string codproducto { get; set; }
        public string nombreproducto { get; set; }
    }
}
