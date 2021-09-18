namespace Net.Business.Entities
{
    public class BE_ConsolidadoSolicitud
    {
        public string DesAlmacenOrigen { get; set; }
        public string DesAlmacenDestino { get; set; }
        public string CodArticulo { get; set; }
        public string DesArticulo { get; set; }
        public decimal CantidadProducto { get; set; }
        public string NumLote { get; set; }
        public decimal CantidadLote { get; set; }
        public string GroupName { get; set; }
        public string CodLaboratorio { get; set; }
    }
}
