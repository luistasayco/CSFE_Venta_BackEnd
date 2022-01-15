namespace Net.Business.Entities
{
    public class BE_PedidoDevolucion
    {
        public int id_tmp { get; set; }
        public string desprod { get; set; }
        public int cantmen { get; set; }
        public int cantmendev { get; set; }
        public decimal prevtaPVP { get; set; }
        public decimal pdsctoprod { get; set; }
        public decimal pdsctoplan { get; set; }
        public decimal qmontotal { get; set; }
        public decimal qmonpaciente { get; set; }
        public decimal qmonasegurado { get; set; }
        public string codproducto { get; set; }
        public decimal valorVVP { get; set; }
        public decimal preundscto { get; set; }
        public string gnc { get; set; }
        public string codatencion { get; set; }
        public string codalmacen { get; set; }
        public string desalmacen { get; set; }
        public string tipopaciente { get; set; }
        public string destipopaciente { get; set; }
        public bool manbtchnum { get; set; }
        public bool binactivat { get; set; }
        public decimal stockalmacen { get; set; }
    }
}
